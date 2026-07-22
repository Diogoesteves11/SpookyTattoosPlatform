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

using SpookyTattoos.Application.DTOs.Jobs;
using SpookyTattoos.Application.DTOs.Tattoos;
using SpookyTattoos.Application.DTOs.Piercings;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Repositories;
using SpookyTattoos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public JobService(IJobRepository jobRepository, IClientRepository clientRepository, IUnitOfWork unitOfWork)
    {
        _jobRepository = jobRepository;
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<JobDto> GetByIdAsync(int id)
    {
        var job = await _jobRepository.GetJobWithDetailsAsync(id);

        if (job == null)
        {
            throw new NotFoundException("Job", id);
        }

        return new JobDto
        {
            Id = job.Id,
            ClientId = job.ClientId,
            ClientName = job.Client?.FullName ?? "Desconhecido",
            Type = job.Type,
            Status = job.Status,
            FinalPrice = job.FinalPrice,
            ScheduledDate = job.ScheduledDate,
            ReferenceImageUrl = job.ReferenceImageUrl,
            
            Tattoos = job.Tattoos.Select(t => new TattooDto
            {
                Id = t.Id,
                SizeCm = t.SizeCm,
                Name = t.name,
                Style = t.Style,
                FinalTattooPrice = t.FinalTattooPrice,
                JobId = t.JobId,
                FillScore = t.FillScore,
                ShadowScore = t.ShadowScore,
                DetailScore = t.DetailScore,
                HasColor = t.HasColor,
                BodyZoneScore = t.BodyZoneScore,
                EstimatedPrice = t.EstimatePrice(),
                CalculatedGhostPoints = t.GhostPoints()
            }).ToList(),

            Piercings = job.Piercings.Select(p => new PiercingDto
            {
                Id = p.Id,
                JobId = p.JobId,
                BodyPart = p.BodyPart,
                Type = p.Type,
                CalculatedGhostPoints = p.GhostPoints()
            }).ToList(),

            CalculatedGhostPoints = job.CalculateGhostPoints() 
        };
    }

    public async Task<IEnumerable<JobListDto>> GetJobsByClientIdAsync(int clientId)
    {
        var jobs = await _jobRepository.GetJobsByClientIdAsync(clientId);

        return jobs.Select(j => new JobListDto
        {
            Id = j.Id,
            ClientId = j.ClientId,
            ClientName = "N/A", 
            Type = j.Type,
            Status = j.Status,
            ScheduledDate = j.ScheduledDate
        });
    }

    public async Task<IEnumerable<JobListDto>> GetActiveJobsAsync()
    {
        var jobs = await _jobRepository.GetActiveJobsAsync();

        return jobs.Select(j => new JobListDto
        {
            Id = j.Id,
            ClientId = j.ClientId,
            ClientName = j.Client?.FullName ?? "N/A",
            Type = j.Type,
            Status = j.Status,
            ScheduledDate = j.ScheduledDate
        });
    }

    public async Task<IEnumerable<JobListDto>> GetJobsByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var jobs = await _jobRepository.GetJobsByDateRangeAsync(startDate, endDate);

        return jobs.Select(j => new JobListDto
        {
            Id = j.Id,
            ClientId = j.ClientId,
            ClientName = j.Client?.FullName ?? "Desconhecido",
            Type = j.Type,
            Status = j.Status,
            ScheduledDate = j.ScheduledDate
        });
    }

    public async Task CreateAsync(CreateJobDto dto)
    {
        var clientExists = await _clientRepository.GetByIdAsync(dto.ClientId);
        if (clientExists == null)
        {
            throw new NotFoundException("Client", dto.ClientId);
        }

        var newJob = new Job
        {
            ClientId = dto.ClientId,
            Type = dto.Type,
            Status = JobStatus.AGENDADO, 
            ScheduledDate = dto.ScheduledDate,
            ReferenceImageUrl = dto.ReferenceImageUrl
        };

        await _jobRepository.AddAsync(newJob);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(int id, UpdateJobDto dto)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        
        if (job == null)
        {
            throw new NotFoundException("Job", id);
        }

        job.Type = dto.Type;
        job.Status = dto.Status;
        job.FinalPrice = dto.FinalPrice;
        job.ScheduledDate = dto.ScheduledDate;
        job.ReferenceImageUrl = dto.ReferenceImageUrl;

        _jobRepository.Update(job);
        await _unitOfWork.CommitAsync();
    }
}