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

public class AdminRepository : IAdminRepository
{
    private readonly SpookyTattoosDbContext _dbContext;

    public AdminRepository(SpookyTattoosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Admin?> GetByIdAsync(int id)
    {
        return await _dbContext.Admins.FindAsync(id);
    }

    public async Task<Admin?> GetByUserNameAsync(string username)
    {
        return await _dbContext.Admins.FirstOrDefaultAsync(a => a.Username == username);
    }

    public async Task<IEnumerable<Admin>> SearchByUsernameAsync(string searchTerm)
    {
        return await _dbContext.Admins
            .Where(a => a.Username != null && EF.Functions.ILike(a.Username, $"%{searchTerm}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<Admin>> SearchByEmailAsync(string searchTerm)
    {
        return await _dbContext.Admins
            .Where(a => a.Email != null && EF.Functions.ILike(a.Email, $"%{searchTerm}%"))
            .ToListAsync();
    }
    public async Task<IEnumerable<Admin>> GetAllAsync()
    {
        return await _dbContext.Admins.OrderBy(a => a.Username).ToListAsync();
    }

    public async Task<IEnumerable<Admin>> GetActiveAsync()
    {
        return await _dbContext.Admins.Where(a => a.Active == true).OrderBy(a => a.Username).ToListAsync();
    }

    public async Task<Admin?> GetByEmailAsync(string email)
    {
        return await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task AddAsync(Admin admin)
    {
        await _dbContext.Admins.AddAsync(admin);
    }

    public void Update(Admin admin)
    {
        _dbContext.Admins.Update(admin);
    }
}