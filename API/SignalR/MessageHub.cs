﻿using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
    {
    [Authorize]
    public class MessageHub:Hub
        {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHubContext<PresenceHub> presenceHub;

        public MessageHub(IUnitOfWork unitOfWork,IMapper mapper,IHubContext<PresenceHub> presenceHub)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.presenceHub = presenceHub;
            }

        public override async Task OnConnectedAsync()
            {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup",group);


            var messages = await unitOfWork.messageRepository
                .GetMessagesThread(Context.User.GetUsername(), otherUser);

            if (unitOfWork.HasChanges())
                await unitOfWork.Complate();
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
            }


        public override async Task OnDisconnectedAsync( Exception? exception )
            {
           var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
            }


        public async Task SendMessage(CreateMessageDto createMessageDto )
            {
            var username = Context.User.GetUsername();
            if (username == createMessageDto.RecipientUsername.ToLower())
                {
                throw new HubException("You cannot send messages to yourself");
                }
            var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if (recipient == null)
                {
                throw new HubException("Not Found");
                }
            var message = new Message()
                {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content,
                };
            var groupName = GetGroupName(sender.UserName,recipient.UserName);
            var group = await unitOfWork.messageRepository.GetMessageGroup(groupName);

            if(group.Connections.Any(x=>x.Username==recipient.UserName))
                {
                message.DateRead = DateTime.UtcNow;
                }
            else
                {
                var connections = await PresenceTracker.GetConnectionForUser(recipient.UserName);
                if (connections != null)
                    {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new {username=sender.UserName,knownAs=sender.KnownAs}
                        );
                    }
                }
            unitOfWork.messageRepository.AddMessage(message);
            if (await unitOfWork.Complate())
                {
                await Clients.Group(groupName).SendAsync("NewMessage",mapper.Map<MessageDto>(message));
                }
            }



        public string GetGroupName(string caller, string other )
            {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
            }


        public async Task<Group> AddToGroup( string groupName )
            {
            var group = await unitOfWork.messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
           if (group == null)
                {
                group = new Group(groupName);
                unitOfWork.messageRepository.AddGroup(group);
                }

           group.Connections.Add(connection);
            if (await unitOfWork.Complate())
                return group;
            throw new HubException("Failed to Add to group");

            }

        private async Task<Group> RemoveFromMessageGroup()
            {
            var group = await unitOfWork.messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x=>x.ConnectionId== Context.ConnectionId);
            unitOfWork.messageRepository.RemoveConnection(connection);
            if (await unitOfWork.Complate())
                return group;
            throw new HubException("Failed to remove from group");
            }


        }
    }
