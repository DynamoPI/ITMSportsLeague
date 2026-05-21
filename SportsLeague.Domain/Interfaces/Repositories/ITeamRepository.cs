using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITeamRepository : IGenericRepository<Team> // Hereda de un repositorio genérico para operaciones CRUD básicas
    {
        Task<Team?> GetByNameAsync(string name); // Método específico para obtener un equipo por su nombre
        Task<IEnumerable<Team>> GetByCityAsync(string city); // Método específico para obtener equipos por ciudad
    }
}
