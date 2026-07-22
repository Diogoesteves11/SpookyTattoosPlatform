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

using SpookyTattoos.Domain.Entities;

namespace SpookyTattoos.Domain.Repositories;


public interface IPostRepository
{
    Task<Post?> GetByIdAsync(int id);
    Task<IEnumerable<Post>> GetPublishedPostsAsync();
    Task<IEnumerable<Post>> GetByCreationDateAsync(DateTimeOffset createdAt);

    Task<Post?> GetByJobIdAsync(int id);

    Task AddAsync(Post post);

    void Update(Post post);

    void Delete(Post post);

}