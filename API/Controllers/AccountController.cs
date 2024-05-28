 using API.Data;
using API.DTOs;
using API.Entities;
using API.Repository;
using AutoMapper;
using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers;
    public class AccountController : BaseApiController
        {
    private readonly UserManager<AppUser> userManager;
    private readonly IMapper _mapper;
    private readonly ITokenRepository _tokenRepository;
    public AccountController(UserManager<AppUser> userManager,ITokenRepository _tokenRepository,IMapper _mapper )
    {
        this._mapper = _mapper;
        this.userManager = userManager;
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
    public async Task<ActionResult<UserDto>>register(RegisterDto registerDto )
        {
        if(await UserExists(registerDto.username)) return BadRequest("Username is Taken");
        var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.username.ToLower();
        var result = await userManager.CreateAsync(user,registerDto.password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        var role ="";
        if (registerDto.gender == "Clinic")
            role = "Clinic";
        else
        if (registerDto.gender == "Doctor")
            role = "Doctor";
        else
            role = "Patient";

        var roleResult = await userManager.AddToRoleAsync(user,role);
        if(!roleResult.Succeeded) return BadRequest(result.Errors);

        return new UserDto
            {
            userName = user.UserName,
            Token=await _tokenRepository.CreateToken(user),
            KnownAs=user.KnownAs,
            Gender=user.Gender,
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
        var user = await userManager.Users
            .Include(p=>p.Photos)
            .SingleOrDefaultAsync(e => e.UserName == loginDto.username);
        if (user == null)
            return Unauthorized("User is Not Found");
        var result = await userManager.CheckPasswordAsync(user, loginDto.password);
        if (!result)
            return Unauthorized("Invalid Password");

        return new UserDto
            {
            userName = user.UserName,
            Token = await _tokenRepository.CreateToken(user),
            photoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender=user.Gender,
            };
        }
    public async Task<bool> UserExists(string username )
        {
        return await userManager.Users.AnyAsync(user =>user.UserName==username.ToLower());
        }
}
