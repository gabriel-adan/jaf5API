using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Logic.Contracts;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class TurnsController : ApiBaseController
    {
        private readonly ITurnsLogic turnsLogic;

        public TurnsController(ILogger<ApiBaseController> logger, ITurnsLogic turnsLogic) : base(logger)
        {
            this.turnsLogic = turnsLogic;
        }

        [HttpPost("Request")]
        [Authorize(Roles = "Player")]
        public IActionResult _Request([FromForm] RequestTurnForm requestTurnForm)
        {
            try
            {
                var turnResult = turnsLogic.Request(requestTurnForm.Date, requestTurnForm.HourId, requestTurnForm.TeamId, requestTurnForm.PerfilId);
                return Ok(turnResult);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al solicitar turno");
                return NotFound(new { ErrorMessage = "Ocurrió un error al solicitar turno" });
            }
        }

        [HttpPost("Reserve")]
        [Authorize(Roles = "Owner, Employee")]
        public IActionResult Reserve([FromForm] TurnReserveForm turnReserveForm)
        {
            try
            {
                var turnResult = turnsLogic.Reserve(turnReserveForm.Date, turnReserveForm.HourId, turnReserveForm.Name);
                return Ok(turnResult);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al reservar turno");
                return NotFound(new { ErrorMessage = "Ocurrió un error al reservar turno" });
            }
        }

        [HttpPost("CreateTeamTurn")]
        [Authorize(Roles = "Player")]
        public IActionResult Create([FromForm] CreateTurnForm createTurnForm)
        {
            try
            {
                var turnResult = turnsLogic.CreateTeamTurn(createTurnForm.Date, createTurnForm.HourId, createTurnForm.Name, createTurnForm.IsPrivate, createTurnForm.PerfilId);
                return Ok(turnResult);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear el equipo");
                return NotFound(new { ErrorMessage = "Ocurrió un error al crear el equipo" });
            }
        }

        [HttpGet("FindInBufferZone")]
        [Authorize(Roles = "Player")]
        public IActionResult FindInBufferZone([FromForm] BufferTurnForm bufferTurnForm)
        {
            try
            {
                var turns = turnsLogic.ListByBufferZone(bufferTurnForm.Longitude, bufferTurnForm.Latitude, bufferTurnForm.Radius);
                return Ok(turns);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "FindInBufferZone method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }
    }
}
