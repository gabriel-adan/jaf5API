using Logic.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class CampsController : ApiBaseController
    {
        private readonly ICampsLogic campsLogic;

        public CampsController(ILogger<ApiBaseController> logger, ICampsLogic campsLogic) : base(logger)
        {
            this.campsLogic = campsLogic;
        }

        [HttpPost("FindInBufferZone")]
        [Authorize(Roles = "Player")]
        public IActionResult FindInBufferZone([FromForm] BufferForm bufferForm)
        {
            try
            {
                var camps = campsLogic.ListByBufferZone(bufferForm.Longitude, bufferForm.Latitude, bufferForm.Radius);
                return Ok(camps);
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
