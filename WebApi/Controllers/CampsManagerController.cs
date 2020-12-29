using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Logic.Contracts;
using WebApi.Models;
using Domain.Dtos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    public class CampsManagerController : ApiBaseController
    {
        private readonly ICampsLogic campsLogic;
        private readonly IList<string> AbcFieldNames;

        public CampsManagerController(ILogger<ApiBaseController> logger, ICampsLogic campsLogic, IConfiguration configuration) : base(logger)
        {
            this.campsLogic = campsLogic;
            string fieldNames = configuration.GetValue<string>("AbcFieldNames");
            AbcFieldNames = fieldNames.Split(',');
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Owner")]
        public IActionResult Create([FromForm] CreateCampForm createCampForm)
        {
            try
            {
                CampDto campDto = campsLogic.Create(createCampForm.CustomerId, createCampForm.Name, createCampForm.Street, createCampForm.Number, createCampForm.Longitude, createCampForm.Latitude, AbcFieldNames, createCampForm.FieldCount);
                return Ok(campDto);
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

        [HttpGet("Fields/{id}")]
        [Authorize(Roles = "Owner, Employee")]
        public IActionResult Fields(int id)
        {
            try
            {
                var fields = campsLogic.GetFields(id);
                return Ok(fields);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fields method");
                return NotFound(new { ErrorMessage = "Ocurrió un error" });
            }
        }

        [HttpPost("FieldEdit")]
        [Authorize(Roles = "Owner, EmployeeFieldEdit")]
        public IActionResult FieldEdit([FromForm] FieldForm fieldForm)
        {
            try
            {
                campsLogic.EditFieldState(fieldForm.FieldId, fieldForm.CampId, fieldForm.IsEnabled);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "FieldEdit method");
                return NotFound(new { ErrorMessage = "Ocurrió un error" });
            }
        }
    }
}
