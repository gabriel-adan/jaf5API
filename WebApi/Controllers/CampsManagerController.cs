using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Logic.Contracts;
using WebApi.Models;
using Domain.Dtos;

namespace WebApi.Controllers
{
    public class CampsManagerController : ApiBaseController
    {
        private readonly ICampsLogic campsLogic;

        public CampsManagerController(ILogger<ApiBaseController> logger, ICampsLogic campsLogic) : base(logger)
        {
            this.campsLogic = campsLogic;
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Owner")]
        public IActionResult Create([FromForm] CreateCampForm createCampForm)
        {
            try
            {
                CampAccountDto campAccountDto = campsLogic.Create(createCampForm.CustomerId, createCampForm.Name, createCampForm.Street, createCampForm.Number, createCampForm.Longitude, createCampForm.Latitude);
                return Ok(campAccountDto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Create method");
                return NotFound(new { ErrorMessage = "Ocurrió un error" });
            }
        }
    }
}
