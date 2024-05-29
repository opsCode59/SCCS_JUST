using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;
public class PostRepository : IPostRepository
    {
    private readonly AppDbContext appDbContext;

    public PostRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
        }
    public async Task AddPost( Post post )
        {
            await appDbContext.posts.AddAsync( post );
        }

    public async Task<bool> DeletePost( int id )
        {
        var post = await appDbContext.posts.FindAsync( id );
        if (post != null)
            {
            appDbContext.posts.Remove(post);
            return true;
            }
        return false;
        }

    public async Task<IEnumerable<Post>> GetAllPosts()
        {
        return await appDbContext.posts.ToListAsync();
        }

    public async Task<IEnumerable<Post>> GetAllPostsBySenderUserName( string username )
        {
        return await appDbContext.posts.Where(x => x.SenderUsername == username).ToListAsync();
        }

    public async Task<Post> GetPostById( int id )
        {
        return await appDbContext.posts.FindAsync(id);
        }

    public void UpdatePost( Post post )
        {
          appDbContext.Entry(post).State = EntityState.Modified;
        }
    }
public interface IPostRepository
    {
        Task AddPost(Post post);
        Task<bool> DeletePost(int id);
        void UpdatePost(Post post);
    Task<Post> GetPostById(int id);
    Task <IEnumerable<Post>> GetAllPosts();
    Task<IEnumerable<Post>> GetAllPostsBySenderUserName( string username);

    }
