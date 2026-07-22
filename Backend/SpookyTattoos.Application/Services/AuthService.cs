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

using Microsoft.Extensions.Caching.Memory;
using SpookyTattoos.Application.DTOs.Auth;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.External;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Repositories;

namespace SpookyTattoos.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAdminRepository _adminRepository;
    private readonly ITokenService _tokenService;
    private readonly IMemoryCache _cache;

    public AuthService(
        IAdminRepository adminRepository, 
        ITokenService tokenService, 
        IMemoryCache cache)
    {
        _adminRepository = adminRepository;
        _tokenService = tokenService;
        _cache = cache;
    }
    public async Task<(LoginResponseDto Admin, string Token)> LoginAdminAsync(LoginRequestDto request)
    {
        var admin = await _adminRepository.GetByUserNameAsync(request.Username);

        if (admin == null || !admin.Active)
        {
            throw new BadRequestException("Invalid Credentials.");
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, admin.Password);

        if (!isPasswordValid)
        {
            throw new BadRequestException("Invalid Credentials.");
        }

        var token = _tokenService.GenerateToken(admin);

        var responseDto = new LoginResponseDto
        {
            Id = admin.Id,
            Username = admin.Username,
        };

        return (responseDto, token);
    }

    public Task LogoutAsync(string jti, TimeSpan remainingTime)
    {
        if (remainingTime <= TimeSpan.Zero)
        {
            return Task.CompletedTask;
        }
        
        _cache.Set(
            $"revoked_token_{jti}", 
            true,                  
            remainingTime        
        );

        return Task.CompletedTask;
    }

    public Task<bool> IsTokenRevokedAsync(string jti)
    {
        bool isRevoked = _cache.TryGetValue($"revoked_token_{jti}", out _);
        
        return Task.FromResult(isRevoked);
    }
}