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

public class CouponRepository : ICouponRepository
{
    private readonly SpookyTattoosDbContext _dbContext;

    public CouponRepository(SpookyTattoosDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Coupon?> GetByIdAsync(int id)
    {
        return await _dbContext.Coupons
            .Include(c => c.Promo) 
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Coupon?> GetByClientIdAsync(int clientId)
    {
        return await _dbContext.Coupons
            .Include(c => c.Promo) 
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);
    }    

    public async Task<IEnumerable<Coupon>> GetByPromoIdAsync(int promoId)
    {
        return await _dbContext.Coupons
            .Where(c => c.PromoId == promoId)
            .ToListAsync();
    }

    public async Task AddAsync(Coupon coupon)
    {
        await _dbContext.Coupons.AddAsync(coupon);
    }

    public void Update(Coupon coupon)
    {
        _dbContext.Coupons.Update(coupon);
    }

    
}