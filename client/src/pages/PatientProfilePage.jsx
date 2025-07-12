import { useEffect, useState } from 'react';
import { getPatient, updatePatient, changePassword } from '../services/patientService';
import { getPatientKeyFromToken } from '../utils/authUtils';
import { handleApiError } from '../utils/errorUtil';

export default function PatientProfilePage() {
  const [patient, setPatient] = useState(null);
  const [newPassword, setNewPassword] = useState('');
  const [currentPassword, setCurrentPassword] = useState('');

  useEffect(() => {
    const patientKey = getPatientKeyFromToken();
    if (patientKey) {
      getPatient(patientKey)
        .then((data) => {
          setPatient({
            ...data,
            PatientKey: data.PatientKey || patientKey,
          });
        })
        .catch(handleApiError);
    }
  }, []);

  if (!patient) return <div>Loading...</div>;

  const handleUpdate = async () => {
    if (!patient.PatientKey) {
      alert("Patient Key is missing.");
      return;
    }

    if (!patient.Phone || patient.Phone.length < 10 || patient.Phone.length > 15) {
      alert("Phone must be between 10 and 15 characters.");
      return;
    }

    try {
      await updatePatient(patient);
      alert('Profile updated successfully!');
    } catch (error) {
      handleApiError(error);
    }
  };

  const handleChangePassword = async () => {
    try {
      await changePassword({
        patientKey: patient.PatientKey,
        currentPassword,
        newPassword,
      });
      alert('Password changed successfully!');
      setCurrentPassword('');
      setNewPassword('');
    } catch (error) {
      handleApiError(error);
    }
  };

  return (
    <div>
      <h2>My Profile</h2>
      <input
        value={patient.PatientName || ''}
        onChange={(e) => setPatient({ ...patient, PatientName: e.target.value })}
        placeholder="Full Name"
      />
      <input
        value={patient.Email || ''}
        onChange={(e) => setPatient({ ...patient, Email: e.target.value })}
        placeholder="Email"
      />
      <input
        value={patient.Phone || ''}
        onChange={(e) => setPatient({ ...patient, Phone: e.target.value })}
        placeholder="Phone"
      />

      {patient.Address && (
        <>
          <input
            value={patient.Address.CityName || ''}
            onChange={(e) =>
              setPatient({
                ...patient,
                Address: { ...patient.Address, CityName: e.target.value },
              })
            }
            placeholder="City"
          />
          <input
            value={patient.Address.StreetName || ''}
            onChange={(e) =>
              setPatient({
                ...patient,
                Address: { ...patient.Address, StreetName: e.target.value },
              })
            }
            placeholder="Street"
          />
          <input
            value={patient.Address.HouseNumber || ''}
            onChange={(e) =>
              setPatient({
                ...patient,
                Address: { ...patient.Address, HouseNumber: e.target.value },
              })
            }
            placeholder="House Number"
          />
          <input
            value={patient.Address.PostalCode || ''}
            onChange={(e) =>
              setPatient({
                ...patient,
                Address: { ...patient.Address, PostalCode: e.target.value },
              })
            }
            placeholder="Postal Code"
          />
        </>
      )}

      <button onClick={handleUpdate}>Update Info</button>

      <h3>Change Password</h3>
      <input
        type="password"
        placeholder="Current Password"
        value={currentPassword}
        onChange={(e) => setCurrentPassword(e.target.value)}
      />
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