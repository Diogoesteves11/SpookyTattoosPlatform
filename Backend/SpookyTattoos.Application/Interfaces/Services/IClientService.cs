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

namespace SpookyTattoos.Application.Interfaces.Services;

public interface IClientService
{
    
    /// <exception cref="NotFoundException">Lauched if the client doesn´t exist</exception>
    Task<ClientResponseDto> GetByIdAsync(int id);
    
    Task<IEnumerable<ClientListDto>> GetAllAsync();
    
    Task<IEnumerable<ClientListDto>> GetActiveClientsAsync();
    
    /// <summary>
    /// Aggregates searches by email, name and instagram user
    /// </summary>
    Task<IEnumerable<ClientListDto>> SearchAsync(string searchTerm);
    
    Task<IEnumerable<ClientListDto>> GetTopClientsByGhostPointsAsync(int limit);

    Task<ClientWithVoucherDto> GetClientVouchersAsync(int id);

    Task<ClientWithCouponDto> GetClientCouponsAsync(int id);

    
    /// <exception cref="ConflictException">Lauched if there is a client with the same email</exception>
    Task<ClientResponseDto> CreateAsync(CreateClientDto dto);
    
    /// <exception cref="NotFoundException">Lauched if the client to update doesn´t exist</exception>
    Task<ClientResponseDto> UpdateAsync(int id, UpdateClientDto dto);
    
    /// <summary>
    /// Deactivates the client with soft delete (Active = false)
    /// </summary>
    /// <exception cref="NotFoundException">Launched if the client doesn´t exist</exception>
    Task DeactivateAsync(int id);

    
    /// <summary>
    /// Generates the promo cuppon and decreases the client´s ghost points
    /// </summary>
    /// <exception cref="NotFoundException">Launched if the client or promo doesn´t exist</exception>
    /// <exception cref="BadRequestException">Launched if there is not enough ghost points</exception>
    Task<CouponDto> RedeemPromoAsync(int clientId, int promoId);

    /// <summary>
    /// Creates a new voucher 
    /// </summary>
     /// <exception cref="NotFoundException">Launched if the client doesn´t exist</exception>
    Task<VoucherDto> IssueVoucherAsync(int clientId, decimal value);
}