using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffService.Models;
using StaffService.Services;

namespace StaffService.Controllers;

[ApiController]
[Route("api/staff")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly StaffBusinessService _staffService;

    public StaffController(StaffBusinessService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> GetAllStaff()
    {
        var staff = await _staffService.GetAllStaffAsync();
        return Ok(new ApiResponse<IEnumerable<StaffDto>>
        {
            Success = true,
            Message = "Staff retrieved successfully",
            Data = staff
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> GetStaff(int id)
    {
        var staff = await _staffService.GetStaffByIdAsync(id);
        if (staff == null)
        {
            return NotFound(new ApiResponse<StaffDto>
            {
                Success = false,
                Message = "Staff not found"
            });
        }

        return Ok(new ApiResponse<StaffDto>
        {
            Success = true,
            Message = "Staff retrieved successfully",
            Data = staff
        });
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<StaffDto>
            {
                Success = false,
                Message = "Invalid request data"
            });
        }

        var staff = await _staffService.CreateStaffAsync(request);
        return CreatedAtAction(nameof(GetStaff), new { id = staff.Id }, new ApiResponse<StaffDto>
        {
            Success = true,
            Message = "Staff created successfully",
            Data = staff
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> UpdateStaff(int id, [FromBody] UpdateStaffRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<StaffDto>
            {
                Success = false,
                Message = "Invalid request data"
            });
        }

        var staff = await _staffService.UpdateStaffAsync(id, request);
        if (staff == null)
        {
            return NotFound(new ApiResponse<StaffDto>
            {
                Success = false,
                Message = "Staff not found"
            });
        }

        return Ok(new ApiResponse<StaffDto>
        {
            Success = true,
            Message = "Staff updated successfully",
            Data = staff
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        var result = await _staffService.DeleteStaffAsync(id);
        if (!result)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = "Staff not found"
            });
        }

        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "Staff deleted successfully"
        });
    }
}

[ApiController]
[Route("api/departments")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly StaffBusinessService _staffService;

    public DepartmentsController(StaffBusinessService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> GetAllDepartments()
    {
        var departments = await _staffService.GetAllDepartmentsAsync();
        return Ok(new ApiResponse<IEnumerable<DepartmentDto>>
        {
            Success = true,
            Message = "Departments retrieved successfully",
            Data = departments
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Owner")]
    public async Task<IActionResult> GetDepartment(int id)
    {
        var department = await _staffService.GetDepartmentByIdAsync(id);
        if (department == null)
        {
            return NotFound(new ApiResponse<DepartmentDto>
            {
                Success = false,
                Message = "Department not found"
            });
        }

        return Ok(new ApiResponse<DepartmentDto>
        {
            Success = true,
            Message = "Department retrieved successfully",
            Data = department
        });
    }

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<DepartmentDto>
            {
                Success = false,
                Message = "Invalid request data"
            });
        }

        var department = await _staffService.CreateDepartmentAsync(request);
        return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, new ApiResponse<DepartmentDto>
        {
            Success = true,
            Message = "Department created successfully",
            Data = department
        });
    }
}