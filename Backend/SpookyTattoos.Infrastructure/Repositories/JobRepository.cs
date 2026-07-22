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

public class JobRepository : IJobRepository
{
    private readonly SpookyTattoosDbContext _dbContext;

    public JobRepository(SpookyTattoosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Job?> GetByIdAsync(int id)
    {
        return await _dbContext.Jobs.FindAsync(id);
    }

    public async Task<Job?> GetJobWithDetailsAsync(int id)
    {
        return await _dbContext.Jobs
            .Include(j => j.Client)
            .Include(j => j.Tattoos)
            .Include(j => j.Piercings)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IEnumerable<Job>> GetJobsByClientIdAsync(int clientId)
    {
        return await _dbContext.Jobs
            .Where(j => j.ClientId == clientId)
            .OrderByDescending(j => j.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetActiveJobsAsync()
    {
        return await _dbContext.Jobs
            .Where(j => j.Status == JobStatus.AGENDADO || j.Status == JobStatus.EXECUCAO)
            .OrderBy(j => j.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetJobsByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _dbContext.Jobs
            .Include(j => j.Client) 
            .Where(j => j.ScheduledDate >= startDate && j.ScheduledDate <= endDate)
            .OrderBy(j => j.ScheduledDate)
            .ToListAsync();
    }

    public async Task AddAsync(Job job)
    {
        await _dbContext.Jobs.AddAsync(job);
    }

    public void Update(Job job)
    {
        _dbContext.Jobs.Update(job);
    }
}