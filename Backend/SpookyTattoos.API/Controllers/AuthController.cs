/*
Copyright 2026 Diogo Esteves, Guilherme Mattos

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt; 
using SpookyTattoos.Application.DTOs.Auth;
using SpookyTattoos.Application.Interfaces.Services;

namespace SpookyTattoos.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous] 
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var (adminInfo, token) = await _authService.LoginAdminAsync(request);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,  
                Secure = true,   
                SameSite = SameSiteMode.Strict, 
                Expires = DateTime.UtcNow.AddHours(5) 
            };

            Response.Cookies.Append("jwt", token, cookieOptions);

            return Ok(adminInfo);
        }
        catch (SpookyTattoos.Application.Exceptions.UnauthorizedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (SpookyTattoos.Application.Exceptions.NotFoundException ex)
        {
            return Unauthorized(new { message = "Credenciais inválidas.", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro interno ao processar o login.", error = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Authorize] 
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Append("jwt", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1)
        });

        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var expClaim = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

        if (!string.IsNullOrEmpty(jti) && long.TryParse(expClaim, out long expSeconds))
        {
            var expireDate = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
            
            var remainingTime = expireDate - DateTime.UtcNow;

            if (remainingTime > TimeSpan.Zero)
            {
                await _authService.LogoutAsync(jti, remainingTime);
            }
        }

        return Ok(new { message = "Logout Done!" });
    }
}