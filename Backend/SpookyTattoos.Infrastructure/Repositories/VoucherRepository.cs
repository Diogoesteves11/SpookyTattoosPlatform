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

public class VoucherRepository : IVoucherRepository
{
    private readonly SpookyTattoosDbContext _dbContext;

    public VoucherRepository(SpookyTattoosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Voucher?> GetByIdAsync(int id)
    {
        return await _dbContext.Vouchers
            .Include(v => v.Emitter)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Voucher>> GetByEmitterIdAsync(int emitterId)
    {
        return await _dbContext.Vouchers
            .Where(v => v.EmitterId == emitterId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Voucher>> GetValidVouchersAsync()
    {
        var now = DateTimeOffset.UtcNow;
        return await _dbContext.Vouchers
            .Where(v => !v.IsUsed && v.ExpiresAt > now)
            .OrderBy(v => v.ExpiresAt)
            .ToListAsync();
    }

    public async Task AddAsync(Voucher voucher)
    {
        await _dbContext.Vouchers.AddAsync(voucher);
    }

    public void Update(Voucher voucher)
    {
        _dbContext.Vouchers.Update(voucher);
    }
}