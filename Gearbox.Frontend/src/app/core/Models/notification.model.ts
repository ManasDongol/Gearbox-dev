export interface NotificationItem {
  id: string;
  userId: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  targetRole?: string;
  isGlobal?: boolean;
}
