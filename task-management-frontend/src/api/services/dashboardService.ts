import axiosInstance from '../axiosConfig';
import { DashboardStats, ApiResponse } from '../../types';

export const dashboardService = {
  getStats: async (): Promise<DashboardStats> => {
    const response = await axiosInstance.get<ApiResponse<DashboardStats>>(
      '/dashboard/stats'
    );
    return response.data.data;
  },
};

export default dashboardService;