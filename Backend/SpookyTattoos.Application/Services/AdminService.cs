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

using SpookyTattoos.Application.DTOs.Admins;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Entities;
using SpookyTattoos.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IAdminRepository adminRepository, IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminDto> GetByIdAsync(int id)
    {
        var admin = await _adminRepository.GetByIdAsync(id);

        if (admin == null)
        {
            throw new NotFoundException("Admin", id);
        }

        return new AdminDto
        {
            Id = admin.Id,
            Username = admin.Username,
            Email = admin.Email,
            Active = admin.Active,
            CreatedAt = admin.CreatedAt,
            LastLogin = admin.LastLogin
        };
    }

    public async Task<AdminDto> GetByUsernameAsync(string username)
    {
        var admin = await _adminRepository.GetByUserNameAsync(username);

        if (admin == null)
        {
            throw new NotFoundException($"Não foi encontrado nenhum Admin com o Username '{username}'.");
        }

        return new AdminDto
        {
            Id = admin.Id,
            Username = admin.Username,
            Email = admin.Email,
            Active = admin.Active,
            CreatedAt = admin.CreatedAt,
            LastLogin = admin.LastLogin
        };
    }

    public async Task<IEnumerable<AdminListDto>> GetActiveAdminsAsync()
    {
        var admins = await _adminRepository.GetActiveAsync();

        return admins.Select(a => new AdminListDto
        {
            Id = a.Id,
            Username = a.Username,
            Email = a.Email,
            Active = a.Active,
            LastLogin = a.LastLogin
        });
    }


    public async Task<IEnumerable<AdminListDto>> GetAllAsync()
    {
        var admins = await _adminRepository.GetAllAsync();

        return admins.Select(a => new AdminListDto
        {
            Id = a.Id,
            Username = a.Username,
            Email = a.Email,
            Active = a.Active,
            LastLogin = a.LastLogin
        });
    }

    public async Task<IEnumerable<AdminListDto>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<AdminListDto>();
        }

        var searchByEmailTask = _adminRepository.SearchByEmailAsync(searchTerm);
        var searchByUsernameTask = _adminRepository.SearchByUsernameAsync(searchTerm);

        await Task.WhenAll(searchByEmailTask, searchByUsernameTask);

        var combinedResults = searchByEmailTask.Result
            .Concat(searchByUsernameTask.Result)
            .DistinctBy(a => a.Id);

        return combinedResults.Select(a => new AdminListDto
        {
            Id = a.Id,
            Username = a.Username,
            Email = a.Email,
            Active = a.Active,
            LastLogin = a.LastLogin
        });
    }

    public async Task CreateAsync(CreateAdminDto dto)
    {
        var emailExists = await _adminRepository.GetByEmailAsync(dto.Email);
        if (emailExists != null)
        {
            throw new ConflictException($"Já existe uma conta associada ao email '{dto.Email}'.");
        }

        var usernameExists = await _adminRepository.GetByUserNameAsync(dto.Username);
        if (usernameExists != null)
        {
            throw new ConflictException($"O username '{dto.Username}' já está em uso.");
        }

        var newAdmin = new Admin
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), 
            Active = true,
            CreatedAt = DateTimeOffset.UtcNow,
            LastLogin = DateTimeOffset.UtcNow
        };

        await _adminRepository.AddAsync(newAdmin);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(int id, UpdateAdminDto dto)
    {
        var admin = await _adminRepository.GetByIdAsync(id);
        
        if (admin == null)
        {
            throw new NotFoundException("Admin", id);
        }

        if (admin.Email != dto.Email)
        {
            var emailExists = await _adminRepository.GetByEmailAsync(dto.Email);
            if (emailExists != null)
            {
                throw new ConflictException($"Já existe uma conta associada ao email '{dto.Email}'.");
            }
        }

        if (admin.Username != dto.Username)
        {
            var usernameExists = await _adminRepository.GetByUserNameAsync(dto.Username);
            if (usernameExists != null)
            {
                throw new ConflictException($"O username '{dto.Username}' já está em uso.");
            }
        }

        admin.Username = dto.Username;
        admin.Email = dto.Email;
        admin.Active = dto.Active;

        _adminRepository.Update(admin);
        await _unitOfWork.CommitAsync();
    }
}