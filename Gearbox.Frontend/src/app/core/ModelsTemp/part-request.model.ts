export interface PartRequest {
  id: string;
  customerId: string;
  partName: string;
  description: string;
  isFulfilled: boolean;
  requestDate: string;
}

export interface NewPartRequest {
  customerId: string;
  partName: string;
  description: string;
  isFulfilled: boolean;
  requestDate: string;
}
