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

using SpookyTattoos.Application.DTOs.Promos;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Repositories;
using SpookyTattoos.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CouponService(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
    {
        _couponRepository = couponRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CouponListDto> GetByIdAsync(int id)
    {
        var coupon = await _couponRepository.GetByIdAsync(id);

        if (coupon == null)
        {
            throw new NotFoundException("Coupon", id);
        }

        return new CouponListDto
        {
            Id = coupon.Id,
            ClientId = coupon.ClientId,
            ClientName = coupon.Client?.FullName ?? "Desconhecido",
            PromoId = coupon.PromoId,
            PromoDescription = coupon.Promo?.Description ?? "Promoção Desconhecida",
            IsUsed = coupon.IsUsed,
            AcquiredAt = coupon.AcquiredAt
        };
    }

    public async Task<IEnumerable<CouponListDto>> GetByClientIdAsync(int clientId)
    {
        var coupons = await _couponRepository.GetByClientIdAsync(clientId);

        return coupons.Select(c => new CouponListDto
        {
            Id = c.Id,
            ClientId = c.ClientId,
            ClientName = c.Client?.FullName ?? "Desconhecido",
            PromoId = c.PromoId,
            PromoDescription = c.Promo?.Description ?? "Promoção Desconhecida",
            IsUsed = c.IsUsed,
            AcquiredAt = c.AcquiredAt
        });
    }

    public async Task<IEnumerable<CouponListDto>> GetByPromoIdAsync(int promoId)
    {
        var coupons = await _couponRepository.GetByPromoIdAsync(promoId);

        return coupons.Select(c => new CouponListDto
        {
            Id = c.Id,
            ClientId = c.ClientId,
            ClientName = "N/A", 
            PromoId = c.PromoId,
            PromoDescription = "N/A",
            IsUsed = c.IsUsed,
            AcquiredAt = c.AcquiredAt
        });
    }

    public async Task UseCouponAsync(int id)
    {
        var coupon = await _couponRepository.GetByIdAsync(id);
        
        if (coupon == null)
        {
            throw new NotFoundException("Coupon", id);
        }

        if (coupon.IsUsed)
        {
            throw new BadRequestException("Este cupão já foi utilizado anteriormente.");
        }

        if (coupon.Promo != null && coupon.Promo.EndDate.HasValue && coupon.Promo.EndDate.Value < System.DateTimeOffset.UtcNow)
        {
            throw new BadRequestException("A promoção associada a este cupão já expirou.");
        }

        coupon.IsUsed = true;

        _couponRepository.Update(coupon);
        await _unitOfWork.CommitAsync();
    }

    public async Task CreateAsync(CreateCouponDto dto)
    {
        var newCoupon = new Coupon
        {
            ClientId = dto.ClientId,
            PromoId = dto.PromoId,
            IsUsed = false,
            AcquiredAt = System.DateTimeOffset.UtcNow
        };

        await _couponRepository.AddAsync(newCoupon);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(int id, UpdateCouponDto dto)
    {
        var coupon = await _couponRepository.GetByIdAsync(id);
        
        if (coupon == null)
        {
            throw new NotFoundException("Coupon", id);
        }

        coupon.ClientId = dto.ClientId;
        coupon.PromoId = dto.PromoId;
        coupon.IsUsed = dto.IsUsed;

        _couponRepository.Update(coupon);
        await _unitOfWork.CommitAsync();
    }
}