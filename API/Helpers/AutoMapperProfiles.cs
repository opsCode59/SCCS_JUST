using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
    {
    public class AutoMapperProfiles : Profile
        {
        public AutoMapperProfiles()
            {

            CreateMap<Feedback, CreateFeedbackDto>();
            CreateMap<CreateFeedbackDto, Feedback>();



            CreateMap<Post, PostDto>();
            CreateMap<PostDto, Post>();


            CreateMap<Post, CreatePostDto>();
            CreateMap<CreatePostDto, Post>();


            CreateMap<Schedule, CreateScheduleDto>();
            CreateMap<CreateScheduleDto, Schedule>();


            CreateMap<Appointment, AppointmentWithIdDto>();
            CreateMap<AppointmentWithIdDto, Appointment>();



            CreateMap<Appointment, AppointmentDto>();
            CreateMap<AppointmentDto, Appointment>();



            CreateMap<FeedbackDto, Feedback>();
            CreateMap<Feedback, FeedbackDto>();
            CreateMap<AppUser, MemberDto>()
               
                .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(photo => photo.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos
                .FirstOrDefault(x => x.IsMain).Url))

                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos
                .FirstOrDefault(x => x.IsMain).Url));
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ?
                DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
            }

        }
    }
