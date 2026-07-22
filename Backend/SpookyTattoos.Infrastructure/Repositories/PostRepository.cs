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

public class PostRepository : IPostRepository
{
    private readonly SpookyTattoosDbContext _dbContext;

    public PostRepository(SpookyTattoosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _dbContext.Posts
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
    {
        return await _dbContext.Posts
            .Where(p => p.IsPublished == true)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByCreationDateAsync(DateTimeOffset createdAt)
    {
        return await _dbContext.Posts
            .Where(p => p.CreatedAt == createdAt)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();  
   }

   public async Task<Post?> GetByJobIdAsync(int id)
   {
        return await _dbContext.Posts
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.JobId == id);
   }

   public async Task AddAsync(Post post)
   {
        await _dbContext.AddAsync(post);
   }

   public void Update(Post post)
   {
        _dbContext.Update(post);
   }

   public void Delete(Post post)
    {
        _dbContext.Posts.Remove(post); 
    }
}