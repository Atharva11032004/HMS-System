import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StaffService, StaffDto, DepartmentDto, CreateStaffRequest, UpdateStaffRequest } from '../../../services/staff.service';

@Component({
  selector: 'app-staff-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './staff-management.html',
  styleUrl: './staff-management.css'
})
export class StaffManagement implements OnInit {
  activeTab: 'staff' | 'departments' = 'staff';
  staff: StaffDto[] = [];
  departments: DepartmentDto[] = [];
  filteredStaff: StaffDto[] = [];
  searchQuery = '';
  filterRole = '';
  filterDept = '';
  loading = false;
  error = '';
  successMsg = '';

  showStaffModal = false;
  showDeptModal = false;
  showDeleteConfirm = false;
  editingStaff: StaffDto | null = null;
  deletingId: number | null = null;

  staffForm: CreateStaffRequest & { isActive: boolean } = this.emptyStaffForm();
  deptForm = { name: '', description: '' };

  roles = ['Staff', 'Manager', 'Receptionist', 'Housekeeping', 'Security', 'Maintenance'];

  constructor(private staffSvc: StaffService) {}

  ngOnInit() {
    this.loadAll();
  }

  loadAll() {
    this.loading = true;
    this.staffSvc.getAllDepartments().subscribe({
      next: d => { this.departments = d; this.loadStaff(); },
      error: () => { this.error = 'Failed to load departments'; this.loading = false; }
    });
  }

  loadStaff() {
    this.staffSvc.getAllStaff().subscribe({
      next: s => { this.staff = s; this.applyFilters(); this.loading = false; },
      error: () => { this.error = 'Failed to load staff'; this.loading = false; }
    });
  }

  applyFilters() {
    this.filteredStaff = this.staff.filter(s => {
      const q = this.searchQuery.toLowerCase();
      const matchSearch = !q || `${s.firstName} ${s.lastName} ${s.email}`.toLowerCase().includes(q);
      const matchRole = !this.filterRole || s.role === this.filterRole;
      const matchDept = !this.filterDept || s.departmentId === +this.filterDept;
      return matchSearch && matchRole && matchDept;
    });
  }

  openCreateStaff() {
    this.editingStaff = null;
    this.staffForm = this.emptyStaffForm();
    this.showStaffModal = true;
  }

  openEditStaff(s: StaffDto) {
    this.editingStaff = s;
    this.staffForm = { firstName: s.firstName, lastName: s.lastName, email: s.email, phone: s.phone, departmentId: s.departmentId, role: s.role, isActive: s.isActive };
    this.showStaffModal = true;
  }

  saveStaff() {
    if (this.editingStaff) {
      const req: UpdateStaffRequest = { ...this.staffForm };
      this.staffSvc.updateStaff(this.editingStaff.id, req).subscribe({
        next: () => { this.showSuccess('Staff updated successfully'); this.closeStaffModal(); this.loadStaff(); },
        error: () => this.error = 'Failed to update staff'
      });
    } else {
      this.staffSvc.createStaff(this.staffForm).subscribe({
        next: () => { this.showSuccess('Staff created successfully'); this.closeStaffModal(); this.loadStaff(); },
        error: () => this.error = 'Failed to create staff'
      });
    }
  }

  confirmDelete(id: number) {
    this.deletingId = id;
    this.showDeleteConfirm = true;
  }

  deleteStaff() {
    if (this.deletingId == null) return;
    this.staffSvc.deleteStaff(this.deletingId).subscribe({
      next: () => { this.showSuccess('Staff deleted'); this.showDeleteConfirm = false; this.deletingId = null; this.loadStaff(); },
      error: () => this.error = 'Failed to delete staff'
    });
  }

  saveDepartment() {
    this.staffSvc.createDepartment(this.deptForm).subscribe({
      next: () => { this.showSuccess('Department created'); this.showDeptModal = false; this.deptForm = { name: '', description: '' }; this.loadAll(); },
      error: () => this.error = 'Failed to create department'
    });
  }

  closeStaffModal() { this.showStaffModal = false; this.editingStaff = null; }

  showSuccess(msg: string) {
    this.successMsg = msg;
    setTimeout(() => this.successMsg = '', 3000);
  }

  emptyStaffForm() {
    return { firstName: '', lastName: '', email: '', phone: '', departmentId: 0, role: 'Staff', isActive: true };
  }

  getDeptName(id: number) {
    return this.departments.find(d => d.id === id)?.name ?? '—';
  }

  get activeStaffCount() { return this.staff.filter(s => s.isActive).length; }
  get inactiveStaffCount() { return this.staff.filter(s => !s.isActive).length; }
}
