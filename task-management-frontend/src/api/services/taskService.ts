import axiosInstance from '../axiosConfig';
import { Task, CreateTaskDto, UpdateTaskDto, ApiResponse } from '../../types';

export const taskService = {
  getAllTasks: async (): Promise<Task[]> => {
    const response = await axiosInstance.get<ApiResponse<Task[]>>('/tasks');
    return response.data.data;
  },

  getTaskById: async (id: number): Promise<Task> => {
    const response = await axiosInstance.get<ApiResponse<Task>>(`/tasks/${id}`);
    return response.data.data;
  },

  getMyTasks: async (): Promise<Task[]> => {
    const response = await axiosInstance.get<ApiResponse<Task[]>>('/tasks/my-tasks');
    return response.data.data;
  },

  createTask: async (task: CreateTaskDto): Promise<Task> => {
    const response = await axiosInstance.post<ApiResponse<Task>>('/tasks', task);
    return response.data.data;
  },

  updateTask: async (id: number, task: UpdateTaskDto): Promise<Task> => {
    const response = await axiosInstance.put<ApiResponse<Task>>(`/tasks/${id}`, task);
    return response.data.data;
  },

  deleteTask: async (id: number): Promise<void> => {
    await axiosInstance.delete(`/tasks/${id}`);
  },

  filterTasks: async (status?: number, priority?: number, overdue?: boolean): Promise<Task[]> => {
    const params = new URLSearchParams();
    if (status !== undefined) params.append('status', status.toString());
    if (priority !== undefined) params.append('priority', priority.toString());
    if (overdue !== undefined) params.append('overdue', overdue.toString());
    
    const response = await axiosInstance.get<ApiResponse<Task[]>>(`/tasks/filter?${params}`);
    return response.data.data;
  },
};

export default taskService;