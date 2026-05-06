export interface Customer {

  userId: string;
  userName:string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  address: string;
  totalSpent: number;
  pendingCredits: number;
  registeredSince: string;
}

export interface NewCustomer {
  userName: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  email: string;
  password: string;
  address: string;
  totalSpent: number;
  pendingCredits: number;
}
