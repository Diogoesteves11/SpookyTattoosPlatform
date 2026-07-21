using SpookyTattoos.Application.DTOs.Promos;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;

    public VoucherService(IVoucherRepository voucherRepository)
    {
        _voucherRepository = voucherRepository;
    }

    public async Task<IEnumerable<VoucherDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<VoucherDto> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}