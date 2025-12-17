import React, { useEffect, useState } from 'react';
import { taskService } from '../api/services/taskService';
import { teamService } from '../api/services/teamService';
import { Task, UpdateTaskDto } from '../types';
import TaskCard from '../components/Tasks/TaskCard';
import toast from 'react-hot-toast';
import { useAuth } from '../context/AuthContext';
import userService from '../api/services/userService';

const MyTasks: React.FC = () => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [myTeams, setMyTeams] = useState<{ id: number; name: string; members: { userId: number; name: string }[] }[]>([]);
  const [selectedTeamId, setSelectedTeamId] = useState<number | null>(null);
  const [selectedMemberId, setSelectedMemberId] = useState<number | null>(null);
  const [allUsers, setAllUsers] = useState<{ id: number; name: string }[]>([]);
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
  const { user, isManager } = useAuth();

  useEffect(() => {
    loadTasks();
    if (isManager) {
      loadMyTeams();
      loadUsers();
    }
  }, []);

  const loadTasks = async () => {
    try {
      const data = await taskService.getMyTasks();
      setTasks(data);
    } catch (error) {
      toast.error('Failed to load your tasks');
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateStatus = async (taskId: number, newStatus: number) => {
    try {
      const updateData: UpdateTaskDto = { status: newStatus };
      await taskService.updateTask(taskId, updateData);
      toast.success('Task status updated');
      loadTasks();
    } catch (error) {
      toast.error('Failed to update task');
    }
  };

  const loadMyTeams = async () => {
    try {
      const allTeams = await teamService.getAllTeams();
      const mine = allTeams.filter(t => t.managerId === user?.id).map(t => ({ id: t.id, name: t.name, members: t.members.map(m => ({ userId: m.userId, name: m.name })) }));
      setMyTeams(mine);
    } catch {
      // ignore
    }
  };

  const assignToTeam = async () => {
    if (!selectedTask || !selectedTeamId) return;
    try {
      const updateData: UpdateTaskDto = { assignedToId: null };
      await taskService.updateTask(selectedTask.id, updateData);
      toast.success('Assigned to team');
      setSelectedTask(null);
      setSelectedTeamId(null);
      loadTasks();
    } catch (err: any) {
      toast.error(err?.response?.data?.message || 'Failed to assign');
    }
  };

  const assignToMember = async () => {
    if (!selectedTask || !selectedMemberId) return;
    try {
      await taskService.updateTask(selectedTask.id, { assignedToId: selectedMemberId });
      toast.success('Assigned to member');
      setSelectedTask(null);
      setSelectedMemberId(null);
      loadTasks();
    } catch (err: any) {
      toast.error(err?.response?.data?.message || 'Failed to assign');
    }
  };

  const loadUsers = async () => {
    try {
      const users = await userService.getUsers();
      setAllUsers(users.map(u => ({ id: u.id, name: `${u.firstName} ${u.lastName}` })));
    } catch {
      // ignore
    }
  };

  const assignToUser = async () => {
    if (!selectedTask || !selectedUserId) return;
    try {
      await taskService.updateTask(selectedTask.id, { assignedToId: selectedUserId });
      toast.success('Assigned to user');
      setSelectedTask(null);
      setSelectedUserId(null);
      loadTasks();
    } catch (err: any) {
      toast.error(err?.response?.data?.message || 'Failed to assign');
    }
  };

  const onChangeStatus = async (code: number) => {
    if (!selectedTask) return;
    await handleUpdateStatus(selectedTask.id, code);
    setSelectedTask(null);
  };

  if (loading) {
    return <div className="flex justify-center items-center h-64">Loading...</div>;
  }

  const groupedTasks = {
    todo: tasks.filter(t => t.status === 'ToDo'),
    inProgress: tasks.filter(t => t.status === 'InProgress'),
    done: tasks.filter(t => t.status === 'Done'),
  };

  return (
    <div>
      <h1 className="text-3xl font-bold text-gray-800 mb-6">My Tasks</h1>
      
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div>
          <div className="bg-yellow-100 px-4 py-2 rounded-t-lg">
            <h2 className="font-semibold text-yellow-800">To Do ({groupedTasks.todo.length})</h2>
          </div>
          <div className="space-y-4 mt-4">
            {groupedTasks.todo.map((task) => (
              <TaskCard
                key={task.id}
                task={task}
                onClick={() => setSelectedTask(task)}
              />
            ))}
          </div>
        </div>
        
        <div>
          <div className="bg-blue-100 px-4 py-2 rounded-t-lg">
            <h2 className="font-semibold text-blue-800">In Progress ({groupedTasks.inProgress.length})</h2>
          </div>
          <div className="space-y-4 mt-4">
            {groupedTasks.inProgress.map((task) => (
              <TaskCard
                key={task.id}
                task={task}
                onClick={() => setSelectedTask(task)}
              />
            ))}
          </div>
        </div>
        
        <div>
          <div className="bg-green-100 px-4 py-2 rounded-t-lg">
            <h2 className="font-semibold text-green-800">Done ({groupedTasks.done.length})</h2>
          </div>
          <div className="space-y-4 mt-4">
            {groupedTasks.done.map((task) => (
              <TaskCard
                key={task.id}
                task={task}
                onClick={() => setSelectedTask(task)}
              />
            ))}
          </div>
        </div>
      </div>
      
      {tasks.length === 0 && (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">You have no tasks assigned</p>
        </div>
      )}

      {selectedTask && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
          onClick={() => setSelectedTask(null)}
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

            <div className="mt-6">
              <div className="text-sm font-medium text-gray-700 mb-2">Update status</div>
              <div className="flex gap-2 flex-wrap">
                <button
                  className="px-3 py-2 rounded-md border text-sm hover:bg-gray-50"
                  onClick={() => onChangeStatus(0)}
                >
                  To Do
                </button>
                <button
                  className="px-3 py-2 rounded-md border text-sm hover:bg-gray-50"
                  onClick={() => onChangeStatus(1)}
                >
                  In Progress
                </button>
                <button
                  className="px-3 py-2 rounded-md border text-sm hover:bg-gray-50"
                  onClick={() => onChangeStatus(2)}
                >
                  Done
                </button>
              </div>
            </div>

            {isManager && (
              <div className="mt-6">
                <div className="text-sm font-medium text-gray-700 mb-2">Assign task</div>
                <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                  <div>
                    <label className="block text-xs text-gray-600 mb-1">Team</label>
                    <select
                      className="w-full px-3 py-2 border border-gray-300 rounded-md"
                      value={selectedTeamId ?? ''}
                      onChange={(e) => {
                        const val = e.target.value ? Number(e.target.value) : null;
                        setSelectedTeamId(val);
                        setSelectedMemberId(null);
                      }}
                    >
                      <option value="">Select team</option>
                      {myTeams.map(t => (
                        <option key={t.id} value={t.id}>{t.name}</option>
                      ))}
                    </select>
                    <button
                      className="mt-2 px-3 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-60"
                      onClick={assignToTeam}
                      disabled={!selectedTeamId}
                    >
                      Assign to team
                    </button>
                  </div>
                  <div>
                    <label className="block text-xs text-gray-600 mb-1">Team member</label>
                    <select
                      className="w-full px-3 py-2 border border-gray-300 rounded-md"
                      value={selectedMemberId ?? ''}
                      onChange={(e) => setSelectedMemberId(e.target.value ? Number(e.target.value) : null)}
                      disabled={!selectedTeamId}
                    >
                      <option value="">Select member</option>
                      {myTeams.find(t => t.id === selectedTeamId)?.members.map(m => (
                        <option key={m.userId} value={m.userId}>{m.name}</option>
                      ))}
                    </select>
                    <button
                      className="mt-2 px-3 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-60"
                      onClick={assignToMember}
                      disabled={!selectedMemberId}
                    >
                      Assign to member
                    </button>
                  </div>
                  <div>
                    <label className="block text-xs text-gray-600 mb-1">Any user</label>
                    <select
                      className="w-full px-3 py-2 border border-gray-300 rounded-md"
                      value={selectedUserId ?? ''}
                      onChange={(e) => setSelectedUserId(e.target.value ? Number(e.target.value) : null)}
                    >
                      <option value="">Select user</option>
                      {allUsers.map(u => (
                        <option key={u.id} value={u.id}>{u.name}</option>
                      ))}
                    </select>
                    <button
                      className="mt-2 px-3 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-60"
                      onClick={assignToUser}
                      disabled={!selectedUserId}
                    >
                      Assign to user
                    </button>
                  </div>
                </div>
              </div>
            )}

            <div className="mt-6 flex justify-end gap-3">
              <button
                onClick={() => setSelectedTask(null)}
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

export default MyTasks;