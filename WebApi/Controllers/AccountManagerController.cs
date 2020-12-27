using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Authentication.Token.Provider;
using WebApi.Models;
using Domain;
using Domain.RepositoryInterfaces;
using Logic;

namespace WebApi.Controllers
{
    public class AccountManagerController : ApiBaseController
    {
        private readonly IAuthenticationTokenProvider authenticationTokenProvider;
        private readonly IPerfilsRepository perfilsRepository;
        private readonly ICustomersRepository customersRepository;
        private readonly Random RandomCodeGenerator;
        private readonly int MinValueVerifyCode;
        private readonly int MaxValueVerifyCode;

        public AccountManagerController(ILogger<ApiBaseController> logger, IAuthenticationTokenProvider authenticationTokenProvider, IPerfilsRepository perfilsRepository, ICustomersRepository customersRepository, IConfiguration configuration) : base(logger)
        {
            this.authenticationTokenProvider = authenticationTokenProvider;
            this.perfilsRepository = perfilsRepository;
            this.customersRepository = customersRepository;
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

        [HttpPost("PlayerSigIn")]
        public IActionResult PlayerSigIn([FromForm] PlayerSigInForm playerSigInForm)
        {
            try
            {
                if (string.IsNullOrEmpty(playerSigInForm.Email))
                    throw new ArgumentException("Debe ingresar un correo electrónico");
                Perfil perfil = perfilsRepository.Exists(playerSigInForm.Email);
                if (perfil != null)
                    if (authenticationTokenProvider.IsEnabledAccount(playerSigInForm.Email, EAuthenticationField.EMAIL))
                        throw new ArgumentException("Ya existe una cuenta activa para este correo electrónico");
                    else
                        throw new ArgumentException("Ya existe una cuenta asociada a este correo electrónico pero aún no está validada, verifique la casilla de correo para seguir los pasos de validación");

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
                logger.LogError(ex, "PlayerSigIn method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }

        [HttpPost("PlayerAccountConfirm")]
        public IActionResult PlayerAccountConfirm([FromForm] UserForm userForm)
        {
            try
            {
                Helper.ThrowIfIsNullOrEmpty(userForm.UserName, "Usuario inválido");
                Helper.ThrowIfIsNullOrEmpty(userForm.VerifyCode, "Código de verificación inválido");
                if (authenticationTokenProvider.ConfirmAccount(userForm.UserName, userForm.VerifyCode, EAuthenticationField.EMAIL))
                    return Ok(new { ResultMessage = "Tu cuenta fue validada exitosamente" });
                else
                    return NotFound(new { ErrorMessage = "No se pudo validar la cuenta" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PlayerAccountConfirm method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }

        [HttpPost("CustomerSigIn")]
        public IActionResult CustomerSigIn([FromForm] CustomerSigInForm customerSigInForm)
        {
            try
            {
                Helper.ThrowIfIsNullOrEmpty(customerSigInForm.Email, "Debe ingresar un correo electrónico");
                Customer customer = customersRepository.Exists(customerSigInForm.Email);
                if (customer != null)
                    if (authenticationTokenProvider.IsEnabledAccount(customerSigInForm.Email, EAuthenticationField.EMAIL))
                        throw new ArgumentException("Ya existe una cuenta activa para este correo electrónico");
                    else
                        throw new ArgumentException("Ya existe una cuenta asociada a este correo electrónico pero aún no está validada, verifique la casilla de correo para seguir los pasos de validación");

                if (authenticationTokenProvider.SigIn(customerSigInForm.FirstName, customerSigInForm.LastName, customerSigInForm.Password, null, customerSigInForm.Email, false, EAuthenticationField.EMAIL, new List<string>() { "Owner" }))
                {
                    customer = new Customer();
                    customer.FirstName = customerSigInForm.FirstName;
                    customer.LastName = customerSigInForm.LastName;
                    customer.Email = customerSigInForm.Email;
                    customersRepository.Save(customer);

                    //NOTIFICAR POR MAIL PARA LA CONFIRMACIÓN DE CUENTA.

                    return Ok(new { ResultMessage = string.Format("Cuenta creada exitosamente. Por favor verifique su casilla de correo {0} para validar su cuenta.", customerSigInForm.Email) });
                }
                else
                {
                    return NotFound(new { ErrorMessage = "No se pudieron registrar los datos." });
                }
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CustomerSigIn method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }

        [HttpPost("CustomerAccountConfirm")]
        public IActionResult CustomerAccountConfirm([FromForm] UserForm userForm)
        {
            try
            {
                Helper.ThrowIfIsNullOrEmpty(userForm.UserName, "Usuario inválido");
                Helper.ThrowIfIsNullOrEmpty(userForm.VerifyCode, "Código de verificación inválido");
                if (authenticationTokenProvider.ConfirmAccount(userForm.UserName, userForm.VerifyCode, EAuthenticationField.EMAIL))
                    return Ok(new { ResultMessage = "La cuenta fue validada exitosamente" });
                else
                    return NotFound(new { ErrorMessage = "No se pudo validar la cuenta" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CustomerAccountConfirm method");
                return NotFound(new { ErrorMessage = "Ocurrió un error." });
            }
        }
    }
}
