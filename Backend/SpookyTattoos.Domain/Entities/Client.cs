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

namespace SpookyTattoos.Domain.Entities;

public class Client
{
    public int Id { get; set; }

    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required bool Active {get; set;} = true;

    public string? InstagramUser { get; set; }
    
    public int GhostPoints { get; set; } = 0;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastJob { get; set; }
    
    // Relações
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
    public ICollection<Voucher> IssuedVouchers { get; set; } = new List<Voucher>();

    /// <summary>
    /// Deduz os pontos do cliente e gera um Cupão para uma Promoção específica.
    /// </summary>
    public Coupon RedeemPromo(Promo promo)
    {
        if (GhostPoints < promo.GhostPoints)
        {
            throw new InvalidOperationException($"Ghost Points insuficientes. A promoção '{promo.Description}' exige {promo.GhostPoints} pontos.");
        }

        if (promo.EndDate.HasValue && promo.EndDate.Value < DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("Esta promoção já expirou.");
        }
        
        GhostPoints -= promo.GhostPoints;

        var coupon = new Coupon
        {
            ClientId = this.Id,
            Client = this, 
            PromoId = promo.Id,
            Promo = promo,
            IsUsed = false,
            AcquiredAt = DateTimeOffset.UtcNow
        };

        Coupons.Add(coupon);
        return coupon;
    }

    /// <summary>
    /// Fabrica um novo voucher de oferta emitido por este cliente.
    /// </summary>
    public Voucher IssueVoucher(decimal value)
    {
        if (value <= 0)
        {
            throw new ArgumentException("O valor do voucher tem de ser superior a zero.");
        }

        var voucher = new Voucher
        {
            EmitterId = this.Id,
            Emitter = this,
            Value = value,
            IsUsed = false,
            GeneratedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddYears(1)
        };

        IssuedVouchers.Add(voucher);
        return voucher;
    }
}