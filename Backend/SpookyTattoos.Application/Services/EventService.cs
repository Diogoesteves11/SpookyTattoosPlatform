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

using SpookyTattoos.Application.DTOs.Events;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Repositories;
using SpookyTattoos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventDto> GetByIdAsync(int id)
    {
        var spookyEvent = await _eventRepository.GetByIdAsync(id);

        if (spookyEvent == null)
        {
            throw new NotFoundException("Event", id);
        }

        return MapToDto(spookyEvent);
    }

    public async Task<IEnumerable<EventListDto>> SearchByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<EventListDto>();
        }

        var events = await _eventRepository.SearchByNameAsync(searchTerm);

        return events.Select(MapToListDto);
    }

    public async Task<IEnumerable<EventListDto>> GetByStartDateAsync(DateTimeOffset startDate)
    {
        var events = await _eventRepository.GetByStartDateAsync(startDate);

        return events.Select(MapToListDto);
    }

    public async Task CreateAsync(CreateEventDto dto)
    {
        if (dto.EndDate < dto.StartDate)
        {
            throw new BadRequestException("A Data de Fim não pode ser anterior à Data de Início do evento.");
        }

        var newEvent = new Event
        {
            EventName = dto.EventName,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl
        };

        await _eventRepository.AddAsync(newEvent);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(int id, UpdateEventDto dto)
    {
        var spookyEvent = await _eventRepository.GetByIdAsync(id);
        
        if (spookyEvent == null)
        {
            throw new NotFoundException("Event", id);
        }

        if (dto.EndDate < dto.StartDate)
        {
            throw new BadRequestException("A Data de Fim não pode ser anterior à Data de Início do evento.");
        }

        spookyEvent.EventName = dto.EventName;
        spookyEvent.StartDate = dto.StartDate;
        spookyEvent.EndDate = dto.EndDate;
        spookyEvent.Description = dto.Description;
        spookyEvent.ImageUrl = dto.ImageUrl;

        _eventRepository.Update(spookyEvent);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var spookyEvent = await _eventRepository.GetByIdAsync(id);
        
        if (spookyEvent == null)
        {
            throw new NotFoundException("Event", id);
        }

        _eventRepository.Delete(spookyEvent);
        await _unitOfWork.CommitAsync();
    }

    private EventDto MapToDto(Event spookyEvent)
    {
        return new EventDto
        {
            Id = spookyEvent.Id,
            EventName = spookyEvent.EventName,
            StartDate = spookyEvent.StartDate,
            EndDate = spookyEvent.EndDate,
            Description = spookyEvent.Description,
            ImageUrl = spookyEvent.ImageUrl
        };
    }

    private EventListDto MapToListDto(Event spookyEvent)
    {
        return new EventListDto
        {
            Id = spookyEvent.Id,
            EventName = spookyEvent.EventName,
            StartDate = spookyEvent.StartDate,
            EndDate = spookyEvent.EndDate,
            ImageUrl = spookyEvent.ImageUrl
        };
    }
}