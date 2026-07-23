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

using System;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Interfaces.External;

public interface IEmailService
{
    /// <summary>
    /// Envia um Voucher de oferta para o email do cliente.
    /// </summary>
    Task SendVoucherAsync(string toEmail, string clientName, string voucherCode, decimal amount, DateTimeOffset expirationDate);

    /// <summary>
    /// Envia um Cupão de desconto promocional para o email do cliente.
    /// </summary>
    Task SendCouponAsync(string toEmail, string clientName, string couponCode, decimal discountPercentage, DateTimeOffset expirationDate);

    /// <summary>
    /// Método base para envio de emails genéricos (avisos, recuperações de password, etc).
    /// </summary>
    Task SendGenericEmailAsync(string toEmail, string subject, string htmlBody);
}