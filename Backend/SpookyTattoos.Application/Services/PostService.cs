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

using SpookyTattoos.Application.DTOs.Posts;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Repositories;
using SpookyTattoos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PostService(IPostRepository postRepository, IJobRepository jobRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostDto> GetByIdAsync(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);

        if (post == null)
        {
            throw new NotFoundException("Post", id);
        }

        return MapToDto(post);
    }

    public async Task<PostDto> GetByJobIdAsync(int jobId)
    {
        var post = await _postRepository.GetByJobIdAsync(jobId);

        if (post == null)
        {
            throw new NotFoundException($"Não foi encontrado nenhum Post para o Job ID {jobId}.");
        }

        return MapToDto(post);
    }

    public async Task<IEnumerable<PostListDto>> GetPublishedPostsAsync()
    {
        var posts = await _postRepository.GetPublishedPostsAsync();

        return posts.Select(MapToListDto);
    }

    public async Task<IEnumerable<PostListDto>> GetByCreationDateAsync(DateTimeOffset createdAt)
    {
        var posts = await _postRepository.GetByCreationDateAsync(createdAt);

        return posts.Select(MapToListDto);
    }

    public async Task CreateAsync(CreatePostDto dto)
    {
        var jobExists = await _jobRepository.GetByIdAsync(dto.JobId);
        if (jobExists == null)
        {
            throw new NotFoundException("Job", dto.JobId);
        }

        try
        {
            var newPost = new Post
            {
                JobId = dto.JobId,
                Description = dto.Description,
                PostText = dto.PostText,
                IsPublished = false, 
                CreatedAt = DateTimeOffset.UtcNow,
                Images = dto.Images.Select(img => new PostImage
                {
                    ImageUrl = img.ImageUrl,
                    DisplayOrder = img.DisplayOrder
                }).ToList()
            };

            await _postRepository.AddAsync(newPost);
            await _unitOfWork.CommitAsync();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task UpdateAsync(int id, UpdatePostDto dto)
    {
        var post = await _postRepository.GetByIdAsync(id);
        
        if (post == null)
        {
            throw new NotFoundException("Post", id);
        }

        post.Description = dto.Description;
        post.PostText = dto.PostText;
        post.IsPublished = dto.IsPublished;

        _postRepository.Update(post);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        
        if (post == null)
        {
            throw new NotFoundException("Post", id);
        }

        _postRepository.Delete(post);
        await _unitOfWork.CommitAsync();
    }

    private PostDto MapToDto(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            JobId = post.JobId,
            Description = post.Description,
            PostText = post.PostText,
            CreatedAt = post.CreatedAt,
            IsPublished = post.IsPublished,
            Images = post.Images.OrderBy(img => img.DisplayOrder).Select(img => new PostImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl,
                DisplayOrder = img.DisplayOrder
            })
        };
    }

    private PostListDto MapToListDto(Post post)
    {
        return new PostListDto
        {
            Id = post.Id,
            JobId = post.JobId,
            Description = post.Description,
            CreatedAt = post.CreatedAt,
            IsPublished = post.IsPublished,
            CoverImageUrl = post.Images.OrderBy(img => img.DisplayOrder).FirstOrDefault()?.ImageUrl
        };
    }
}