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
using System.Collections.Generic;

namespace SpookyTattoos.Application.DTOs.Financial;

public class FinancialReportDto
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    
    public decimal TotalRevenue { get; set; }
    public int TotalJobsCompleted { get; set; }
    public decimal AverageJobPrice { get; set; }
    
    // Divisão de receitas por tipo de trabalho (TATTOO, PIERCING, etc.)
    public Dictionary<string, decimal> RevenueByJobType { get; set; } = new Dictionary<string, decimal>();
}