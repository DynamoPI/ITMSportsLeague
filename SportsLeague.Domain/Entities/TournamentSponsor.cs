namespace SportsLeague.Domain.Entities
{
    public class TournamentSponsor : AuditBase
    {
        // 1. IDs de conexión (Foreign Keys)
        public int TournamentId { get; set; }
        public int SponsorId { get; set; }

        // 2. Datos propios de esta relación 
        public decimal ContractAmount { get; set; }
        public DateTime JoinedAt { get; set; }

        // 3. Propiedades de Navegación 
        public virtual Tournament Tournament { get; set; } = null!;
        public virtual Sponsor Sponsor { get; set; } = null!;
    }
}
