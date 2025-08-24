using Cultural_Heritage_System.Dtos.Response;

namespace Cultural_Heritage_System.Services
{
    public interface IHeritageLocationService
    {
        Task<List<HeritageLocationResponse>> GetHeritagesWithCoordinates();
    }
}
