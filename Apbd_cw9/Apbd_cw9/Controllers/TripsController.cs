using Apbd_cw9.Data;
using Apbd_cw9.DTOs;
using Apbd_cw9.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apbd_cw9.Controllers;

[ApiController]
[Route("api/")]
public class TripsController : ControllerBase
{
    private readonly ApbdContext _context;
    public TripsController(ApbdContext context)
    {
        _context = context;
    }
    
    [HttpGet("trips")]
    public async Task<IActionResult> GetTrips(int _pageNum = 1, int _pageSize = 10)
    {
        var trips = await _context.Trips.Select(
            e => new
            {
                Name = e.Name,
                Description = e.Description,
                DateFrom = e.DateFrom,
                DateTo = e.DateTo,
                MaxPeople = e.MaxPeople,
                Countries = e.IdCountries.Select(c => new
                {
                    Name = c.Name
                }),

                Clients = e.ClientTrips.Select(t => new
                {
                    FirstName = t.IdClientNavigation.FirstName,
                    LastName = t.IdClientNavigation.LastName
                })

            }).Take(_pageNum * _pageSize).ToListAsync();

        var _allPages = trips.Count; 
        var res = new
        {
            pageNum = _pageNum,
            pageSize = _pageSize,
            allPages = _allPages,
            trips
        };
        
        return Ok(res);
    }

    [HttpDelete("clients/{id:int}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var clientTrips = await _context.ClientTrips.
            Select(t => t.IdClient.Equals(id)).ToListAsync();

        if (clientTrips.Count > 0)
        {
            return Conflict("Can't delete client that has trips");
        }

        var clientToDelete = _context.Clients.Single(c => c.IdClient.Equals(id));

        _context.Remove(clientToDelete);

        await _context.SaveChangesAsync();
        
        return Ok();
    }

    [HttpPost("trips/{idTrip:int}/clients")]
    public async Task<IActionResult> AddClientToTrip(ClientTripDTO clientTripData)
    {
        
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Pesel.Equals(clientTripData.Pesel));

        if (client != null)
        {
            return Conflict("Client already exists");
        }
      
        if (client != null){
            
            //Nie wiem jaki jest w ogole sens to sparawdzac. Jezeli klient juz istnieje zwracamy blad, wiec, jezeli przechodzimy dalej nie jest to mozliwe, ze klient bedzie zapisany na
            //wycieczke, bo nie istnieje, ale w zadaniu jest to wymagane, wiec dodalem
            
            if (client.ClientTrips.Any(ct => ct.IdTrip == clientTripData.IdTrip))
            {
                return Conflict("Client already enrolled");
            }
        }
        
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip.Equals(clientTripData.IdTrip));
        if (trip == null)
        {
            return NotFound("Trip doesn't exist");
        }
      
        var tripDate = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip.Equals(clientTripData.IdTrip));
        if (tripDate.DateTo <= DateTime.Now)
        {
            Forbid("Trip already over");
        }
        
        var newClient = new Client()
        {
            FirstName = clientTripData.FirstName,
            LastName = clientTripData.LastName,
            Email = clientTripData.Email,
            Telephone = clientTripData.Telephone,
            Pesel = clientTripData.Pesel
        };
        await _context.Clients.AddAsync(newClient);
        await _context.SaveChangesAsync();

        var newClientId = _context.Clients.FirstOrDefaultAsync(c => c.Pesel.Equals(clientTripData.Pesel)).Result!.IdClient;

        var newClientTrip = new ClientTrip()
        {
            IdClient = newClientId,
            IdTrip = clientTripData.IdTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = clientTripData.PaymentDate
        };

        await _context.ClientTrips.AddAsync(newClientTrip);
        await _context.SaveChangesAsync();

        return Ok();
    }
}