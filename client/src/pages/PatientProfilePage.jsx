import { useEffect, useState } from 'react';
import { getPatient, updatePatient, changePassword } from '../services/patientService';
import { getPatientKeyFromToken } from '../utils/authUtils';
import { handleApiError } from '../utils/errorUtil';

export default function PatientProfilePage() {
  const [patient, setPatient] = useState(null);
  const [originalPatient, setOriginalPatient] = useState(null);
  const [newPassword, setNewPassword] = useState('');
  const [currentPassword, setCurrentPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(true);
  const [updating, setUpdating] = useState(false);
  const [changingPassword, setChangingPassword] = useState(false);

  useEffect(() => {
    const patientKey = getPatientKeyFromToken();
    if (patientKey) {
      getPatient(patientKey)
        .then((data) => {
          const patientData = {
            ...data,
            PatientKey: data.PatientKey || patientKey,
            // וידוא שיש כתובת ברירת מחדל
            Address: data.Address || {
              CityName: '',
              StreetName: '',
              HouseNumber: '',
              PostalCode: ''
            }
          };
          setPatient(patientData);
          setOriginalPatient(JSON.parse(JSON.stringify(patientData))); // עותק עמוק
        })
        .catch(handleApiError)
        .finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
  }, []);

  if (loading) return <div style={{ padding: '20px' }}>Loading...</div>;
  if (!patient) return <div style={{ padding: '20px' }}>Patient not found</div>;

  const handleUpdate = async () => {
    if (!patient.PatientKey) {
      alert("Patient Key is missing.");
      return;
    }

    // בניית אובייקט עם רק השדות שהשתנו
    const updatedFields = {};
    let hasChanges = false;

    // בדיקת שינויים בפרטים האישיים
    if (patient.PatientName !== originalPatient.PatientName) {
      if (!patient.PatientName || patient.PatientName.trim().length < 2) {
        alert("Name must be at least 2 characters.");
        return;
      }
      updatedFields.PatientName = patient.PatientName.trim();
      hasChanges = true;
    }

    if (patient.Email !== originalPatient.Email) {
      if (!patient.Email || !isValidEmail(patient.Email)) {
        alert("Please enter a valid email address.");
        return;
      }
      updatedFields.Email = patient.Email.trim();
      hasChanges = true;
    }

    if (patient.Phone !== originalPatient.Phone) {
      if (!patient.Phone || patient.Phone.length < 10 || patient.Phone.length > 15) {
        alert("Phone must be between 10 and 15 characters.");
        return;
      }
      updatedFields.Phone = patient.Phone.trim();
      hasChanges = true;
    }

    // בדיקת שינויים בכתובת
    const addressChanged = 
      patient.Address.CityName !== originalPatient.Address.CityName ||
      patient.Address.StreetName !== originalPatient.Address.StreetName ||
      patient.Address.HouseNumber !== originalPatient.Address.HouseNumber ||
      patient.Address.PostalCode !== originalPatient.Address.PostalCode;

    if (addressChanged) {
      // בדיקה אם יש לפחות שדה כתובת אחד מלא
      const hasAddressData = patient.Address.CityName || patient.Address.StreetName || 
                            patient.Address.HouseNumber || patient.Address.PostalCode;

      if (hasAddressData) {
        // אם יש נתוני כתובת, בדוק שכולם מלאים ותקינים
        if (!patient.Address.CityName || patient.Address.CityName.trim().length < 2) {
          alert("City name must be at least 2 characters.");
          return;
        }
        
        if (!patient.Address.StreetName || patient.Address.StreetName.trim().length < 2) {
          alert("Street name must be at least 2 characters.");
          return;
        }
        
        if (!patient.Address.HouseNumber || patient.Address.HouseNumber < 1 || patient.Address.HouseNumber > 9999) {
          alert("House number must be between 1 and 9999.");
          return;
        }
        
        if (!patient.Address.PostalCode || patient.Address.PostalCode.trim().length < 4 || patient.Address.PostalCode.trim().length > 10) {
          alert("Postal code must be between 4 and 10 characters.");
          return;
        }

        updatedFields.Address = {
          CityName: patient.Address.CityName.trim(),
          StreetName: patient.Address.StreetName.trim(),
          HouseNumber: parseInt(patient.Address.HouseNumber),
          PostalCode: patient.Address.PostalCode.trim()
        };
        hasChanges = true;
      } else {
        // אם אין נתוני כתובת כלל, אפשר לשלוח null כדי למחוק כתובת קיימת
        updatedFields.Address = null;
        hasChanges = true;
      }
    }

    if (!hasChanges) {
      alert("No changes detected.");
      return;
    }

    // הוספת PatientKey תמיד
    updatedFields.PatientKey = patient.PatientKey;

    setUpdating(true);
    try {
      await updatePatient(updatedFields);
      alert('Profile updated successfully!');
      // עדכון הנתונים המקוריים
      setOriginalPatient(JSON.parse(JSON.stringify(patient)));
    } catch (error) {
      handleApiError(error);
    } finally {
      setUpdating(false);
    }
  };

  const handleChangePassword = async () => {
    if (!currentPassword) {
      alert("Please enter your current password.");
      return;
    }

    if (!newPassword || newPassword.length < 4 || newPassword.length > 15) {
      alert("New password must be between 4 and 15 characters.");
      return;
    }

    if (newPassword !== confirmPassword) {
      alert("New password and confirmation do not match.");
      return;
    }

    setChangingPassword(true);
    try {
      await changePassword({
        patientKey: patient.PatientKey,
        currentPassword,
        newPassword,
      });
      alert('Password changed successfully!');
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
    } catch (error) {
      handleApiError(error);
    } finally {
      setChangingPassword(false);
    }
  };

  const isValidEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const handleAddressChange = (field, value) => {
    setPatient(prev => ({
      ...prev,
      Address: {
        ...prev.Address,
        [field]: value
      }
    }));
  };

  const hasUnsavedChanges = () => {
    return JSON.stringify(patient) !== JSON.stringify(originalPatient);
  };

  return (
    <div style={{ padding: '20px', maxWidth: '600px', margin: '0 auto' }}>
      <h2>My Profile</h2>
      
      {/* Personal Information Section */}
      <div style={{ 
        border: '1px solid #ddd', 
        padding: '20px', 
        marginBottom: '20px', 
        borderRadius: '8px',
        backgroundColor: '#f9f9f9'
      }}>
        <h3>Personal Information</h3>
        <p style={{ color: '#666', marginBottom: '15px', fontSize: '14px' }}>
          Update only the fields you want to change. Leave others as they are.
        </p>
        
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            Full Name:
          </label>
          <input
            style={{ 
              width: '100%', 
              padding: '8px', 
              borderRadius: '4px', 
              border: '1px solid #ddd' 
            }}
            value={patient.PatientName || ''}
            onChange={(e) => setPatient({ ...patient, PatientName: e.target.value })}
            placeholder="Full Name"
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            Email:
          </label>
          <input
            style={{ 
              width: '100%', 
              padding: '8px', 
              borderRadius: '4px', 
              border: '1px solid #ddd' 
            }}
            type="email"
            value={patient.Email || ''}
            onChange={(e) => setPatient({ ...patient, Email: e.target.value })}
            placeholder="Email"
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            Phone:
          </label>
          <input
            style={{ 
              width: '100%', 
              padding: '8px', 
              borderRadius: '4px', 
              border: '1px solid #ddd' 
            }}
            value={patient.Phone || ''}
            onChange={(e) => setPatient({ ...patient, Phone: e.target.value })}
            placeholder="Phone (10-15 characters)"
          />
        </div>
      </div>

      {/* Address Section */}
      <div style={{ 
        border: '1px solid #ddd', 
        padding: '20px', 
        marginBottom: '20px', 
        borderRadius: '8px',
        backgroundColor: '#f9f9f9'
      }}>
        <h3>Address</h3>
        <p style={{ color: '#666', marginBottom: '15px', fontSize: '14px' }}>
          Fill all address fields or leave all empty. Partial address updates are not allowed.
        </p>
        
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            City:
          </label>
          <input
            style={{ 
              width: '100%', 
              padding: '8px', 
              borderRadius: '4px', 
              border: '1px solid #ddd' 
            }}
            value={patient.Address?.CityName || ''}
            onChange={(e) => handleAddressChange('CityName', e.target.value)}
            placeholder="City"
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            Street:
          </label>
          <input
            style={{ 
              width: '100%', 
              padding: '8px', 
              borderRadius: '4px', 
              border: '1px solid #ddd' 
            }}
            value={patient.Address?.StreetName || ''}
            onChange={(e) => handleAddressChange('StreetName', e.target.value)}
            placeholder="Street"
          />
        </div>

        <div style={{ display: 'flex', gap: '10px', marginBottom: '15px' }}>
          <div style={{ flex: 1 }}>
            <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
              House Number:
            </label>
            <input
              style={{ 
                width: '100%', 
                padding: '8px', 
                borderRadius: '4px', 
                border: '1px solid #ddd' 
              }}
              type="number"
              min="1"
              max="9999"
              value={patient.Address?.HouseNumber || ''}
              onChange={(e) => handleAddressChange('HouseNumber', parseInt(e.target.value) || '')}
              placeholder="House Number"
            />
          </div>

          <div style={{ flex: 1 }}>
            <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
              Postal Code:
            </label>
            <input
              style={{ 
                width: '100%', 
                padding: '8px', 
                borderRadius: '4px', 
                border: '1px solid #ddd' 
              }}
              value={patient.Address?.PostalCode || ''}
              onChange={(e) => handleAddressChange('PostalCode', e.target.value)}
              placeholder="Postal Code"
            />
          </div>
        </div>
      </div>

      <button 
        onClick={handleUpdate}
        disabled={updating || !hasUnsavedChanges()}
        style={{
          backgroundColor: updating ? '#ccc' : (!hasUnsavedChanges() ? '#6c757d' : '#007bff'),
          color: 'white',
          padding: '10px 20px',
          border: 'none',
          borderRadius: '4px',
          cursor: (updating || !hasUnsavedChanges()) ? 'not-allowed' : 'pointer',
          fontSize: '16px',
          marginBottom: '30px',
          width: '100%'
        }}
      >
        {updating ? 'Updating...' : (!hasUnsavedChanges() ? 'No Changes to Save' : 'Update Info')}
      </button>

      {/* Change Password Section */}
      <div style={{ 
        border: '1px solid #ddd', 
        padding: '20px', 
        borderRadius: '8px',
        backgroundColor: '#f9f9f9'
      }}>
        <h3>Change Password</h3>
        
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            Current Password:
          </label>
          <input
            style={{ 
              width: '100%', 
              padding: '8px', 
              borderRadius: '4px', 
              border: '1px solid #ddd' 
            }}
            type="password"
            placeholder="Current Password"
            value={currentPassword}
            onChange={(e) => setCurrentPassword(e.target.value)}
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            New Password:
          </label>
          <input
            style={{ 
              width: '100%', 
              padding: '8px', 
              borderRadius: '4px', 
              border: '1px solid #ddd' 
            }}
            type="password"
            placeholder="New Password (4-15 characters)"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            Confirm New Password:
          </label>
          <input
            style={{ 
              width: '100%', 
              padding: '8px', 
              borderRadius: '4px', 
              border: '1px solid #ddd' 
            }}
            type="password"
            placeholder="Confirm New Password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
          />
        </div>

        <button 
          onClick={handleChangePassword}
          disabled={changingPassword}
          style={{
            backgroundColor: changingPassword ? '#ccc' : '#28a745',
            color: 'white',
            padding: '10px 20px',
            border: 'none',
            borderRadius: '4px',
            cursor: changingPassword ? 'not-allowed' : 'pointer',
            fontSize: '16px',
            width: '100%'
          }}
        >
          {changingPassword ? 'Changing...' : 'Change Password'}
        </button>
      </div>
    </div>
  );
}