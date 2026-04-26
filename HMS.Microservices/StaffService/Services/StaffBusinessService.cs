using Microsoft.EntityFrameworkCore;
using StaffService.Data;
using StaffService.Models;

namespace StaffService.Services;

public class StaffBusinessService
{
    private readonly ApplicationDbContext _context;

    public StaffBusinessService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
    {
        var staff = await _context.Staff
            .Include(s => s.Department)
            .ToListAsync();

        return staff.Select(s => new StaffDto
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Email = s.Email,
            Phone = s.Phone,
            DepartmentId = s.DepartmentId,
            DepartmentName = s.Department?.Name ?? string.Empty,
            Role = s.Role,
            HireDate = s.HireDate,
            IsActive = s.IsActive
        });
    }

    public async Task<StaffDto?> GetStaffByIdAsync(int id)
    {
        var staff = await _context.Staff
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (staff == null) return null;

        return new StaffDto
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Email = staff.Email,
            Phone = staff.Phone,
            DepartmentId = staff.DepartmentId,
            DepartmentName = staff.Department?.Name ?? string.Empty,
            Role = staff.Role,
            HireDate = staff.HireDate,
            IsActive = staff.IsActive
        };
    }

    public async Task<StaffDto> CreateStaffAsync(CreateStaffRequest request)
    {
        var staff = new Staff
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            DepartmentId = request.DepartmentId,
            Role = request.Role,
            HireDate = DateTime.UtcNow,
            IsActive = true
        };

        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();

        return await GetStaffByIdAsync(staff.Id) ?? throw new InvalidOperationException("Failed to create staff");
    }

    public async Task<StaffDto?> UpdateStaffAsync(int id, UpdateStaffRequest request)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null) return null;

        staff.FirstName = request.FirstName;
        staff.LastName = request.LastName;
        staff.Email = request.Email;
        staff.Phone = request.Phone;
        staff.DepartmentId = request.DepartmentId;
        staff.Role = request.Role;
        staff.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return await GetStaffByIdAsync(id);
    }

    public async Task<bool> DeleteStaffAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null) return false;

        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
    {
        var departments = await _context.Departments
            .Include(d => d.Staff)
            .ToListAsync();

        return departments.Select(d => new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            StaffCount = d.Staff.Count
        });
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Staff)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null) return null;

        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            StaffCount = department.Staff.Count
        };
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentRequest request)
    {
        var department = new Department
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return await GetDepartmentByIdAsync(department.Id) ?? throw new InvalidOperationException("Failed to create department");
    }
}