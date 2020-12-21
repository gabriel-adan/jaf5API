using Logic.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class TeamsController : ApiBaseController
    {
        private readonly ITeamsLogic teamsLogic;

        public TeamsController(ILogger<ApiBaseController> logger, ITeamsLogic teamsLogic) : base(logger)
        {
            this.teamsLogic = teamsLogic;
        }

        [HttpPost("RequestJoinToTeam")]
        [Authorize(Roles = "Player")]
        public IActionResult RequestJoinToTeam([FromForm] PlayerJoinForm playerJoinForm)
        {
            try
            {
                if (teamsLogic.RequestJoinToTeam(playerJoinForm.TurnId, playerJoinForm.PerfilId))
                    return Ok(new { ResultMessage = "Solicitud de unión enviada" });
                else
                    return NotFound(new { ResultMessage = "No te pudiste unir al grupo" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RequestJoinToTeam method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }

        [HttpPost("ResponseJoinToTeam")]
        [Authorize(Roles = "Player")]
        public IActionResult ResponseJoinToTeam([FromForm] PlayerJoinForm playerJoinForm)
        {
            try
            {
                var turn = teamsLogic.ResponseJoinToTeam(playerJoinForm.TurnId, playerJoinForm.PlayerId, playerJoinForm.IsAccepted);
                if (turn != null)
                    return Ok(new { ResultMessage = string.Format("Tu grupo {0} acaba de completarse y tiene el turno confirmado para el día {1} a Hs {2} en {3}", turn.Name, turn.DateTime.ToString("dd/MM/yy"), turn.DateTime.ToString("HH:mm"), turn.Field) });
                else
                    return Ok(new { ResultMessage = "Nuevo integrante sumado a tu grupo!" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ResponseJoinToTeam method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }
    }
}
