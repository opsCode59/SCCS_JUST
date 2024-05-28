using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;
public class AppointmentRepository : IAppointmentRepository
    {
    private readonly AppDbContext appDbContext;

    public AppointmentRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
        }
    public async Task AddAsync( Appointment appointment )
        {
         await appDbContext.appointments.AddAsync( appointment );
        }

    public async Task<bool> DeleteAsync( int id )
        {

        var appointment = await appDbContext.appointments.FindAsync(id);
        if (appointment != null)
            {
             appDbContext.appointments.Remove(appointment);
            return true;
            }
        return false;
        }

    public async Task<IEnumerable<Appointment>> GetAllAppointment(int ClinicId)
        {
        return await appDbContext.appointments
            .Where(x => x.AppUserId == ClinicId)
            .ToListAsync();
        }

    public async Task<Appointment> GetAppointmentById( int id )
        {
        return await appDbContext.appointments.FindAsync( id );
        }

    public async Task<Appointment> GetAppointmentByUsername( string username )
        {
        return await appDbContext.appointments.FirstOrDefaultAsync(x => x.userName == username);
        }
    }
public interface IAppointmentRepository
    {
    Task<Appointment> GetAppointmentByUsername( string username );
    Task<IEnumerable<Appointment>> GetAllAppointment( int ClinicId );
    Task AddAsync( Appointment appointment );
    Task<Appointment> GetAppointmentById( int id );
    Task<bool> DeleteAsync( int id );
    }