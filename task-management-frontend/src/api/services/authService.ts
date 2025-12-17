import axiosInstance from '../axiosConfig';
import { LoginDto, RegisterDto, AuthResponse, ApiResponse } from '../../types';

export const authService = {
  login: async (credentials: LoginDto): Promise<AuthResponse> => {
    const response = await axiosInstance.post<ApiResponse<AuthResponse>>(
      '/auth/login',
      credentials
    );
    return response.data.data;
  },

  register: async (userData: RegisterDto): Promise<AuthResponse> => {
    const response = await axiosInstance.post<ApiResponse<AuthResponse>>(
      '/auth/register',
      userData
    );
    return response.data.data;
  },
};

export default authService;