using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
    private readonly DataContext _context;
    public UsersController(DataContext context)
    {
      _context = context;
    }


    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<AppUser>>> GetUsers()
    {
      return await _context.ApplicationUsers.ToListAsync();
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
      return await _context.ApplicationUsers.FindAsync(id);
    }
  }
}