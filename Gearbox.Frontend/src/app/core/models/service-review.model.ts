export interface ServiceReview {
  id: string;
  customerId: string;
  appointmentId?: string | null;
  serviceId?: string | null;
  rating: number;
  comment: string;
  reviewDate: string;
}

export interface NewServiceReview {
  customerId: string;
  appointmentId?: string | null;
  serviceId?: string | null;
  rating: number;
  comment: string;
  reviewDate: string;
}
