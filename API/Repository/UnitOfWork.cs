using API.Data;
using AutoMapper;

namespace API.Repository;
public class UnitOfWork : IUnitOfWork
    {
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public UnitOfWork(AppDbContext appDbContext,IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
        }
    public IUserRepository UserRepository => new UserRepository(appDbContext,mapper);

    public IMessageRepository messageRepository => new MessageRepository(appDbContext, mapper);

    public ILikeRepository likeRepository => new LikeRepository(appDbContext);
    public IFeedbackRepository FeedbackRepository => new FeedbackRepository(appDbContext);
    public IPhotoRepo photoRepo => new PhotoRepo(appDbContext);
    public IAppointmentRepository appointmentRepository => new AppointmentRepository(appDbContext);
  public IScheduleRepository scheduleRepository => new ScheduleRepository(appDbContext);
    public IPostRepository postRepository => new PostRepository(appDbContext);
    public async Task<bool> Complate()
        {
        return await appDbContext.SaveChangesAsync() > 0;
        }

    public bool HasChanges()
        {
        return appDbContext.ChangeTracker.HasChanges();
        }
    }
public interface IUnitOfWork
    {
    IUserRepository UserRepository { get; }
    IFeedbackRepository FeedbackRepository { get; }
    IMessageRepository messageRepository { get; }
    ILikeRepository likeRepository { get; }
    IPhotoRepo photoRepo { get; }
    IAppointmentRepository appointmentRepository { get; }
    IScheduleRepository scheduleRepository { get; }
    IPostRepository postRepository { get; }
    Task<bool> Complate();
    bool HasChanges();
    }
