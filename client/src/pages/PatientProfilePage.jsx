import  { useEffect, useState } from 'react';
import { getPatient, updatePatient, changePassword } from '../services/patientService';
 import { getPatientIdFromToken } from '../utils/authUtils';
 
export default function PatientProfilePage() {
  const [patient, setPatient] = useState(null);
  const [newPassword, setNewPassword] = useState('');

  useEffect(() => {
  
    const patientId = getPatientIdFromToken();
    if(patientId) 
    getPatient(patientId).then(setPatient).catch(() => alert('Failed to load profile'));
  }, []);

  if (!patient) return <div>Loading...</div>;

  const handleUpdate = async () => {
    try {
      await updatePatient(patient);
      alert('Profile updated successfully!');
    } catch {
      alert('Failed to update profile.');
    }
  };

  const handleChangePassword = async () => {
    try {
      await changePassword({ id: patient.id, password: newPassword });
      alert('Password changed successfully!');
    } catch {
      alert('Failed to change password.');
    }
  };

  return (
    <div>
      <h2>My Profile</h2>
      <input
        value={patient.name || ''}
        onChange={(e) => setPatient({ ...patient, name: e.target.value })}
        placeholder="Full Name"
      />
      <button onClick={handleUpdate}>Update Info</button>

      <input
        type="password"
        placeholder="New Password"
        value={newPassword}
        onChange={(e) => setNewPassword(e.target.value)}
      />
      <button onClick={handleChangePassword}>Change Password</button>
    </div>
  );
}
