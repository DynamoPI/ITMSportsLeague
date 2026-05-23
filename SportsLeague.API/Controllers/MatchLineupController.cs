using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/match/{matchId}/lineup")] 
public class MatchLineupController : ControllerBase
{
    private readonly IMatchLineupService _lineupService;
    private readonly IMapper _mapper;

    public MatchLineupController(IMatchLineupService lineupService, IMapper mapper)
    {
        _lineupService = lineupService;
        _mapper = mapper;
    }

    // 1. POST: Agregar un jugador a la alineación
    [HttpPost]
    public async Task<ActionResult<MatchLineupDto>> Create([FromRoute] int matchId, [FromBody] CreateMatchLineupDto dto)
    {
        try
        {
            var lineup = _mapper.Map<MatchLineup>(dto);
            lineup.MatchId = matchId; // Capturamos el MatchId directamente de la URL

            var created = await _lineupService.CreateAsync(lineup);

            // Recargamos el registro para jalar los Includes y que AutoMapper no devuelva nombres vacíos
            var matchLineups = await _lineupService.GetByMatchAsync(matchId);
            var createdWithDetails = matchLineups.FirstOrDefault(l => l.Id == created.Id);

            var responseDto = _mapper.Map<MatchLineupDto>(createdWithDetails);

            return StatusCode(201, responseDto); // HTTP 201 Created según la rúbrica
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message }); // HTTP 404
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // HTTP 409
        }
    }

    // 2. GET: Obtener la alineación completa de un partido
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatchLineupDto>>> GetByMatch([FromRoute] int matchId)
    {
        try
        {
            var lineups = await _lineupService.GetByMatchAsync(matchId);
            return Ok(_mapper.Map<IEnumerable<MatchLineupDto>>(lineups)); // HTTP 200
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // 3. GET: Obtener la alineación filtrada por un equipo específico
    [HttpGet("team/{teamId}")]
    public async Task<ActionResult<IEnumerable<MatchLineupDto>>> GetByMatchAndTeam([FromRoute] int matchId, [FromRoute] int teamId)
    {
        try
        {
            var lineups = await _lineupService.GetByMatchAndTeamAsync(matchId, teamId);
            return Ok(_mapper.Map<IEnumerable<MatchLineupDto>>(lineups)); // HTTP 200
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // 4. DELETE: Eliminar un jugador de la alineación
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int matchId, [FromRoute] int id)
    {
        try
        {
            await _lineupService.DeleteAsync(id);
            return NoContent(); // HTTP 204 Exitoso sin contenido
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message }); // HTTP 404
        }
    }
}