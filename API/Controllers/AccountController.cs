using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AccountController : ControllerBase
  {
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)
    {
      _tokenService = tokenService;
      _context = context;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserTokenDTO>> Register(RegisterUserDTO regUser)
    {

      if (await IsUserExists(regUser.UserName))
        return BadRequest("UserName is Already Exists");

      using var hmac = new HMACSHA512();

      var user = new AppUser
      {
        UserName = regUser.UserName.ToLower(),
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regUser.Password)),
        PasswordSalt = hmac.Key,
      };

      _context.ApplicationUsers.Add(user);

      await _context.SaveChangesAsync();

      return new UserTokenDTO
      {
        UserName = user.UserName,
        Token = _tokenService.CreateToken(user)
      };
    }



    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserTokenDTO>> Login([FromBody] LoginUserDTO loginUser)
    {
      AppUser user = await _context.ApplicationUsers.SingleOrDefaultAsync(x => x.UserName == loginUser.UserName);

      if (user == null)
        return Unauthorized("Invalid UserName");

      using var hmac = new HMACSHA512(user.PasswordSalt);

      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginUser.Password));

      for (int i = 0; i < computedHash.Length; i++)
      {
        if (computedHash[i] != user.PasswordHash[i])
          return Unauthorized("Invalid Password");
      }

      return new UserTokenDTO
      {
        UserName = user.UserName,
        Token = _tokenService.CreateToken(user)
      };
    }
    private async Task<bool> IsUserExists(string userName)
    {
      return await _context.ApplicationUsers.AnyAsync(user => user.UserName == userName.ToLower());
    }

  }
}