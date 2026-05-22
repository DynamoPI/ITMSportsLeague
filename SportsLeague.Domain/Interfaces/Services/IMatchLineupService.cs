using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services;

public interface IMatchLineupService
{
    // 1. POST /api/match/{matchId}/lineup -> Agregar jugador
    Task<MatchLineup> CreateAsync(MatchLineup lineup);

    // 2. GET /api/match/{matchId}/lineup -> Obtener alineación completa
    Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);

    // 3. GET /api/match/{matchId}/lineup/team/{teamId} -> Obtener por equipo específico
    Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);

    // 4. DELETE /api/match/{matchId}/lineup/{id} -> Eliminar jugador de la alineación
    Task DeleteAsync(int id);
}