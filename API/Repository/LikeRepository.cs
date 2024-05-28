using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace API.Repository;


public class LikeRepository : ILikeRepository
    {
    private readonly AppDbContext appDbContext;

    public LikeRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
        }
    public async Task<UserLike> GetUserLike( int sourceUserId, int targetUserId )
        {
        return await appDbContext.userLikes.FindAsync(sourceUserId, targetUserId);
        }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
        var users =  appDbContext.appUsers.OrderBy(u=>u.UserName).AsQueryable();

        var likes = appDbContext.userLikes.AsQueryable();

        if (likesParams.Predicate == "liked")
            {
            likes = likes.Where(like => like.SourceUserId == likesParams.userId);
            users = likes.Select(like => like.TargetUser);
            }

        if (likesParams.Predicate == "likedBy")
            {
            likes = likes.Where(like => like.TargetUserId == likesParams.userId);
            users = likes.Select(like => like.SourceUser);
            }

       var likedUsers= users.Select(user => new LikeDto
            {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
            City = user.City,
            Id = user.Id,
            });
        return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

    public async Task<AppUser> GetUserWithLikes( int userId )
        {
        return await appDbContext.appUsers
            .Include(u => u.LikedUser)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
public interface ILikeRepository
    {
    public Task<UserLike> GetUserLike( int sourceUserId, int targetUserId );
    public Task<AppUser> GetUserWithLikes(int userId);
    public Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }