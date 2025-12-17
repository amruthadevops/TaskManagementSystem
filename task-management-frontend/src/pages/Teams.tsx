import React, { useEffect, useState } from 'react';
import { teamService } from '../api/services/teamService';
import { userService, SimpleUser } from '../api/services/userService';
import { Team } from '../types';
import { Users, Plus, X, UserPlus } from 'lucide-react';
import toast from 'react-hot-toast';
import { useAuth } from '../context/AuthContext';

const Teams: React.FC = () => {
  const [teams, setTeams] = useState<Team[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [showAddMember, setShowAddMember] = useState(false);
  const [selectedTeam, setSelectedTeam] = useState<Team | null>(null);
  const [availableUsers, setAvailableUsers] = useState<SimpleUser[]>([]);
  const [selectedUserToAdd, setSelectedUserToAdd] = useState<number | null>(null);
  const [addingMember, setAddingMember] = useState(false);
  const { user, isManager } = useAuth();

  useEffect(() => {
    loadTeams();
  }, []);

  const loadTeams = async () => {
    try {
      const data = await teamService.getAllTeams();
      setTeams(data);
    } catch (error) {
      toast.error('Failed to load teams');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateTeam = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) {
      toast.error('Team name is required');
      return;
    }
    try {
      setSubmitting(true);
      await teamService.createTeam({
        name: name.trim(),
        description: description.trim(),
        managerId: user?.id,
      });
      toast.success('Team created');
      setShowForm(false);
      setName('');
      setDescription('');
      loadTeams();
    } catch (err: any) {
      toast.error(err?.response?.data?.message || 'Failed to create team');
    } finally {
      setSubmitting(false);
    }
  };

  const handleAddMemberClick = async (team: Team) => {
    try {
      setSelectedTeam(team);
      const users = await userService.getUsers();
      // Filter out users already in the team
      const memberIds = new Set(team.members.map(m => m.userId));
      setAvailableUsers(users.filter(u => !memberIds.has(u.id) && u.isActive));
      setShowAddMember(true);
    } catch (error) {
      toast.error('Failed to load available users');
    }
  };

  const handleAddMember = async () => {
    if (!selectedTeam || !selectedUserToAdd) {
      toast.error('Please select a user');
      return;
    }
    try {
      setAddingMember(true);
      await teamService.addMember(selectedTeam.id, selectedUserToAdd);
      toast.success('Member added to team');
      setShowAddMember(false);
      setSelectedUserToAdd(null);
      setSelectedTeam(null);
      loadTeams();
    } catch (err: any) {
      toast.error(err?.response?.data?.message || 'Failed to add member');
    } finally {
      setAddingMember(false);
    }
  };

  if (loading) {
    return <div className="flex justify-center items-center h-64">Loading...</div>;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">Teams</h1>
        {isManager && (
          <button
            onClick={() => setShowForm(true)}
            className="flex items-center space-x-2 px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition"
          >
            <Plus className="w-4 h-4" />
            <span>Create Team</span>
          </button>
        )}
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {teams.map((team) => (
          <div key={team.id} className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center space-x-3 mb-4">
              <div className="bg-blue-500 p-3 rounded-full">
                <Users className="w-6 h-6 text-white" />
              </div>
              <div>
                <h3 className="text-lg font-semibold text-gray-800">{team.name}</h3>
                <p className="text-sm text-gray-500">Manager: {team.managerName}</p>
              </div>
            </div>
            
            <p className="text-gray-600 text-sm mb-4">{team.description}</p>
            
            <div className="border-t pt-4">
              <p className="text-sm text-gray-500 mb-2">
                Members: {team.members.length}
              </p>
              <div className="flex -space-x-2 mb-4">
                {team.members.slice(0, 5).map((member, idx) => (
                  <div
                    key={idx}
                    className="w-8 h-8 rounded-full bg-gray-300 border-2 border-white flex items-center justify-center text-xs font-semibold"
                    title={member.name}
                  >
                    {member.name.split(' ').map(n => n[0]).join('')}
                  </div>
                ))}
                {team.members.length > 5 && (
                  <div className="w-8 h-8 rounded-full bg-gray-200 border-2 border-white flex items-center justify-center text-xs">
                    +{team.members.length - 5}
                  </div>
                )}
              </div>
              
              {isManager && team.managerId === user?.id && (
                <button
                  onClick={() => handleAddMemberClick(team)}
                  className="w-full flex items-center justify-center space-x-2 px-3 py-2 bg-green-50 text-green-700 rounded-md hover:bg-green-100 transition text-sm"
                >
                  <UserPlus className="w-4 h-4" />
                  <span>Add Member</span>
                </button>
              )}
            </div>
          </div>
        ))}
      </div>
      
      {teams.length === 0 && (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">No teams found</p>
        </div>
      )}

      {showForm && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
          onClick={() => setShowForm(false)}
        >
          <div
            className="bg-white w-full max-w-lg mx-4 rounded-lg shadow-xl p-6"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex items-start justify-between mb-4">
              <h2 className="text-xl font-semibold text-gray-800">Create Team</h2>
              <button
                className="p-2 rounded-md hover:bg-gray-100"
                onClick={() => setShowForm(false)}
                aria-label="Close"
              >
                <X className="w-5 h-5 text-gray-600" />
              </button>
            </div>
            <form onSubmit={handleCreateTeam} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Team Name</label>
                <input
                  type="text"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="e.g., Platform Team"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Description</label>
                <textarea
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  rows={3}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="What this team is responsible for"
                />
              </div>
              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setShowForm(false)}
                  className="px-4 py-2 rounded-md bg-gray-100 text-gray-700 hover:bg-gray-200"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  className="px-4 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-60"
                >
                  {submitting ? 'Creating…' : 'Create Team'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showAddMember && selectedTeam && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
          onClick={() => setShowAddMember(false)}
        >
          <div
            className="bg-white w-full max-w-lg mx-4 rounded-lg shadow-xl p-6"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex items-start justify-between mb-4">
              <h2 className="text-xl font-semibold text-gray-800">Add Member to {selectedTeam.name}</h2>
              <button
                className="p-2 rounded-md hover:bg-gray-100"
                onClick={() => setShowAddMember(false)}
                aria-label="Close"
              >
                <X className="w-5 h-5 text-gray-600" />
              </button>
            </div>
            
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Select User</label>
                {availableUsers.length > 0 ? (
                  <div className="border border-gray-300 rounded-lg max-h-96 overflow-y-auto">
                    {availableUsers.map((user) => (
                      <label
                        key={user.id}
                        className={`flex items-center p-3 border-b cursor-pointer hover:bg-gray-50 ${
                          selectedUserToAdd === user.id ? 'bg-blue-50' : ''
                        }`}
                      >
                        <input
                          type="radio"
                          name="user"
                          value={user.id}
                          checked={selectedUserToAdd === user.id}
                          onChange={(e) => setSelectedUserToAdd(Number(e.target.value))}
                          className="w-4 h-4 text-blue-600"
                        />
                        <div className="ml-3">
                          <p className="font-bold text-gray-800">
                            {user.firstName} {user.lastName}
                          </p>
                          <p className="text-xs text-gray-600">{user.role}</p>
                        </div>
                      </label>
                    ))}
                  </div>
                ) : (
                  <p className="text-gray-500 text-sm p-3 border border-gray-200 rounded-lg">
                    No available users to add (all active users are already in this team)
                  </p>
                )}
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => setShowAddMember(false)}
                  className="px-4 py-2 rounded-md bg-gray-100 text-gray-700 hover:bg-gray-200"
                >
                  Cancel
                </button>
                <button
                  onClick={handleAddMember}
                  disabled={addingMember || !selectedUserToAdd}
                  className="px-4 py-2 rounded-md bg-green-600 text-white hover:bg-green-700 disabled:opacity-60"
                >
                  {addingMember ? 'Adding…' : 'Add Member'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Teams;
