export interface Notification {
  id: number;
  message: string;
  taskId?: number;
  isRead: boolean;
  createdAt: string;
}