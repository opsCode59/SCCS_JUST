using API.Data;
using API.DTOs;
using API.Entities;
using API.Repository;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers;
    public class AccountController : BaseApiController
        {
    private readonly AppDbContext _context;
    private readonly ITokenRepository _tokenRepository;
    public AccountController(AppDbContext _context,ITokenRepository _tokenRepository )
    {
        this._context = _context;
        this._tokenRepository= _tokenRepository;
    }
    //[HttpPost("register")]
    //public async Task<ActionResult<AppUser>> register(RegisterDto registerDto)
    //    {
    //    if(await UserExists(registerDto.username)) return BadRequest("Username has Taken");
    //    using var hmac = new HMACSHA512();
    //    var user = new AppUser()
    //        {
    //        Name = registerDto.username.ToLower(),
    //        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
    //        PasswordSalt=hmac.Key,
    //        };
    //     _context.appUsers.Add(user);
    //    await _context.SaveChangesAsync();
    //    return user;
    //    }
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> register(RegisterDto registerDto )
        {
        if(await UserExists(registerDto.username)) return BadRequest("Username is Taken");
        using var hmac = new HMACSHA512();
        var user = new AppUser()
            {
            Name= registerDto.username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
            PasswordSalt = hmac.Key,
            };
        await _context.appUsers.AddAsync(user);
        await _context.SaveChangesAsync();
        return new UserDto
            {
            Username=user.Name,
            Token=_tokenRepository.CreateToken(user)
            };
        }
    //[HttpPost("login")]
    //public async Task<ActionResult<AppUser> >Login(LoginDto loginDto )
    //    {
    //    var user = await _context.appUsers.SingleOrDefaultAsync(user => user.Name == loginDto.username);
    //    if (user==null)return Unauthorized("Username is not Found");
    //    using var hmac = new HMACSHA512(user.PasswordSalt);
    //    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));
    //    for(int i=0;i<computedHash.Length;i++)
    //        {
    //        if (computedHash[i] != user.PasswordHash[i])
    //            return Unauthorized("Password is Not Matching");
    //        }
    //    return user;
    //    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto )
        {
        var user = await _context.appUsers.SingleOrDefaultAsync(e => e.Name == loginDto.username);
        if (user == null)
            return Unauthorized("User is Not Found");
        using var hmac= new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));
        for(int i = 0; i < computedHash.Length; i++)
            {
            if (computedHash[i] != user.PasswordHash[i])
                {
                return Unauthorized("Password is not Matching");
                }
            }
        return new UserDto
            {
            Username = user.Name,
            Token = _tokenRepository.CreateToken(user)
            };
        }
    public async Task<bool> UserExists(string username )
        {
        return await _context.appUsers.AnyAsync(user =>user.Name==username.ToLower());
        }
}
