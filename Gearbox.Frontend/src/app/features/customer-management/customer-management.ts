import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Navmenu } from '../../shared/components/navmenu/navmenu';

interface Customer {
  id: number;
  name: string;
  email: string;
  phone: string;
  address: string;
  status: string;
  moneySpent: number;
  totalOrders: number;
}

@Component({
  selector: 'app-customer-management',
  imports: [Navmenu, FormsModule],
  templateUrl: './customer-management.html',
  styleUrl: './customer-management.css',
})
export class CustomerManagement {
  customers: Customer[] = [
    {
      id: 1,
      name: 'Ayush Das',
      email: 'ayushdas@gmail.com',
      phone: '9810556677',
      address: 'Kharibot, Lalitpur',
      status: 'Active',
      moneySpent: 19000,
      totalOrders: 16,
    },
    {
      id: 2,
      name: 'Suman Poudel',
      email: 'suman.poudel@gmail.com',
      phone: '9841234567',
      address: 'Baneshwor, Kathmandu',
      status: 'Active',
      moneySpent: 45200,
      totalOrders: 32,
    },
    {
      id: 3,
      name: 'Rita Shrestha',
      email: 'rita.shrestha@gmail.com',
      phone: '9812345678',
      address: 'Pokhara, Kaski',
      status: 'Inactive',
      moneySpent: 8500,
      totalOrders: 5,
    },
    {
      id: 4,
      name: 'Bikram Tamang',
      email: 'bikram.t@gmail.com',
      phone: '9823456789',
      address: 'Bhaktapur, Bhaktapur',
      status: 'Active',
      moneySpent: 27300,
      totalOrders: 21,
    },
    {
      id: 5,
      name: 'Anita Maharjan',
      email: 'anita.m@gmail.com',
      phone: '9834567890',
      address: 'Patan, Lalitpur',
      status: 'Active',
      moneySpent: 15800,
      totalOrders: 12,
    },
  ];

  searchQuery: string = '';
  statusFilter: string = '';
  showCustomerModal: boolean = false;
  selectedCustomer: Customer | null = null;

  get filteredCustomers() {
    return this.customers.filter((c) => {
      const matchesSearch =
        !this.searchQuery.trim() ||
        c.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        c.email.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        c.id.toString().includes(this.searchQuery);

      const matchesStatus =
        !this.statusFilter || c.status === this.statusFilter;

      return matchesSearch && matchesStatus;
    });
  }

  viewCustomer(customer: Customer) {
    this.selectedCustomer = customer;
    this.showCustomerModal = true;
  }

  closeCustomerModal() {
    this.showCustomerModal = false;
    this.selectedCustomer = null;
  }
}
