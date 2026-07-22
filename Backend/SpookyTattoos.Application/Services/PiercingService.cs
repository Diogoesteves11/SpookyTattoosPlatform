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

using SpookyTattoos.Application.DTOs.Piercings;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Repositories;
using SpookyTattoos.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class PiercingService : IPiercingService
{
    private readonly IPiercingRepository _piercingRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PiercingService(IPiercingRepository piercingRepository, IJobRepository jobRepository, IUnitOfWork unitOfWork)
    {
        _piercingRepository = piercingRepository;
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PiercingDto> GetByIdAsync(int id)
    {
        var piercing = await _piercingRepository.GetByIdAsync(id);

        if (piercing == null)
        {
            throw new NotFoundException("Piercing", id);
        }

        return MapToDto(piercing);
    }

    public async Task<IEnumerable<PiercingDto>> GetByJobIdAsync(int jobId)
    {
        var piercings = await _piercingRepository.GetByJobIdAsync(jobId);

        return piercings.Select(MapToDto);
    }

    public async Task CreateAsync(CreatePiercingDto dto)
    {
        var jobExists = await _jobRepository.GetByIdAsync(dto.JobId);
        if (jobExists == null)
        {
            throw new NotFoundException("Job", dto.JobId);
        }

        var newPiercing = new Piercing
        {
            JobId = dto.JobId,
            BodyPart = dto.BodyPart,
            Type = dto.Type
        };

        await _piercingRepository.AddAsync(newPiercing);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(int id, UpdatePiercingDto dto)
    {
        var piercing = await _piercingRepository.GetByIdAsync(id);
        
        if (piercing == null)
        {
            throw new NotFoundException("Piercing", id);
        }

        piercing.BodyPart = dto.BodyPart;
        piercing.Type = dto.Type;

        _piercingRepository.Update(piercing);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var piercing = await _piercingRepository.GetByIdAsync(id);
        
        if (piercing == null)
        {
            throw new NotFoundException("Piercing", id);
        }

        _piercingRepository.Delete(piercing);
        await _unitOfWork.CommitAsync();
    }

    private PiercingDto MapToDto(Piercing piercing)
    {
        return new PiercingDto
        {
            Id = piercing.Id,
            JobId = piercing.JobId,
            BodyPart = piercing.BodyPart,
            Type = piercing.Type,
            CalculatedGhostPoints = piercing.GhostPoints()
        };
    }
}