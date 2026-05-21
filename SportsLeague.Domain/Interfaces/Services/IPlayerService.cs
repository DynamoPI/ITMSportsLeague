using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IPlayerService
    {
        Task<IEnumerable<Player>> GetAllAsync();  // Consider adding pagination parameters for large datasets
        Task<Player?> GetByIdAsync(int id); // Consider returning a more specific DTO or a Result type to handle not found cases more gracefully
        Task<IEnumerable<Player>> GetByTeamAsync(int teamId);
        Task<Player> CreateAsync(Player player);
        Task UpdateAsync(int id, Player player);
        Task DeleteAsync(int id);
    }

}
