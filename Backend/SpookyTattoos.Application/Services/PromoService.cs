using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class PromoService : IPromoService
{
    private readonly IPromoRepository _promoRepository;

    public PromoService(IPromoRepository promoRepository)
    {
        _promoRepository = promoRepository;
    }

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        // Placeholder até avançarmos na implementação concreta
        throw new NotImplementedException();
    }
}