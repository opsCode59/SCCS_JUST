using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
    {
    [Authorize]
    public class UserController : BaseApiController
        {
        private readonly AppDbContext _context;
        public UserController( AppDbContext _context )
            {
            this._context = _context;
            }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
            {
            return await _context.appUsers.FindAsync(id);
            }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
            {
            var users = await _context.appUsers.ToListAsync();
            return users;
            }
        }
    }
