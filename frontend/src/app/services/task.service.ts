import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task, CreateTask, TaskStatus } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = 'https://localhost:7007/api/tasks';

  constructor(private http: HttpClient) {}

  createTask(task: CreateTask): Observable<Task> {
    return this.http.post<Task>(this.apiUrl, task);
  }

  getAssignedToMe(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/assigned-to-me`);
  }

  getAssignedByMe(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/assigned-by-me`);
  }

  getTaskById(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${id}`);
  }

  updateStatus(id: number, status: TaskStatus): Observable<Task> {
    return this.http.patch<Task>(`${this.apiUrl}/${id}/status`, { status });
  }
}