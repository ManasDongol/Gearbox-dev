export interface UserProfile {
  username: string;
  email: string;
  passwordHash?: string;
  role?: string;
  firstName: string;
  lastName: string;
  address: string;
  phoneNumber: string;
  isActive: boolean;
  emailVerified?: boolean;
}
