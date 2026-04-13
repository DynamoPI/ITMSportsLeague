using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories.SportsLeague.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace SportsLeague.DataAccess.Repositories
{
    public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
    {
        public SponsorRepository(LeagueDbContext context) : base(context) { }

        // Sobrescribimos el Add para limpiar el nombre
        public async Task<Sponsor> AddAsync(Sponsor sponsor)
        {
            sponsor.Name = sponsor.Name.Trim();
            return await base.CreateAsync(sponsor);
        }

        // Obtiene el sponsor incluyendo la relación con los torneos
        public async Task<Sponsor?> GetByIdWithTournamentsAsync(int id)
        {
            return await _context.Sponsors
                .Include(s => s.TournamentSponsors)
                    .ThenInclude(ts => ts.Tournament)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Agrega la relación en la tabla intermedia
        public async Task AddTournamentRelationAsync(TournamentSponsor relation)
        {
            await _context.Set<TournamentSponsor>().AddAsync(relation);
            await _context.SaveChangesAsync();
        }

        // Elimina la relación de la tabla intermedia
        public async Task RemoveTournamentRelationAsync(int sponsorId, int tournamentId)
        {
            var relation = await _context.Set<TournamentSponsor>()
                .FirstOrDefaultAsync(ts => ts.SponsorId == sponsorId && ts.TournamentId == tournamentId);

            if (relation != null)
            {
                _context.Set<TournamentSponsor>().Remove(relation);
                await _context.SaveChangesAsync();
            }
        }
    }
}