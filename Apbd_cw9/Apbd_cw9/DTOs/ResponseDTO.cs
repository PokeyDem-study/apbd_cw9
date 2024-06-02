namespace Apbd_cw9.DTOs;

public class ResponseDTO
{
    public int pageNum { get; set; }
    public int pageSize { get; set; }
    public int allPages { get; set; }
    public IEnumerable<TripDTO> Trips { get; set; } = new List<TripDTO>();
}