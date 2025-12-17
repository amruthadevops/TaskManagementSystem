import axiosInstance from '../axiosConfig';
import { ApiResponse } from '../../types';

export interface SimpleUser {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  isActive: boolean;
}

export const userService = {
  getManagers: async (): Promise<SimpleUser[]> => {
    const response = await axiosInstance.get<ApiResponse<SimpleUser[]>>('/users/managers');
    return response.data.data;
  },
  getUsers: async (): Promise<SimpleUser[]> => {
    const response = await axiosInstance.get<ApiResponse<SimpleUser[]>>('/users');
    return response.data.data;
  },
};

export default userService;
