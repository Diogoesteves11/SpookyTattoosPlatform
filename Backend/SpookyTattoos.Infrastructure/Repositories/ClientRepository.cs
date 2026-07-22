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


using Microsoft.EntityFrameworkCore;
using SpookyTattoos.Domain.Entities;
using SpookyTattoos.Domain.Repositories;
using SpookyTattoos.Infrastructure.Persistence;

namespace SpookyTattoos.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly SpookyTattoosDbContext _dbContext;

    public ClientRepository(SpookyTattoosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == id);
    }

     public async Task<IEnumerable<Client>> SearchByFullNameAsync(string searchTerm)
    {
        return await _dbContext.Clients
            .Where(c => c.FullName != null && EF.Functions.ILike(c.FullName, $"%{searchTerm}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<Client>> SearchByEmailAsync(string searchTerm)
    {
        return await _dbContext.Clients
            .Where(c => c.Email != null && EF.Functions.ILike(c.Email, $"%{searchTerm}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<Client>> SearchByInstagramUserAsync(string searchTerm)
    {
        return await _dbContext.Clients
            .Where(c => c.InstagramUser != null && EF.Functions.ILike(c.InstagramUser, $"%{searchTerm}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _dbContext.Clients.ToListAsync();
    }

    public async Task<IEnumerable<Client>> GetTopClientsByGhostPointsAsync(int limit)
    {
        return await _dbContext.Clients
            .OrderByDescending(c => c.GhostPoints)
            .Take(limit)
            .ToListAsync();
    }


    public async Task<Client?> GetClientWithVouchersAsync(int id)
    {
        return await _dbContext.Clients 
            .Include(c => c.IssuedVouchers)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Client?> GetClientWithCouponsAsync(int id)
    {
        return await _dbContext.Clients
            .Include(c => c.Coupons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Client>> GetActiveAsync()
    {
        return await _dbContext.Clients
            .Where(c => c.Active == true)
            .OrderBy(c => c.FullName)
            .ToListAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbContext.Clients.AnyAsync(c => c.Email == email);
    }

    public async Task AddAsync(Client client)
    {
        await _dbContext.Clients.AddAsync(client);
    }

    public void Update(Client client)
    {
        _dbContext.Clients.Update(client);
    }

}