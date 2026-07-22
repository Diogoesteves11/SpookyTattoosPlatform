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

using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Application.DTOs.Promos;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Domain.Entities;
using SpookyTattoos.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class PromoService : IPromoService
{
    private readonly IPromoRepository _promoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PromoService(IPromoRepository promoRepository, IUnitOfWork unitOfWork)
    {
        _promoRepository = promoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PromoListDto>> GetAllAsync()
    {
        var promos = await _promoRepository.GetAllAsync();

        return promos.Select(p => new PromoListDto 
        {
            Id          = p.Id,
            Description = p.Description,
            GhostPoints = p.GhostPoints,
            StartDate   = p.StartDate,
            EndDate     = p.EndDate
        });
    }

    public async Task<PromoDto> GetByIdAsync(int id)
    {
        var promo = await _promoRepository.GetByIdAsync(id);

        if(promo == null)
        {
            throw new NotFoundException("Promo", id);
        }

        return new PromoDto
        {
            Id = promo.Id,
            Description = promo.Description,
            Conditions = promo.Conditions,
            GhostPoints = promo.GhostPoints,
            StartDate = promo.StartDate,
            EndDate = promo.EndDate
        };
    }

    public async Task<IEnumerable<PromoListDto>> GetActivePromos()
    {
        var promos = await _promoRepository.GetActivePromosAsync();

        return promos.Select(p => new PromoListDto 
        {
            Id          = p.Id,
            Description = p.Description,
            GhostPoints = p.GhostPoints,
            StartDate   = p.StartDate,
            EndDate     = p.EndDate
        });
    }

    public async Task CreateAsync(CreatePromoDto createPromoDto)
    {
        var promo = new Promo
        {
            Description = createPromoDto.Description,
            Conditions = createPromoDto.Conditions,
            GhostPoints = createPromoDto.GhostPoints,
            StartDate = createPromoDto.StartDate,
            EndDate = createPromoDto.EndDate
        };

        if (!promo.IsDateRangeValid())
        {
            throw new BadRequestException("The end date cannot shorter then the start date.");
        }

        await _promoRepository.AddAsync(promo);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(UpdatePromoDto updatePromoDto)
    {
        var promo = await _promoRepository.GetByIdAsync(updatePromoDto.Id);
        
        if (promo == null)
        {
            throw new NotFoundException("Promo", updatePromoDto.Id);
        }

        promo.Description = updatePromoDto.Description;
        promo.Conditions = updatePromoDto.Conditions;
        promo.GhostPoints = updatePromoDto.GhostPoints;
        promo.StartDate = updatePromoDto.StartDate;
        promo.EndDate = updatePromoDto.EndDate;

        if (!promo.IsDateRangeValid())
        {
            throw new BadRequestException("The end date cannot shorter then the start date.");
        }

        _promoRepository.Update(promo);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var promo = await _promoRepository.GetByIdAsync(id);
        
        if (promo == null)
        {
            throw new NotFoundException("Promo", id);
        }

        _promoRepository.Delete(promo);
        await _unitOfWork.CommitAsync();
    }
}