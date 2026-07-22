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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Interfaces.Services;

public interface ICouponService
{
    Task<CouponListDto> GetByIdAsync(int id);
    Task<IEnumerable<CouponListDto>> GetByClientIdAsync(int clientId);
    Task<IEnumerable<CouponListDto>> GetByPromoIdAsync(int promoId);
    
    Task UseCouponAsync(int id);

    Task CreateAsync(CreateCouponDto dto);
    Task UpdateAsync(int id, UpdateCouponDto dto);
}