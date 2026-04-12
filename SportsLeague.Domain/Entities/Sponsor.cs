using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Entities
{
    public class Sponsor : AuditBase
    {
        public string Name { get; set; } = string.Empty; //required
        public string ContactEmail { get; set; } = string.Empty; //required
        public string? Phone { get; set; } //optional
        public string? WebsiteUrl { get; set; } //optional

        public SponsorCategory Category { get; set; } 

        public virtual ICollection<TournamentSponsor> TournamentSponsors { get; set; } = new List<TournamentSponsor>();

    }
}
