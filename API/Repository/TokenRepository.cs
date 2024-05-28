using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Repository;
public class TokenRepository : ITokenRepository
    {
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> userManager;

    //
    public TokenRepository( IConfiguration config,UserManager<AppUser>userManager  )
        {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        this.userManager = userManager;
        }
    public async Task<string> CreateToken( AppUser appUser )
        {
        var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.NameId,appUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName,appUser.UserName),
            };
        var roles = await userManager.GetRolesAsync(appUser);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
            {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
            };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
        }
    }
public interface ITokenRepository
        {
    public Task<string>CreateToken( AppUser appUser );
        }
