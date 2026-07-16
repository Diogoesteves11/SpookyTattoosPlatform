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

public class PromoRepository : IPromoRepository
{
    private readonly SpookyTattoosDbContext _dbContext;

    public PromoRepository(SpookyTattoosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Promo?> GetByIdAsync(int id)
    {
        return await _dbContext.Promos.FindAsync(id);
    }

    public async Task<IEnumerable<Promo>> GetAllAsync()
    {
        return await _dbContext.Promos.ToListAsync();
    }

    public async Task<IEnumerable<Promo>> GetActivePromosAsync()
    {
        var now = DateTimeOffset.UtcNow;
        return await _dbContext.Promos
            .Where(p => (!p.StartDate.HasValue || p.StartDate <= now) && 
                        (!p.EndDate.HasValue || p.EndDate >= now))
            .ToListAsync();
    }

    public async Task AddAsync(Promo promo)
    {
        await _dbContext.Promos.AddAsync(promo);
    }

    public void Update(Promo promo)
    {
        _dbContext.Promos.Update(promo);
    }

    public void Delete(Promo promo)
    {
        _dbContext.Promos.Remove(promo);
    }
}