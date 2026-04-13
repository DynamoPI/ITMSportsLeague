using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories.SportsLeague.Domain.Repository;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Consultando todos los patrocinadores");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Consultando patrocinador con ID: {Id}", id);
            return await _sponsorRepository.GetByIdAsync(id);
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetAllAsync();
            if (existing.Any(s => s.Name.Trim().ToLower() == sponsor.Name.Trim().ToLower()))
            {
                throw new InvalidOperationException($"El patrocinador '{sponsor.Name}' ya existe.");
            }

            _logger.LogInformation("Creando patrocinador: {Name}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;
            existing.UpdatedAt = DateTime.Now;

            _logger.LogInformation("Actualizando patrocinador con ID: {Id}", id);
            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");

            _logger.LogInformation("Eliminando patrocinador con ID: {Id}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

        public async Task AssignToTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
        {
            // 1. Validamos que el sponsor exista
            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (sponsor == null)
                throw new KeyNotFoundException($"No se encontró el patrocinador con ID {sponsorId}");

            // 2. Creamos la entidad intermedia
            var relation = new TournamentSponsor
            {
                SponsorId = sponsorId,
                TournamentId = tournamentId,
                ContractAmount = contractAmount,
                JoinedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            _logger.LogInformation("Vinculando Sponsor {SponsorId} al Torneo {TournamentId}", sponsorId, tournamentId);

            // 3. Llamamos al método que creamos en el SponsorRepository
            await _sponsorRepository.AddTournamentRelationAsync(relation);
        }
        public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            var sponsor = await _sponsorRepository.GetByIdWithTournamentsAsync(sponsorId);
            if (sponsor == null) throw new KeyNotFoundException("Sponsor no encontrado");

            // Extraemos solo los torneos de la tabla intermedia
            return sponsor.TournamentSponsors.Select(ts => ts.Tournament);
        }

        public async Task UnlinkTournamentAsync(int sponsorId, int tournamentId)
        {
            await _sponsorRepository.RemoveTournamentRelationAsync(sponsorId, tournamentId);
        }
    }
}