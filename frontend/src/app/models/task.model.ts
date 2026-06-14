export enum TaskStatus {
  Assigned = 'Assigned',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Approved = 'Approved'
}

export interface Task {
  id: number;
  title: string;
  description?: string;
  dueDate?: string;
  status: TaskStatus;
  assignedById?: number;
  assignedToId?: number;
  assignedByName: string;
  assignedToName: string;
  createdAt: string;
}

export interface CreateTask {
  title: string;
  description?: string;
  dueDate?: string;
  assignedToId: number;
}