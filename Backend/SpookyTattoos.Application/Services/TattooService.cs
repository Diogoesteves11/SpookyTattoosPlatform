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

using SpookyTattoos.Application.DTOs.Tattoos;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Repositories;
using SpookyTattoos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class TattooService : ITattooService
{
    private readonly ITattooRepository _tattooRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TattooService(ITattooRepository tattooRepository, IJobRepository jobRepository, IUnitOfWork unitOfWork)
    {
        _tattooRepository = tattooRepository;
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TattooDto> GetByIdAsync(int id)
    {
        var tattoo = await _tattooRepository.GetByIdAsync(id);

        if (tattoo == null)
        {
            throw new NotFoundException("Tattoo", id);
        }

        return MapToDto(tattoo);
    }

    public async Task<IEnumerable<TattooDto>> GetByJobIdAsync(int jobId)
    {
        var tattoos = await _tattooRepository.GetByJobIdAsync(jobId);

        return tattoos.Select(MapToDto);
    }

    public async Task CreateAsync(CreateTattooDto dto)
    {
        var jobExists = await _jobRepository.GetByIdAsync(dto.JobId);
        if (jobExists == null)
        {
            throw new NotFoundException("Job", dto.JobId);
        }

        try
        {
            var newTattoo = new Tattoo
            {
                JobId = dto.JobId,
                SizeCm = dto.SizeCm,
                name = dto.Name,
                Style = dto.Style,
                HasColor = dto.HasColor,
                FillScore = dto.FillScore,
                ShadowScore = dto.ShadowScore,
                DetailScore = dto.DetailScore,
                BodyZoneScore = dto.BodyZoneScore,
                FinalTattooPrice = 0 
            };

            await _tattooRepository.AddAsync(newTattoo);
            await _unitOfWork.CommitAsync();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task UpdateAsync(int id, UpdateTattooDto dto)
    {
        var tattoo = await _tattooRepository.GetByIdAsync(id);
        
        if (tattoo == null)
        {
            throw new NotFoundException("Tattoo", id);
        }

        try
        {
            tattoo.SizeCm = dto.SizeCm;
            tattoo.name = dto.Name;
            tattoo.Style = dto.Style;
            tattoo.HasColor = dto.HasColor;
            tattoo.FillScore = dto.FillScore;
            tattoo.ShadowScore = dto.ShadowScore;
            tattoo.DetailScore = dto.DetailScore;
            tattoo.BodyZoneScore = dto.BodyZoneScore;
            tattoo.FinalTattooPrice = dto.FinalTattooPrice;

            _tattooRepository.Update(tattoo);
            await _unitOfWork.CommitAsync();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var tattoo = await _tattooRepository.GetByIdAsync(id);
        
        if (tattoo == null)
        {
            throw new NotFoundException("Tattoo", id);
        }

        _tattooRepository.Delete(tattoo);
        await _unitOfWork.CommitAsync();
    }

    private TattooDto MapToDto(Tattoo tattoo)
    {
        return new TattooDto
        {
            Id = tattoo.Id,
            SizeCm = tattoo.SizeCm,
            Name = tattoo.name,
            Style = tattoo.Style,
            FinalTattooPrice = tattoo.FinalTattooPrice,
            JobId = tattoo.JobId,
            FillScore = tattoo.FillScore,
            ShadowScore = tattoo.ShadowScore,
            DetailScore = tattoo.DetailScore,
            HasColor = tattoo.HasColor,
            BodyZoneScore = tattoo.BodyZoneScore,
            EstimatedPrice = tattoo.EstimatePrice(),
            CalculatedGhostPoints = tattoo.GhostPoints()
        };
    }
}