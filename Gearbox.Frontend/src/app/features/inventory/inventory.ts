import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';

@Component({
  selector: 'app-inventory',
  imports: [Navmenu, FormsModule],
  templateUrl: './inventory.html',
  styleUrl: './inventory.css',
})
export class Inventory {
  parts = [
    {
      id: 1,
      name: 'Brake Pad Set',
      sellingPrice: 2500,
      stock: 45,
      vendorId: 'VND-001',
    },
    {
      id: 2,
      name: 'Engine Oil 5W-30',
      sellingPrice: 1800,
      stock: 120,
      vendorId: 'VND-003',
    },
    {
      id: 3,
      name: 'Air Filter',
      sellingPrice: 650,
      stock: 3,
      vendorId: 'VND-002',
    },
    {
      id: 4,
      name: 'Spark Plug (Set of 4)',
      sellingPrice: 1200,
      stock: 78,
      vendorId: 'VND-007',
    },
    {
      id: 5,
      name: 'Timing Belt',
      sellingPrice: 3500,
      stock: 15,
      vendorId: 'VND-001',
    },
    {
      id: 6,
      name: 'Clutch Plate',
      sellingPrice: 4200,
      stock: 2,
      vendorId: 'VND-010',
    },
  ];

  searchQuery: string = '';
  minPrice: number | null = null;
  maxPrice: number | null = null;

  get filteredParts() {
    return this.parts.filter((p) => {
      const matchesSearch =
        !this.searchQuery.trim() ||
        p.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        p.id.toString().includes(this.searchQuery) ||
        p.vendorId.toLowerCase().includes(this.searchQuery.toLowerCase());

      const matchesMin =
        this.minPrice === null || p.sellingPrice >= this.minPrice;

      const matchesMax =
        this.maxPrice === null || p.sellingPrice <= this.maxPrice;

      return matchesSearch && matchesMin && matchesMax;
    });
  }
}
