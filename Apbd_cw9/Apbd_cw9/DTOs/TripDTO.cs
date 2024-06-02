using Apbd_cw9.Models;

namespace Apbd_cw9.DTOs;

public class TripDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public IEnumerable<CountryDTO> Countries { get; set; } = new List<CountryDTO>();
    public IEnumerable<ClientDTO> Clients { get; set; } = new List<ClientDTO>();
}