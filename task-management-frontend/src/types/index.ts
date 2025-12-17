export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  isActive: boolean;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: number;
}

export interface Task {
  id: number;
  title: string;
  description: string;
  status: string;
  priority: string;
  dueDate: string | null;
  createdById: number;
  createdByName: string;
  assignedToId: number | null;
  assignedToName: string | null;
  teamId: number | null;
  teamName: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateTaskDto {
  title: string;
  description: string;
  priority: number;
  dueDate: string | null;
  assignedToId: number | null;
  teamId: number | null;
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  status?: number;
  priority?: number;
  dueDate?: string | null;
  assignedToId?: number | null;
}

export interface Team {
  id: number;
  name: string;
  description: string;
  managerId: number;
  managerName: string;
  createdAt: string;
  members: TeamMember[];
}

export interface TeamMember {
  userId: number;
  name: string;
  email: string;
  joinedAt: string;
}

export interface CreateTeamDto {
  name: string;
  description: string;
  managerId?: number;
}

export interface Comment {
  id: number;
  content: string;
  taskId: number;
  userId: number;
  userName: string;
  createdAt: string;
}

export interface CreateCommentDto {
  content: string;
  taskId: number;
}

export interface DashboardStats {
  totalTasks: number;
  toDoTasks: number;
  inProgressTasks: number;
  doneTasks: number;
  overdueTasks: number;
  highPriorityTasks: number;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string[];
}