using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class MatchLineupService : IMatchLineupService
{
    private readonly IMatchLineupRepository _matchLineupRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly MatchValidationHelper _validationHelper;

    // Inyección de Dependencias: El framework nos pasa los repositorios automáticamente
    public MatchLineupService(
        IMatchLineupRepository matchLineupRepository,
        IMatchRepository matchRepository,
        MatchValidationHelper validationHelper)
    {
        _matchLineupRepository = matchLineupRepository;
        _matchRepository = matchRepository;
        _validationHelper = validationHelper;
    }

    public async Task<MatchLineup> CreateAsync(MatchLineup lineup)
    {
        // V1: El partido debe existir en la base de datos
        var match = await _matchRepository.GetByIdAsync(lineup.MatchId);
        if (match == null)
            throw new KeyNotFoundException($"No se encontró el partido con ID {lineup.MatchId}"); 

        // V6: Validación por estado del partido (Solo en Scheduled)
        if (match.Status != MatchStatus.Scheduled)
            throw new InvalidOperationException("Solo se pueden registrar alineaciones en partidos Scheduled"); 

        // V2 & V3: El jugador debe existir y pertenecer al HomeTeam o AwayTeam del partido
        // Reutilizamos el helper inyectable de la Fase 5 tal como recomendó el profe
        var player = await _validationHelper.ValidatePlayerInMatchAsync(lineup.PlayerId, match); 

        // V4: El jugador no puede estar registrado dos veces en la misma alineación del mismo partido
        var alreadyExists = await _matchLineupRepository.ExistsByMatchAndPlayerAsync(lineup.MatchId, lineup.PlayerId);
        if (alreadyExists)
            throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido"); 

        // V5: Máximo 11 titulares por equipo por partido (Solo aplica si viene como IsStarter = true)
        if (lineup.IsStarter) 
        {
            // Traemos de la bodega los jugadores ya convocados para este partido y este equipo específico
            var currentTeamLineups = await _matchLineupRepository.GetByMatchAndTeamAsync(lineup.MatchId, player.TeamId);

            // Usamos LINQ para contar cuántos de esos registros ya tienen el rol de Titular (IsStarter == true)
            var starterCount = currentTeamLineups.Count(l => l.IsStarter);

            if (starterCount >= 11)
                throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido"); 
        }

        // Si pasó todas las aduanas, se guarda en la base de datos
        return await _matchLineupRepository.CreateAsync(lineup);
    }

    // Método para eliminar un jugador de la alineación (Requerimiento DELETE)
    public async Task DeleteAsync(int id)
    {
        // Validamos si el registro de la alineación existe en la BD
        var exists = await _matchLineupRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"No se encontró el registro de alineación con ID {id}");

        await _matchLineupRepository.DeleteAsync(id);
    }

    // Método para obtener la alineación filtrada por equipo (Requerimiento GET)
    public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
    {
        // Validamos primero que el partido exista antes de ir a buscar
        var matchExists = await _matchRepository.ExistsAsync(matchId);
        if (!matchExists)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        return await _matchLineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
    }

    // Método para obtener la alineación completa de un partido (Requerimiento GET)
    public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
    {
        var matchExists = await _matchRepository.ExistsAsync(matchId);
        if (!matchExists)
            throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

        return await _matchLineupRepository.GetByMatchAsync(matchId);
    }
}