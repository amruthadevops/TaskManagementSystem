import axiosInstance from '../axiosConfig';
import { Comment, CreateCommentDto, ApiResponse } from '../../types';

export const commentService = {
  getTaskComments: async (taskId: number): Promise<Comment[]> => {
    const response = await axiosInstance.get<ApiResponse<Comment[]>>(
      `/comments/task/${taskId}`
    );
    return response.data.data;
  },

  addComment: async (comment: CreateCommentDto): Promise<Comment> => {
    const response = await axiosInstance.post<ApiResponse<Comment>>(
      '/comments',
      comment
    );
    return response.data.data;
  },

  deleteComment: async (id: number): Promise<void> => {
    await axiosInstance.delete(`/comments/${id}`);
  },
};

export default commentService;