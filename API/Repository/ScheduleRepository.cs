using API.Data;
using API.Entities;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;
public class ScheduleRepository : IScheduleRepository
    {
    private readonly AppDbContext appDbContext;

    public ScheduleRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
        }
    public async Task AddAsync( Schedule schedule )
        {
         await appDbContext.schedules.AddAsync( schedule );
        }

    public async Task<bool> DeleteAsync( int id )
        {
        var schedule = await appDbContext.schedules.FindAsync(id);
        if (schedule != null)
            {
            appDbContext.schedules.Remove(schedule);
            return true;
            }
        return false;
        }

    public async Task<bool> deleteSchedule( string userName, string clinicUserName )
        {
        var schedule = await GetScheduleByUserNameClinicUserName(userName, clinicUserName);
        if (schedule!= null)
            {
            appDbContext.schedules.Remove(schedule);
            return true;
            }
        return false;
        }
    public async Task<Schedule> GetScheduleByUserNameClinicUserName( string userName, string clinicUserName )
        {
        var schedule = await appDbContext.schedules.FirstOrDefaultAsync(x => x.userName == userName && x.clinicUserName == clinicUserName);
        return schedule;
        }

    public async Task<Schedule> GetScheduleByUserName( string userName )
        {
        return await appDbContext.schedules.FirstOrDefaultAsync(x => x.userName==userName);
        }

    public async Task<IEnumerable<Schedule>> GetSchedulesByClinicUserNameAsync( string clinicUserName )
        {
        return await appDbContext.schedules.Where(x=>x.clinicUserName==clinicUserName).ToListAsync();
        }
    }
public interface IScheduleRepository
    {

    Task<IEnumerable<Schedule>> GetSchedulesByClinicUserNameAsync( string clinicUserName );
    Task<Schedule> GetScheduleByUserName(string userName );   
    Task<bool> DeleteAsync( int id );
    Task AddAsync( Schedule schedule );
    Task<bool> deleteSchedule( string userName, string clinicUserName );
    Task<Schedule> GetScheduleByUserNameClinicUserName( string userName, string clinicUserName );
    }
