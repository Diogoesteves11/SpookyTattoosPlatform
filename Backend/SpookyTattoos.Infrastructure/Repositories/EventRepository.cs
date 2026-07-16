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


public class EventRepository : IEventRepository
{
    private readonly SpookyTattoosDbContext _dbContext;

    public EventRepository(SpookyTattoosDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _dbContext.Events
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Event>> SearchByNameAsync(string searchTerm)
    {
        return await _dbContext.Events
            .Where(e => e.EventName != null && EF.Functions.ILike(e.EventName, $"%{searchTerm}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetByStartDateAsync(DateTimeOffset startDate)
    {
        return await _dbContext.Events
            .Where(c => c.StartDate == startDate)
            .OrderBy(c => c.EventName)
            .ToListAsync();
    }

    public async Task AddAsync(Event spookyEvent)
    {
        await _dbContext.Events.AddAsync(spookyEvent);
    }

    public void Update(Event spookyEvent)
    {
        _dbContext.Events.Update(spookyEvent);
    }
    
    public void Delete(Event spookyEvent)
    {
        _dbContext.Events.Remove(spookyEvent);
    }

}