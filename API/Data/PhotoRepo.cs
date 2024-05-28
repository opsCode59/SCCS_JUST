using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class PhotoRepo : IPhotoRepo
    {
    private readonly AppDbContext appDbContext;

    private readonly AppDbContext _context;
    public PhotoRepo( AppDbContext context )
        {
        _context = context;
        }
    public async Task<Photo> GetPhotoById( int id )
        {
        return await _context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id);
        }


    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
        return await _context.Photos
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(u => new PhotoForApprovalDto
                {
                Id = u.Id,
                userName = u.AppUser.UserName,
                Url = u.Url,
                isApproved = u.IsApproved
                }).ToListAsync();
        }

    public void RemovePhoto( Photo photo )
        {
        _context.Photos.Remove(photo);
        }
    }
public interface IPhotoRepo
    {
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
    Task<Photo> GetPhotoById( int id );
    void RemovePhoto( Photo photo );
    }
