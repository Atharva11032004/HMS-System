import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  InventoryService, InventoryItemDto,
  CreateInventoryItemRequest, UpdateInventoryItemRequest, AdjustStockRequest
} from '../../../services/inventory.service';

type Tab = 'all' | 'lowstock';

@Component({
  selector: 'app-inventory-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inventory-management.html',
  styleUrl: './inventory-management.css'
})
export class InventoryManagement implements OnInit {
  activeTab: Tab = 'all';

  items: InventoryItemDto[] = [];
  lowStockItems: InventoryItemDto[] = [];
  loading = false;
  lowStockLoading = false;

  // Search / filter
  searchQuery = '';
  filterCategory = '';

  // Create / Edit modal
  showModal = false;
  editingItem: InventoryItemDto | null = null;
  form: CreateInventoryItemRequest = { name: '', description: '', category: '', supplier: '', quantityInStock: 0, minimumStockLevel: 0, unitPrice: 0 };

  // Adjust stock modal
  showAdjustModal = false;
  adjustingItem: InventoryItemDto | null = null;
  adjustForm: AdjustStockRequest = { quantityAdjustment: 0, reason: '' };

  // Delete confirm
  showDeleteConfirm = false;
  deletingId: number | null = null;

  successMsg = '';
  errorMsg   = '';

  constructor(private svc: InventoryService) {}

  ngOnInit() { this.loadAll(); this.loadLowStock(); }

  loadAll() {
    this.loading = true;
    this.svc.getAll().subscribe({
      next: items => { this.items = items; this.loading = false; },
      error: (e: any) => { this.showError(e?.error?.message || e?.message || 'Failed to load inventory'); this.loading = false; }
    });
  }

  loadLowStock() {
    this.lowStockLoading = true;
    this.svc.getLowStock().subscribe({
      next: items => { this.lowStockItems = items; this.lowStockLoading = false; },
      error: () => { this.lowStockLoading = false; }
    });
  }

  get filtered(): InventoryItemDto[] {
    const q = this.searchQuery.toLowerCase();
    return this.items.filter(i => {
      const matchQ = !q || `${i.name} ${i.category} ${i.supplier}`.toLowerCase().includes(q);
      const matchCat = !this.filterCategory || i.category === this.filterCategory;
      return matchQ && matchCat;
    });
  }

  get categories(): string[] {
    return [...new Set(this.items.map(i => i.category))].sort();
  }

  // ── Create / Edit ──
  openAdd() {
    this.editingItem = null;
    this.form = { name: '', description: '', category: '', supplier: '', quantityInStock: 0, minimumStockLevel: 0, unitPrice: 0 };
    this.showModal = true;
  }

  openEdit(item: InventoryItemDto) {
    this.editingItem = item;
    this.form = {
      name: item.name, description: item.description ?? '',
      category: item.category, supplier: item.supplier,
      quantityInStock: item.quantityInStock,
      minimumStockLevel: item.minimumStockLevel, unitPrice: item.unitPrice
    };
    this.showModal = true;
  }

  save() {
    if (!this.form.name || !this.form.category || !this.form.supplier || !this.form.unitPrice) {
      this.showError('Name, category, supplier and unit price are required'); return;
    }
    if (this.editingItem) {
      const req: UpdateInventoryItemRequest = {
        name: this.form.name, description: this.form.description,
        category: this.form.category, supplier: this.form.supplier,
        minimumStockLevel: +this.form.minimumStockLevel,
        unitPrice: +this.form.unitPrice, isActive: true
      };
      this.svc.update(this.editingItem.id, req).subscribe({
        next: () => { this.showSuccess('Item updated'); this.showModal = false; this.loadAll(); },
        error: (e: any) => this.showError(e?.error?.message || 'Failed to update item')
      });
    } else {
      const req: CreateInventoryItemRequest = {
        ...this.form,
        quantityInStock: +this.form.quantityInStock,
        minimumStockLevel: +this.form.minimumStockLevel,
        unitPrice: +this.form.unitPrice
      };
      this.svc.create(req).subscribe({
        next: () => { this.showSuccess('Item created'); this.showModal = false; this.loadAll(); this.loadLowStock(); },
        error: (e: any) => this.showError(e?.error?.message || 'Failed to create item')
      });
    }
  }

  // ── Adjust Stock ──
  openAdjust(item: InventoryItemDto) {
    this.adjustingItem = item;
    this.adjustForm = { quantityAdjustment: 0, reason: '' };
    this.showAdjustModal = true;
  }

  saveAdjust() {
    if (!this.adjustingItem) return;
    if (this.adjustForm.quantityAdjustment === 0) { this.showError('Adjustment cannot be zero'); return; }
    this.svc.adjustStock(this.adjustingItem.id, {
      quantityAdjustment: +this.adjustForm.quantityAdjustment,
      reason: this.adjustForm.reason
    }).subscribe({
      next: () => { this.showSuccess('Stock adjusted'); this.showAdjustModal = false; this.loadAll(); this.loadLowStock(); },
      error: (e: any) => this.showError(e?.error?.message || 'Failed to adjust stock')
    });
  }

  // ── Delete ──
  confirmDelete(id: number) { this.deletingId = id; this.showDeleteConfirm = true; }

  deleteItem() {
    if (this.deletingId == null) return;
    this.svc.delete(this.deletingId).subscribe({
      next: () => { this.showSuccess('Item deleted'); this.showDeleteConfirm = false; this.loadAll(); this.loadLowStock(); },
      error: (e: any) => this.showError(e?.error?.message || 'Failed to delete item')
    });
  }

  // ── Stats ──
  get totalItems()    { return this.items.length; }
  get activeItems()   { return this.items.filter(i => i.isActive).length; }
  get lowStockCount() { return this.lowStockItems.length; }
  get totalValue()    { return this.items.reduce((s, i) => s + i.unitPrice * i.quantityInStock, 0); }

  stockClass(item: InventoryItemDto) {
    if (item.quantityInStock === 0) return 'out';
    if (item.isLowStock) return 'low';
    return 'ok';
  }

  showSuccess(msg: string) { this.successMsg = msg; this.errorMsg = ''; setTimeout(() => this.successMsg = '', 3000); }
  showError(msg: string)   { this.errorMsg = msg; setTimeout(() => this.errorMsg = '', 6000); }
}
