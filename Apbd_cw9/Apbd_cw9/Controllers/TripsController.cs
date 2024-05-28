using Apbd_cw9.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apbd_cw9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ApbdContext _context;
    public TripsController(ApbdContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTrips()
    {
        //always add .ToListAsync() to the end of linq request
        var trips = await _context.Trips.Select(
            e => new
            {
                Name = e.Name,
                Countries = e.IdCountries.Select(c => new
                {
                    Name = c.Name
                })
            }).ToListAsync();
        return Ok(trips);
    }
}