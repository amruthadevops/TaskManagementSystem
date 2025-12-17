import React, { useEffect, useState } from 'react';
import { taskService } from '../api/services/taskService';
import { Task } from '../types';
import TaskCard from '../components/Tasks/TaskCard';
import TaskForm from '../components/Tasks/TaskForm';
import { Plus, Filter } from 'lucide-react';
import userService from '../api/services/userService';
import { useAuth } from '../context/AuthContext';
import toast from 'react-hot-toast';

const Tasks: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const { isManager, isAdmin } = useAuth();
  const [managers, setManagers] = useState<{id: number; name: string}[]>([]);
  const [assignManagerId, setAssignManagerId] = useState<number | null>(null);

  useEffect(() => {
    loadTasks();
    if (isAdmin) loadManagers();
  }, []);

  const loadTasks = async () => {
    try {
      const data = await taskService.getAllTasks();
      setTasks(data);
    } catch (error) {
      toast.error('Failed to load tasks');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateTask = async (taskData: any) => {
    try {
      await taskService.createTask(taskData);
      toast.success('Task created successfully');
      setShowForm(false);
      loadTasks();
    } catch (error) {
      toast.error('Failed to create task');
    }
  };

  const loadManagers = async () => {
    try {
      const list = await userService.getManagers();
      setManagers(list.map(u => ({ id: u.id, name: `${u.firstName} ${u.lastName}` })));
    } catch (e) {
      // ignore
    }
  };

  const assignToManager = async () => {
    if (!selectedTask || !assignManagerId) return;
    try {
      await taskService.updateTask(selectedTask.id, { assignedToId: assignManagerId });
      toast.success('Assigned to manager');
      closeDetails();
      loadTasks();
    } catch (err: any) {
      toast.error(err?.response?.data?.message || 'Failed to assign');
    }
  };

  const closeDetails = () => setSelectedTask(null);

  if (loading) {
    return <div className="flex justify-center items-center h-64">Loading...</div>;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">All Tasks</h1>
        <div className="flex space-x-3">
          <button className="flex items-center space-x-2 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition">
            <Filter className="w-4 h-4" />
            <span>Filter</span>
          </button>
          {isManager && (
            <button
              onClick={() => setShowForm(true)}
              className="flex items-center space-x-2 px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition"
            >
              <Plus className="w-4 h-4" />
              <span>New Task</span>
            </button>
          )}
        </div>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {tasks.map((task) => (
          <TaskCard
            key={task.id}
            task={task}
            onClick={() => setSelectedTask(task)}
          />
        ))}
      </div>
      
      {tasks.length === 0 && (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">No tasks found</p>
        </div>
      )}
      
      {showForm && (
        <TaskForm
          onSubmit={handleCreateTask}
          onClose={() => setShowForm(false)}
        />
      )}

      {selectedTask && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
          onClick={closeDetails}
        >
          <div
            className="bg-white w-full max-w-xl mx-4 rounded-lg shadow-xl p-6"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex items-start justify-between mb-4">
              <div>
                <h2 className="text-2xl font-semibold text-gray-800">{selectedTask.title}</h2>
                <p className="text-gray-600 mt-1">{selectedTask.description}</p>
              </div>
              <div className="text-right">
                <span className="inline-block px-3 py-1 text-xs font-medium rounded-full bg-gray-100 text-gray-700">
                  {selectedTask.status}
                </span>
                <div className="mt-2 text-sm text-gray-500">Priority: {selectedTask.priority}</div>
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 text-sm text-gray-700">
              <div>
                <span className="font-medium">Assigned to: </span>
                {selectedTask.assignedToName || 'Unassigned'}
              </div>
              <div>
                <span className="font-medium">Team: </span>
                {selectedTask.teamName || '—'}
              </div>
              <div>
                <span className="font-medium">Due date: </span>
                {selectedTask.dueDate ? new Date(selectedTask.dueDate).toLocaleDateString() : '—'}
              </div>
              <div>
                <span className="font-medium">Created by: </span>
                {selectedTask.createdByName}
              </div>
              <div>
                <span className="font-medium">Created at: </span>
                {new Date(selectedTask.createdAt).toLocaleString()}
              </div>
              {selectedTask.updatedAt && (
                <div>
                  <span className="font-medium">Updated at: </span>
                  {new Date(selectedTask.updatedAt).toLocaleString()}
                </div>
              )}
            </div>

            <div className="mt-6 flex justify-end gap-3">
              {isAdmin && (
                <div className="flex-1">
                  <div className="text-sm font-medium text-gray-700 mb-2">Assign to manager</div>
                  <div className="flex gap-2">
                    <select
                      className="flex-1 px-3 py-2 border border-gray-300 rounded-md"
                      value={assignManagerId ?? ''}
                      onChange={(e) => setAssignManagerId(e.target.value ? Number(e.target.value) : null)}
                    >
                      <option value="">Select manager</option>
                      {managers.map(m => (
                        <option key={m.id} value={m.id}>{m.name}</option>
                      ))}
                    </select>
                    <button
                      onClick={assignToManager}
                      disabled={!assignManagerId}
                      className="px-4 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-60"
                    >
                      Assign
                    </button>
                  </div>
                </div>
              )}
              <button
                onClick={closeDetails}
                className="px-4 py-2 rounded-md bg-gray-100 text-gray-700 hover:bg-gray-200"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Tasks;