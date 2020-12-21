using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Authentication.Token.Provider;
using WebApi.Models;
using Domain;
using Domain.RepositoryInterfaces;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    public class AccountManagerController : ApiBaseController
    {
        private readonly IAuthenticationTokenProvider authenticationTokenProvider;
        private readonly IPerfilsRepository perfilsRepository;
        private readonly Random RandomCodeGenerator;
        private readonly int MinValueVerifyCode;
        private readonly int MaxValueVerifyCode;

        public AccountManagerController(ILogger<ApiBaseController> logger, IAuthenticationTokenProvider authenticationTokenProvider, IPerfilsRepository perfilsRepository, IConfiguration configuration) : base(logger)
        {
            this.authenticationTokenProvider = authenticationTokenProvider;
            this.perfilsRepository = perfilsRepository;
            RandomCodeGenerator = new Random();
            MinValueVerifyCode = configuration.GetValue<int>("MinValueVerifyCode");
            MaxValueVerifyCode = configuration.GetValue<int>("MaxValueVerifyCode");
        }

        [HttpPost("LogIn")]
        public IActionResult LogIn([FromForm] UserForm userForm)
        {
            try
            {
                string token = authenticationTokenProvider.LogIn(userForm.UserName, userForm.Password, EAuthenticationField.EMAIL);
                if (!string.IsNullOrEmpty(token))
                    return Ok(new { AccessToken = token });
                else
                    return NotFound(new { ErrorMessage = "Usuario o contraseña inválidos" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogIn method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }

        [HttpPost("SigIn")]
        public IActionResult SigIn([FromForm] PlayerSigInForm playerSigInForm)
        {
            try
            {
                if (string.IsNullOrEmpty(playerSigInForm.Email))
                    throw new ArgumentException("Debe ingresar un correo electrónico");
                Perfil perfil = perfilsRepository.Exists(playerSigInForm.Email);
                if (perfil != null)
                    throw new ArgumentException("Ya existe una cuenta asociada a éste mail");
                
                int verifyCode = RandomCodeGenerator.Next(MinValueVerifyCode, MaxValueVerifyCode);
                if (authenticationTokenProvider.SigIn(playerSigInForm.FirstName, playerSigInForm.LastName, playerSigInForm.Password, null, playerSigInForm.Email, false, EAuthenticationField.EMAIL, new List<string>() { "Player" }, verifyCode.ToString()))
                {
                    perfil = new Perfil();
                    perfil.Name = playerSigInForm.FirstName + " " + playerSigInForm.LastName;
                    perfil.Email = playerSigInForm.Email;
                    perfilsRepository.Save(perfil);

                    //NOTIFICAR POR MAIL LA CONFIRMACIÓN DE CUENTA AL NUEVO USUARIO.
                    
                    return Ok(new { ResultMessage = string.Format("Cuenta creada exitosamente. Por favor verifica tu casilla de correo {0} para validar tu cuenta.", playerSigInForm.Email) });
                }
                else
                {
                    return NotFound(new { ErrorMessage = "No se pudo crear la cuenta" });
                }
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SigIn method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }

        [HttpPost("ConfirmAccount")]
        public IActionResult ConfirmAccount([FromForm] PlayerConfirmAccountForm playerConfirmAccountForm)
        {
            try
            {
                if (string.IsNullOrEmpty(playerConfirmAccountForm.UserName))
                    throw new ArgumentException("Usuario inválido");
                if (string.IsNullOrEmpty(playerConfirmAccountForm.VerifyCode))
                    throw new ArgumentException("Código de verificación inválido");
                if (authenticationTokenProvider.ConfirmAccount(playerConfirmAccountForm.UserName, playerConfirmAccountForm.VerifyCode, EAuthenticationField.EMAIL))
                {
                    return Ok(new { ResultMessage = "Tu cuenta fue validada exitosamente" });
                }
                else
                {
                    return NotFound(new { ErrorMessage = "No se pudo validar la cuenta" });
                }
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ConfirmAccount method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }
    }
}
