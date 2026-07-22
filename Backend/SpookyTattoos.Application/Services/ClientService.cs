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

using SpookyTattoos.Application.DTOs.Clients;
using SpookyTattoos.Application.DTOs.Promos;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Entities;
using SpookyTattoos.Domain.Repositories;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace SpookyTattoos.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IPromoRepository _promoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ClientService(
        IClientRepository clientRepository, 
        IPromoRepository promoRepository, 
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _promoRepository = promoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClientResponseDto> GetByIdAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client == null || !client.Active)
        {
            throw new NotFoundException("Client is not Active or doesn´t exist.");
        }

        return new ClientResponseDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            InstagramUser = client.InstagramUser,
            GhostPoints = client.GhostPoints,
            LastJob = client.LastJob,
            CreatedAt = client.CreatedAt,
            Active = client.Active
        };
    }

    public async Task<IEnumerable<ClientListDto>> GetAllAsync()
    {
        var clients = await _clientRepository.GetAllAsync();

        return clients.Select(c => new ClientListDto
        {
            Id = c.Id,
            FullName = c.FullName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            InstagramUser = c.InstagramUser,
            Active = c.Active
        });
    }

    public async Task<IEnumerable<ClientListDto>> GetActiveClientsAsync()
    {
        var activeClients = await _clientRepository.GetActiveAsync();

        return activeClients.Select(c => new ClientListDto
        {
            Id = c.Id,
            FullName = c.FullName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            InstagramUser = c.InstagramUser,
            Active = c.Active
        });
    }

    public async Task<IEnumerable<ClientListDto>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<ClientListDto>();
        }

        var searchByEmailTask = _clientRepository.SearchByEmailAsync(searchTerm);
        var searchByNameTask = _clientRepository.SearchByFullNameAsync(searchTerm);
        var searchByInstaTask = _clientRepository.SearchByInstagramUserAsync(searchTerm);

        await Task.WhenAll(searchByEmailTask, searchByNameTask, searchByInstaTask);

        var combinedResults = searchByEmailTask.Result
            .Concat(searchByNameTask.Result)
            .Concat(searchByInstaTask.Result);

        var uniqueClients = combinedResults.DistinctBy(c => c.Id);

        return uniqueClients.Select(c => new ClientListDto
        {
            Id = c.Id,
            FullName = c.FullName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            InstagramUser = c.InstagramUser,
            Active = c.Active
        });
    }

    public async Task<IEnumerable<ClientListDto>> GetTopClientsByGhostPointsAsync(int limit)
    {
        var topClients = await _clientRepository.GetTopClientsByGhostPointsAsync(limit);

        if (!topClients.Any())
        {
            return Enumerable.Empty<ClientListDto>();
        }

        return topClients.Select(c => new ClientListDto
        {
            Id = c.Id,
            FullName = c.FullName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            InstagramUser = c.InstagramUser,
            Active = c.Active
        });
    }

    public async Task<ClientWithVoucherDto> GetClientVouchersAsync(int id)
    {
        var client = await _clientRepository.GetClientWithVouchersAsync(id);

        if (client == null)
        {
            throw new NotFoundException("Client", id);
        }

        return new ClientWithVoucherDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email,
            GhostPoints = client.GhostPoints,
            IssuedVouchers = client.IssuedVouchers.Select(v => new VoucherDto
            {
                Id = v.Id,
                Value = v.Value,
                IsUsed = v.IsUsed,
                GeneratedAt = v.GeneratedAt,
                ExpiresAt = v.ExpiresAt
            }).ToList()
        };
    }

    public async Task<ClientWithCouponDto> GetClientCouponsAsync(int id)
    {
        var client = await _clientRepository.GetClientWithCouponsAsync(id);

        if (client == null)
        {
            throw new NotFoundException("Client", id);
        }

        return new ClientWithCouponDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email,
            GhostPoints = client.GhostPoints,
            Coupons = client.Coupons.Select(c => new CouponDto
            {
                Id = c.Id,
                PromoId = c.PromoId,
                IsUsed = c.IsUsed,
                AcquiredAt = c.AcquiredAt
            }).ToList()
        };
    }

    public async Task<ClientResponseDto> CreateAsync(CreateClientDto dto)
    {
        bool emailExists = await _clientRepository.ExistsByEmailAsync(dto.Email);
        if (emailExists)
        {
            throw new ConflictException($"Duplicate Email '{dto.Email}'.");
        }

        var newClient = new Client
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            InstagramUser = dto.InstagramUser,
        };

        await _clientRepository.AddAsync(newClient);
        await _unitOfWork.CommitAsync(); 

        return new ClientResponseDto
        {
            Id = newClient.Id,
            FullName = newClient.FullName,
            Email = newClient.Email,
            PhoneNumber = newClient.PhoneNumber,
            InstagramUser = newClient.InstagramUser,
            GhostPoints = newClient.GhostPoints,
            LastJob = newClient.LastJob,
            CreatedAt = newClient.CreatedAt,
            Active = newClient.Active
        };
    }

    public async Task<ClientResponseDto> UpdateAsync(int id, UpdateClientDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        
        if (client == null)
        {
            throw new NotFoundException("Client", id);
        }

        client.FullName = dto.FullName;
        client.PhoneNumber = dto.PhoneNumber;
        client.InstagramUser = dto.InstagramUser;
        
        if (dto.Active.HasValue)
        {
            client.Active = dto.Active.Value;
        }

        _clientRepository.Update(client);
        await _unitOfWork.CommitAsync(); 

        return new ClientResponseDto
        {
            Id = client.Id,
            FullName = client.FullName,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            InstagramUser = client.InstagramUser,
            GhostPoints = client.GhostPoints,
            LastJob = client.LastJob,
            CreatedAt = client.CreatedAt,
            Active = client.Active
        };
    }
    
    public async Task DeactivateAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        
        if (client == null)
        {
            throw new NotFoundException("Client", id);
        }

        client.Active = false;
        
        _clientRepository.Update(client);
        await _unitOfWork.CommitAsync(); 
    }

    public async Task<CouponDto> RedeemPromoAsync(int clientId, int promoId)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null)
        {
            throw new NotFoundException("Client", clientId);
        }

        var promo = await _promoRepository.GetByIdAsync(promoId);
        if (promo == null)
        {
            throw new NotFoundException("Promo", promoId);
        }

        try
        {
            var coupon = client.RedeemPromo(promo);
            
            _clientRepository.Update(client); 
            await _unitOfWork.CommitAsync(); 
            
            return new CouponDto
            {
                Id = coupon.Id,
                PromoId = coupon.PromoId,
                IsUsed = coupon.IsUsed,
                AcquiredAt = coupon.AcquiredAt
            };
        }
        catch (InvalidOperationException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<VoucherDto> IssueVoucherAsync(int clientId, decimal value)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null)
        {
            throw new NotFoundException("Client", clientId);
        }

        try
        {
            var voucher = client.IssueVoucher(value);
            
            _clientRepository.Update(client);
            await _unitOfWork.CommitAsync(); 
            
            return new VoucherDto
            {
                Id = voucher.Id,
                Value = voucher.Value,
                IsUsed = voucher.IsUsed,
                GeneratedAt = voucher.GeneratedAt,
                ExpiresAt = voucher.ExpiresAt
            };
        }
        catch (ArgumentException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }
}