using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using System;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace API.Repository;
public class UserRepository : IUserRepository
    {
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public UserRepository(AppDbContext appDbContext,IMapper mapper) { 
        this.appDbContext = appDbContext;
        this.mapper = mapper;
        }

    public void DeleteUser( int id )
        {
        var user = appDbContext.Users.Find(id);
          appDbContext.Users.Remove(user);
        }

    public async Task<MemberDto> GetMemberAsync( string username, bool
  isCurrentUser )
        {
        var query = appDbContext.Users
        .Where(x => x.UserName == username)
        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        .AsQueryable();
        if (isCurrentUser)
            query = query.IgnoreQueryFilters();
        return await query.FirstOrDefaultAsync();
        }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
        var query = appDbContext.appUsers.AsQueryable();
        query = query.Where(user => user.UserName != userParams.CurrentUsername);
        query = query.Where(user => user.Gender == userParams.Gender);

        // Convert minDob and maxDob to DateTime
        // Convert minDob and maxDob to DateTime with DateTimeKind.Utc
        DateTime minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1).ToUniversalTime();
        DateTime maxDob = DateTime.Today.AddYears(-userParams.MinAge).ToUniversalTime();

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);



        query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive),
             };
        return await PagedList<MemberDto>.CreateAsync(
            query.AsNoTracking().ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            , userParams.PageNumber,
            userParams.PageSize);
        }

    public async Task<AppUser> GetUserByIdAsync( int id )
        {
        return await appDbContext.appUsers.FindAsync(id);
        }

    public async Task<AppUser> GetUserByPhotoId( int photoId )
        {
        return await appDbContext.Users
            .Include(x => x.Photos)
            .IgnoreQueryFilters()
            .Where(x => x.Photos.Any(p => p.Id == photoId))
            .FirstOrDefaultAsync();
        }

    public async Task<AppUser> GetUserByUsernameAsync( string username )
        {
            return await appDbContext.appUsers
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(user=>user.UserName==username);
        }

    public async Task<string> GetUserGender( string username )
        {
            return await appDbContext.Users.Where(x=>x.UserName==username)
            .Select(x=>x.Gender).FirstOrDefaultAsync();
        }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
        return await appDbContext.appUsers.Include(p => p.Photos).ToListAsync();
        }

    public void Update( AppUser user )
        {
        appDbContext.Entry(user).State = EntityState.Modified;
        }
    }
public interface IUserRepository
    {
    public void Update(AppUser user);
    public Task<IEnumerable<AppUser>> GetUsersAsync();
    public Task <AppUser> GetUserByIdAsync(int id);
    public Task<AppUser> GetUserByUsernameAsync( string username );
    public Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    public Task<MemberDto> GetMemberAsync( string username,bool  isCurrentUser);
    public Task<string> GetUserGender( string username );
    Task<AppUser> GetUserByPhotoId( int photoId );
    public void DeleteUser( int id );
    }
