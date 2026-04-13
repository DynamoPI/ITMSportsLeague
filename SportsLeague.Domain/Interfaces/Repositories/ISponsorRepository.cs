using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    namespace SportsLeague.Domain.Repository
    {
        public interface ISponsorRepository : IGenericRepository<Sponsor>
        {
            Task<Sponsor> AddAsync(Sponsor sponsor);
            Task AddTournamentRelationAsync(TournamentSponsor relation);
            Task<Sponsor?> GetByIdWithTournamentsAsync(int id);
        }
    }
}
