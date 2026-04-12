using AutoMapper;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;

namespace SportsLeague.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Definimos un límite de profundidad global o específico.
            // .MaxDepth(5) es un valor seguro y estándar para la mayoría de APIs.

            // Team mappings
            CreateMap<TeamRequestDTO, Team>()
                .MaxDepth(5);

            CreateMap<Team, TeamResponseDTO>() 
                .MaxDepth(5); // Esto asegura que al mapear un Team, no se caerá en ciclos infinitos al intentar mapear sus Players o Tournaments relacionados.

            // Player mappings
            CreateMap<PlayerRequestDTO, Player>();
            CreateMap<Player, PlayerResponseDTO>()
                .ForMember(
                    dest => dest.TeamName,
                    opt => opt.MapFrom(src => src.Team.Name));

            // Referee mappings
            CreateMap<RefereeRequestDTO, Referee>();
            CreateMap<Referee, RefereeResponseDTO>();

            // Tournament mappings
            CreateMap<TournamentRequestDTO, Tournament>();
            CreateMap<Tournament, TournamentResponseDTO>()
                .ForMember(
                    dest => dest.TeamsCount,
                    opt => opt.MapFrom(src =>
                        src.TournamentTeams != null ? src.TournamentTeams.Count : 0));

        }
    }
}
    
