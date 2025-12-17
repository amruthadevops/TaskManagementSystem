import React from 'react';
import { Task } from '../../types';
import { Calendar, User, AlertCircle } from 'lucide-react';
import { format } from 'date-fns';

interface TaskCardProps {
  task: Task;
  onClick: () => void;
}

const TaskCard: React.FC<TaskCardProps> = ({ task, onClick }) => {
  const statusColors = {
    ToDo: 'bg-yellow-100 text-yellow-800',
    InProgress: 'bg-blue-100 text-blue-800',
    Done: 'bg-green-100 text-green-800',
  };

  const priorityColors = {
    Low: 'text-gray-500',
    Medium: 'text-yellow-500',
    High: 'text-orange-500',
    Critical: 'text-red-500',
  };

  return (
    <div
      onClick={onClick}
      className="bg-white rounded-lg shadow-md p-4 hover:shadow-lg transition cursor-pointer border-l-4 border-blue-500"
    >
      <div className="flex justify-between items-start mb-3">
        <h3 className="text-lg font-semibold text-gray-800 flex-1">{task.title}</h3>
        <span
          className={`px-3 py-1 rounded-full text-xs font-medium ${
            statusColors[task.status as keyof typeof statusColors]
          }`}
        >
          {task.status}
        </span>
      </div>
      
      <p className="text-gray-600 text-sm mb-4 line-clamp-2">{task.description}</p>
      
      <div className="flex items-center justify-between text-sm">
        <div className="flex items-center space-x-4">
          <div className="flex items-center text-gray-500">
            <User className="w-4 h-4 mr-1" />
            <span>{task.assignedToName || 'Unassigned'}</span>
          </div>
          {task.dueDate && (
            <div className="flex items-center text-gray-500">
              <Calendar className="w-4 h-4 mr-1" />
              <span>{format(new Date(task.dueDate), 'MMM dd')}</span>
            </div>
          )}
        </div>
        <div className={`flex items-center ${priorityColors[task.priority as keyof typeof priorityColors]}`}>
          <AlertCircle className="w-4 h-4 mr-1" />
          <span className="font-medium">{task.priority}</span>
        </div>
      </div>
    </div>
  );
};

export default TaskCard;