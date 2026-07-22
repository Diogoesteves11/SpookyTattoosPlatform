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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VoucherService(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VoucherDto> GetByIdAsync(int id)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);

        if (voucher == null)
        {
            throw new NotFoundException("Voucher", id);
        }

        return new VoucherDto
        {
            Id = voucher.Id,
            Value = voucher.Value,
            IsUsed = voucher.IsUsed,
            GeneratedAt = voucher.GeneratedAt,
            ExpiresAt = voucher.ExpiresAt,
            EmitterId = voucher.EmitterId,
            EmitterName = voucher.Emitter?.FullName ?? "Desconhecido" 
        };
    }

    public async Task<IEnumerable<VoucherListDto>> GetValidVouchersAsync()
    {
        var vouchers = await _voucherRepository.GetValidVouchersAsync();

        return vouchers.Select(v => new VoucherListDto
        {
            Id = v.Id,
            Value = v.Value,
            IsUsed = v.IsUsed,
            ExpiresAt = v.ExpiresAt,
            EmitterId = v.EmitterId
        });
    }

    public async Task<IEnumerable<VoucherListDto>> GetByEmitterIdAsync(int emitterId)
    {
        var vouchers = await _voucherRepository.GetByEmitterIdAsync(emitterId);

        return vouchers.Select(v => new VoucherListDto
        {
            Id = v.Id,
            Value = v.Value,
            IsUsed = v.IsUsed,
            ExpiresAt = v.ExpiresAt,
            EmitterId = v.EmitterId
        });
    }

    public async Task UseVoucherAsync(int id)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);
        
        if (voucher == null)
        {
            throw new NotFoundException("Voucher", id);
        }

        if (voucher.IsExpired())
        {
            throw new BadRequestException("Este voucher já expirou e não pode ser utilizado.");
        }

        try
        {
            voucher.MarkAsUsed();
            
            _voucherRepository.Update(voucher);
            await _unitOfWork.CommitAsync();
        }
        catch (InvalidOperationException ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }
}