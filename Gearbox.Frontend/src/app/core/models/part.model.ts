export interface Part {
  id: string;
  name: string;
  description: string;
  partNumber: string;
  sellingPrice: number;
  stockQuantity: number;
  vendorId: string;
}

export interface NewPart {
  name: string;
  description: string;
  partNumber: string;
  sellingPrice: number;
  stockQuantity: number;
  vendorId: string;
}
