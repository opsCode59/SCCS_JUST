using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;
public class MessageRepository : IMessageRepository
    {
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public MessageRepository(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
        }

    public void AddGroup( Group group )
        {
        appDbContext.groups.Add(group);
        }

    public void AddMessage( Message message )
        {
        appDbContext.Add( message );
        }

    public void DeleteMessage( Message message )
        {
        appDbContext.Remove( message );
        }

    public async Task<Connection> GetConnection( string connectionId )
        {
        return await appDbContext.connections.FindAsync(connectionId);
        }

    public async Task<Group> GetGroupForConnection( string connectionId )
        {
        return await appDbContext.groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
        }

    public async Task<Message> GetMessageAsync( int id )
        {
        return await appDbContext.messages.FindAsync(id);
        }

    public async Task<Group> GetMessageGroup( string groupName )
        {
        return await appDbContext.groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
        var query = appDbContext.messages
            .OrderByDescending(e => e.MessageSent)
            .AsQueryable();
        query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted ==false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.DateRead == null 
                && u.RecipientDeleted == false),
                };
        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
        return await PagedList<MessageDto>
            .CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
        }

    public async Task<IEnumerable<MessageDto>> GetMessagesThread( string currentUsername, string recipientUsername )
        {
        var query = appDbContext.messages
            .Where(
            m => m.RecipientUsername == currentUsername &&
            m.RecipientDeleted == false &&
            m.SenderUsername == recipientUsername ||
            m.RecipientUsername == recipientUsername &&
            m.SenderUsername == currentUsername &&
            m.SenderDeleted == false
            ).OrderBy(m => m.MessageSent)
            .AsQueryable();

        var unreadMessages = query.Where(m => m.DateRead == null 
        && m.RecipientUsername == currentUsername).ToList();

        if (unreadMessages.Any()){
            foreach(var message in unreadMessages)
                {
                message.DateRead = DateTime.UtcNow;
                }
            }
        return await query.ProjectTo<MessageDto>(mapper.ConfigurationProvider).ToListAsync();
        }

    public void RemoveConnection( Connection connection )
        {
        appDbContext.connections.Remove(connection);
        }

    }
public interface IMessageRepository
    {
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessageAsync(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername,string recipientUsername);
    void AddGroup( Group group );
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnection(string connectionId);
    Task<Group> GetMessageGroup(string groupName);
    Task<Group> GetGroupForConnection( string connectionId );
    }
