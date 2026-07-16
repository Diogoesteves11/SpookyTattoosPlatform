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

public class TattooRepository : ITattooRepository
{
    private readonly SpookyTattoosDbContext _dbContext;

    public TattooRepository(SpookyTattoosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tattoo?> GetByIdAsync(int id)
    {
        return await _dbContext.Tattoos
            .Include(t => t.Job)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tattoo>> GetByJobIdAsync(int jobId)
    {
        return await _dbContext.Tattoos
            .Include(t => t.Job)
            .Where(t => t.JobId == jobId)
            .ToListAsync();
    }

    public async Task AddAsync(Tattoo tattoo)
    {
        await _dbContext.Tattoos.AddAsync(tattoo);
    }

    public void Update(Tattoo tattoo)
    {
        _dbContext.Tattoos.Update(tattoo);
    }
}