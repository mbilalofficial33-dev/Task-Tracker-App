import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { Task, TaskStatus } from '../../models/task.model';
import { Notification } from '../../models/notification.model';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { User } from '../../models/user.model';
import { UserService } from '../../services/user.service';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {

  TaskStatus = TaskStatus;

  tasks: Task[] = [];
  currentTasks: Task[] = [];
  notifications: Notification[] = [];
  unreadCount = 0;
  loading = false;
  notificationsLoading = false;
  createTaskLoading = false;
  activeTab = 'assigned';
  showNotifications = false;
  createTaskForm!: FormGroup;
  user: User | null = null;
  users: User[] = [];
  tasksError = '';
  notificationsError = '';
  createTaskError = '';
  statusUpdateError = '';

  constructor(
    private taskService: TaskService,
    private notificationService: NotificationService,
    private authService: AuthService,
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router
  ) {
    this.createTaskForm = this.fb.group({
      title: ['', Validators.required],
  description: [''],
  dueDate: [''],
  assignedToId: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.user = this.authService.currentUser;
    this.loadData();
    this.loadNotifications();
    this.loadUsers();
  }

  loadData() {
    this.loading = true;
    this.tasksError = '';

    const apiCall = this.activeTab === 'assigned'
      ? this.taskService.getAssignedToMe()
      : this.taskService.getAssignedByMe();

    apiCall.subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.currentTasks = tasks;
        this.loading = false;
      },
      error: () => {
        this.tasksError = 'Unable to load tasks right now.';
        this.loading = false;
      }
    });
  }

  loadUsers() {
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users.filter((u) => u.id !== this.user?.id);
      },
      error: () => {
        this.users = [];
      }
    });
  }

  loadNotifications() {
    this.notificationsLoading = true;
    this.notificationsError = '';
    this.notificationService.getNotifications().subscribe({
      next: (notifications) => {
        this.notifications = notifications;
        this.unreadCount = notifications.filter(n => !n.isRead).length;
        this.notificationsLoading = false;
      },
      error: () => {
        this.notificationsError = 'Unable to load notifications.';
        this.notificationsLoading = false;
      }
    });
  }

  setTab(tab: string) {
    this.activeTab = tab;
    this.loadData();
  }

  createTask() {
    if (this.createTaskForm.valid) {
      this.createTaskLoading = true;
      this.createTaskError = '';
      const taskData = this.createTaskForm.value;

      this.taskService.createTask(taskData).subscribe({
        next: () => {
          this.createTaskForm.reset();
          this.loadData();
          this.createTaskLoading = false;
        },
        error: () => {
          this.createTaskError = 'Unable to create task.';
          this.createTaskLoading = false;
        }
      });
    }
  }

  updateTaskStatus(taskId: number, status: TaskStatus) {
    this.statusUpdateError = '';
    this.taskService.updateStatus(taskId, status).subscribe({
      next: (updatedTask) => {
        this.tasks = this.tasks.map(t => t.id === updatedTask.id ? updatedTask : t);
        this.currentTasks = this.currentTasks.map(t => t.id === updatedTask.id ? updatedTask : t);
        this.loadNotifications();
      },
      error: () => {
        this.statusUpdateError = 'Status update failed. Transition may be invalid or unauthorized.';
      }
    });
  }

  canStartTask(task: Task) {
    return this.isAssignee(task) && task.status === TaskStatus.Assigned;
  }

  canCompleteTask(task: Task) {
    return this.isAssignee(task) && [TaskStatus.Assigned, TaskStatus.InProgress].includes(task.status);
  }

  canApproveTask(task: Task) {
    return this.isAssigner(task) && task.status === TaskStatus.Completed;
  }

  isAssignee(task: Task) {
    if (task.assignedToId && this.user?.id) {
      return task.assignedToId === this.user.id;
    }
    return task.assignedToName === this.user?.username;
  }

  isAssigner(task: Task) {
    if (task.assignedById && this.user?.id) {
      return task.assignedById === this.user.id;
    }
    return task.assignedByName === this.user?.username;
  }

  getTaskStatusClass(status: TaskStatus) {
    return status;
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    if (this.showNotifications) {
      this.markVisibleNotificationsAsRead();
    }
  }

  markVisibleNotificationsAsRead() {
    const hasUnread = this.notifications.some((n) => !n.isRead);
    if (!hasUnread) {
      return;
    }

    this.notificationService.markAllAsRead().subscribe({
      next: () => {
        this.notifications = this.notifications.map((n) => ({ ...n, isRead: true }));
        this.unreadCount = 0;
      },
      error: () => {
        this.notificationsError = 'Unable to mark notifications as read.';
      }
    });
  }

  markAsRead(notificationId: number) {
    this.notificationService.markAsRead(notificationId).subscribe(() => {
      this.notifications = this.notifications.map(n =>
        n.id === notificationId ? { ...n, isRead: true } : n
      );
      this.unreadCount = this.notifications.filter(n => !n.isRead).length;
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}