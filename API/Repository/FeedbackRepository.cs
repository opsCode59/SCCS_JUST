using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;

public class FeedbackRepository : IFeedbackRepository
    {
    private readonly AppDbContext appDbContext;

    public FeedbackRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
        }

    public async Task AddAsync( Feedback feedback )
        {
        await appDbContext.feedbacks.AddAsync(feedback);
        }

    public async Task DeleteAsync( int id )
        {
         
        var feedback =await appDbContext.feedbacks.FindAsync(id);
         appDbContext.feedbacks.Remove(feedback);
        }

    public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
        return await appDbContext.feedbacks.ToListAsync();
        }

    public async Task<IEnumerable<Feedback>> GetAllFeedbackByIdAsync( int userId )
        {
        return await appDbContext.feedbacks.Where(x => x.AppUserId == userId).ToListAsync();
        }

    public async Task<Feedback> GetByUserNameAsync( string username )
        {
        return await appDbContext.feedbacks.FirstOrDefaultAsync(x=>x.userName==username);
        }
    }
public interface IFeedbackRepository
        {
    // void AddFeedback( Feedback feedback );

    Task<IEnumerable<Feedback>> GetAllAsync();
    Task<Feedback> GetByUserNameAsync( string username );
    Task AddAsync( Feedback feedback );
    Task DeleteAsync( int id );
    Task<IEnumerable<Feedback>> GetAllFeedbackByIdAsync( int userId );
    }

