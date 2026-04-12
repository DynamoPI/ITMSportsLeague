using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories.SportsLeague.Domain.Repository;

namespace SportsLeague.DataAccess.Repositories
{

    public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
    {
        public SponsorRepository(LeagueDbContext context) : base(context) { }

        public async Task<Sponsor> AddAsync(Sponsor sponsor)
        {
            
            sponsor.Name = sponsor.Name.Trim();

           
            return await base.CreateAsync(sponsor);
        }
    }
}