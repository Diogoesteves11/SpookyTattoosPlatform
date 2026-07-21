using SpookyTattoos.Application.DTOs.Promos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Interfaces.Services;

public interface IVoucherService
{
    Task<IEnumerable<VoucherDto>> GetAllAsync();
    Task<VoucherDto> GetByIdAsync(int id);
}