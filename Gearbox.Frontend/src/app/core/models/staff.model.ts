export interface Staff {
  userId: string;
  firstName: string;
  lastName: string;
  department: string;
  jobTitle: string;
  role: 'Admin' | 'Staff' | 'Unassigned' | string;
}

export interface NewStaff {
  userName: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  address: string;
  email: string;
  password: string;
  department: string;
  jobTitle: string;
}
