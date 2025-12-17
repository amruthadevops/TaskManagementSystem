import React, { useEffect, useState } from 'react';
import { dashboardService } from '../api/services/dashboardService';
import { DashboardStats } from '../types';
import { CheckCircle, Clock, AlertCircle, ListTodo, TrendingUp } from 'lucide-react';
import toast from 'react-hot-toast';

const Dashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      const data = await dashboardService.getStats();
      setStats(data);
    } catch (error) {
      toast.error('Failed to load dashboard stats');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="flex justify-center items-center h-64">Loading...</div>;
  }

  const statCards = [
    {
      title: 'Total Tasks',
      value: stats?.totalTasks || 0,
      icon: ListTodo,
      color: 'bg-blue-500',
    },
    {
      title: 'To Do',
      value: stats?.toDoTasks || 0,
      icon: Clock,
      color: 'bg-yellow-500',
    },
    {
      title: 'In Progress',
      value: stats?.inProgressTasks || 0,
      icon: TrendingUp,
      color: 'bg-purple-500',
    },
    {
      title: 'Completed',
      value: stats?.doneTasks || 0,
      icon: CheckCircle,
      color: 'bg-green-500',
    },
    {
      title: 'Overdue',
      value: stats?.overdueTasks || 0,
      icon: AlertCircle,
      color: 'bg-red-500',
    },
    {
      title: 'High Priority',
      value: stats?.highPriorityTasks || 0,
      icon: AlertCircle,
      color: 'bg-orange-500',
    },
  ];

  return (
    <div>
      <h1 className="text-3xl font-bold text-gray-800 mb-6">Dashboard</h1>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {statCards.map((card, index) => (
          <div
            key={index}
            className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition"
          >
            <div className="flex items-center justify-between">
              <div>
                <p className="text-gray-500 text-sm font-medium mb-1">{card.title}</p>
                <p className="text-3xl font-bold text-gray-800">{card.value}</p>
              </div>
              <div className={`${card.color} p-3 rounded-full`}>
                <card.icon className="w-6 h-6 text-white" />
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Dashboard;