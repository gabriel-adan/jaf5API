using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Logic.Contracts;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class HoursController : ApiBaseController
    {
        private readonly IHoursLogic hoursLogic;

        public HoursController(ILogger<ApiBaseController> logger, IHoursLogic hoursLogic) : base(logger)
        {
            this.hoursLogic = hoursLogic;
        }

        [HttpGet("GetList/{campId}")]
        [Authorize(Roles = "Owner, Employee")]
        public IActionResult GetList(int campId)
        {
            try
            {
                return Ok(hoursLogic.List(campId));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al listar horarios");
                return NotFound(new { ErrorMessage = "Ocurrió un error al listar los horarios" });
            }
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Owner, EmployeeHourCreate")]
        public IActionResult Create([FromForm] HourForm hourForm)
        {
            try
            {
                return Ok(hoursLogic.Create(hourForm.Time, hourForm.DayOfWeek, hourForm.IsEnabled, hourForm.CampId));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al registrar horario");
                return NotFound(new { ErrorMessage = "Ocurrió un error al intentar registrar el horario" });
            }
        }

        [HttpPost("ChangeState")]
        [Authorize(Roles = "Owner, EmployeeHourEdit")]
        public IActionResult ChangeState([FromForm] HourForm hourForm)
        {
            try
            {
                hoursLogic.EnableDisable(hourForm.Id, hourForm.IsEnabled);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, string.Format("Error al {0} el horario Id = {1}", hourForm.IsEnabled ? "habilitar" : "deshabilitar", hourForm.Id));
                return NotFound(new { ErrorMessage = "Ocurrió un error" });
            }
        }
    }
}
