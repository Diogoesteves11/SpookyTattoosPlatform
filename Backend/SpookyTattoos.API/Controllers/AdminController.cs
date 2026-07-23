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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpookyTattoos.Application.Interfaces.Services;
using SpookyTattoos.Application.DTOs.Admins;
using SpookyTattoos.Application.DTOs.Financial;
using SpookyTattoos.API.Queries;

namespace SpookyTattoos.API.Controllers;

[ApiController]
[Route("api/admin")]

public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IFinancialService _financialService;

    public AdminController(IAdminService adminService, IFinancialService financialService)
    {
        _adminService = adminService;
        _financialService = financialService;
    }

    [HttpGet("")]
    [Authorize]
    public async Task<IActionResult> ListAdminsAsync([FromQuery] AdminSearchQuery query)
    {   
        try
        {
            if (query.id.HasValue)
            {
                var admin = await _adminService.GetByIdAsync(query.id.Value);
                return Ok(new[] { admin });
            }

            if (!string.IsNullOrWhiteSpace(query.username))
            {
                var admin = await _adminService.GetByUsernameAsync(query.username);
                return Ok(new[] { admin });
            }

            if (!string.IsNullOrWhiteSpace(query.email))
            {
                var admins = await _adminService.SearchAsync(query.email);
                return Ok(admins);
            }

            if (query.active.HasValue && query.active.Value)
            {
                var admins = await _adminService.GetActiveAdminsAsync();
                return Ok(admins);
            }

            var allAdmins = await _adminService.GetAllAsync();
            return Ok(allAdmins);
        }
        catch (SpookyTattoos.Application.Exceptions.NotFoundException ex)
        {
            return StatusCode(404, new { message = ex.Message }); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal error listing admins.", error = ex.Message });
        }
    }

    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> CreateAdminAsync([FromBody] CreateAdminDto dto)
    {
        try
        {
            await _adminService.CreateAsync(dto);
            return StatusCode(201, new { message = "New Admin Created." });
        }
        catch (SpookyTattoos.Application.Exceptions.ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (SpookyTattoos.Application.Exceptions.BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error creating Admin.", error = ex.Message });
        }
    }

    [HttpPatch("{id}/deactivate")]
    [Authorize]
    public async Task<IActionResult> DeactivateAdminAsync(int id)
    {
        try
        {
            var updateDto = new UpdateAdminDto { Active = false }; 
            await _adminService.UpdateAsync(id, updateDto);
            
            return Ok(new { message = $"Admin {id} deactivated." });
        }
        catch (SpookyTattoos.Application.Exceptions.NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal Server error deactivating admin.", error = ex.Message });
        }
    }
    [HttpPatch("{id}/activate")]
    [Authorize]
    public async Task<IActionResult> ActivateAdminAsync(int id)
    {
        try
        {
            var updateDto = new UpdateAdminDto { Active = true };
            await _adminService.UpdateAsync(id, updateDto);
            
            return Ok(new { message = $"Admin {id} activated." });
        }
        catch (SpookyTattoos.Application.Exceptions.NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error activating admin.", error = ex.Message });
        }
    }

    [HttpGet("financial-report")]
    [Authorize]
    public async Task<IActionResult> GetFinancialReportAsync(
        [FromQuery] DateTimeOffset? startDate, 
        [FromQuery] DateTimeOffset? endDate, 
        [FromQuery] int? year, 
        [FromQuery] int? month)
    {
        // GET /api/admin/financial-report?year=2026&month=7
        // GET /api/admin/financial-report?startDate=2026-01-01&endDate=2026-12-31
        
        try
        {
            if (year.HasValue && month.HasValue)
            {
                var report = await _financialService.GenerateMonthlyReportAsync(year.Value, month.Value);
                return Ok(report);
            }
            
            if (startDate.HasValue && endDate.HasValue)
            {
                var report = await _financialService.GenerateReportAsync(startDate.Value, endDate.Value);
                return Ok(report);
            }

            return BadRequest(new { message = "Should indicate year + month or start date + end date." });
        }
        catch (SpookyTattoos.Application.Exceptions.BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error generating report.", error = ex.Message });
        }
    }
}
