import axiosInstance from '../axiosConfig';
import { Team, CreateTeamDto, ApiResponse } from '../../types';

export const teamService = {
  getAllTeams: async (): Promise<Team[]> => {
    const response = await axiosInstance.get<ApiResponse<Team[]>>('/teams');
    return response.data.data;
  },

  getTeamById: async (id: number): Promise<Team> => {
    const response = await axiosInstance.get<ApiResponse<Team>>(`/teams/${id}`);
    return response.data.data;
  },

  createTeam: async (team: CreateTeamDto): Promise<Team> => {
    const response = await axiosInstance.post<ApiResponse<Team>>('/teams', team);
    return response.data.data;
  },

  addMember: async (teamId: number, userId: number): Promise<void> => {
    await axiosInstance.post(`/teams/${teamId}/members/${userId}`);
  },

  removeMember: async (teamId: number, userId: number): Promise<void> => {
    await axiosInstance.delete(`/teams/${teamId}/members/${userId}`);
  },
};

export default teamService;