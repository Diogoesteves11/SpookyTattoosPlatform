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

using SpookyTattoos.Application.DTOs.Financial;
using SpookyTattoos.Application.Exceptions;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Domain.Entities;
using SpookyTattoos.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpookyTattoos.Application.Services;

public class FinancialService : IFinancialService
{
    private readonly IJobRepository _jobRepository;

    public FinancialService(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<FinancialReportDto> GenerateReportAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (endDate < startDate)
        {
            throw new BadRequestException("A data de fim não pode ser anterior à data de início.");
        }

        var jobsInRange = await _jobRepository.GetJobsByDateRangeAsync(startDate, endDate);

        var completedJobs = jobsInRange
            .Where(j => j.Status == JobStatus.CONCLUIDO && j.FinalPrice.HasValue)
            .ToList();

        var report = new FinancialReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalJobsCompleted = completedJobs.Count,
            TotalRevenue = completedJobs.Sum(j => j.FinalPrice!.Value),
        };

        report.AverageJobPrice = report.TotalJobsCompleted > 0 
            ? report.TotalRevenue / report.TotalJobsCompleted 
            : 0;

        report.RevenueByJobType = completedJobs
            .GroupBy(j => j.Type.ToString())
            .ToDictionary(
                group => group.Key,
                group => group.Sum(j => j.FinalPrice!.Value)
            );

        return report;
    }

    public async Task<FinancialReportDto> GenerateMonthlyReportAsync(int year, int month)
    {
        if (month < 1 || month > 12)
        {
            throw new BadRequestException("Mês inválido. Deve ser entre 1 e 12.");
        }

        var daysInMonth = DateTime.DaysInMonth(year, month);
        var startDate = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var endDate = new DateTimeOffset(year, month, daysInMonth, 23, 59, 59, TimeSpan.Zero);

        return await GenerateReportAsync(startDate, endDate);
    }
}