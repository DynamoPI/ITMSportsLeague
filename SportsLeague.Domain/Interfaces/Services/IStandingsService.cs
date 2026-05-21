namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IStandingsService
    {
        Task<object> GetStandingsAsync(int tournamentId); // Devuelve una estructura con la tabla de posiciones
        Task<object> GetTopScorersAsync(int tournamentId); // Devuelve una lista de los máximos goleadores del torneo
        Task<object> GetCardStatsAsync(int tournamentId); // Devuelve estadísticas de tarjetas (amarillas, rojas) por equipo y jugador
    }

}
