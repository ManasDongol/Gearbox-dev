export interface Appointment {
  id: string;
  customerId: string;
  vehicleId: string;
  appointmentDate: string;
  status: string;
  notes: string;
  createdDate: string;
}

export interface NewAppointment {
  customerId: string;
  vehicleId: string;
  appointmentDate: string;
  status: string;
  notes: string;
  createdDate: string;
}
