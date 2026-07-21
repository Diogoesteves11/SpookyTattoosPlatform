using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Interfaces.Services;

// Ajusta o namespace do DTO conforme necessário
// using SpookyTattoos.Application.DTOs.Promos;

public interface IPromoService
{
    // Apenas assinaturas base para permitir o build inicial
    Task<IEnumerable<object>> GetAllAsync(); // Substitui 'object' pelo teu PromoDto
}