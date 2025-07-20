import { useEffect, useState } from 'react';
import { getPatient, updatePatient, changePassword, getCities, getStreetsByCity } from '../services/patientService';
import { getPatientKeyFromToken } from '../utils/authUtils';
import { handleApiError } from '../utils/errorUtil';

export default function PatientProfilePage() {
  // נתוני המטופל
  const [patientKey, setPatientKey] = useState(null);
  const [patientName, setPatientName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
 
  // כתובת
  const [cityId, setCityId] = useState('');
  const [streetId, setStreetId] = useState('');
  const [houseNumber, setHouseNumber] = useState('');
  const [postalCode, setPostalCode] = useState('');
 
  // נתונים מקוריים להשוואה
  const [originalData, setOriginalData] = useState({});
 
  // רשימות
  const [cities, setCities] = useState([]);
  const [streets, setStreets] = useState([]);
 
  // סיסמה
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
 
  // מצבי טעינה
  const [loading, setLoading] = useState(true);
  const [updating, setUpdating] = useState(false);
  const [changingPassword, setChangingPassword] = useState(false);
  const [loadingStreets, setLoadingStreets] = useState(false);
  const [hasChanges, setHasChanges] = useState(false);

  useEffect(() => {
    loadInitialData();
  }, []);

  // עדכון מצב השינויים כאשר נתונים משתנים
  useEffect(() => {
    checkForChanges();
  }, [patientName, email, phone, cityId, streetId, houseNumber, postalCode, originalData]);

  const loadInitialData = async () => {
    const patientKeyFromToken = getPatientKeyFromToken();
    if (!patientKeyFromToken) {
      console.error('❌ No patient key found');
      setLoading(false);
      return;
    }

    try {
      console.log('🚀 Loading data for patient:', patientKeyFromToken);
     
      const [patientData, citiesData] = await Promise.all([
        getPatient(patientKeyFromToken),
        getCities()
      ]);
     
      console.log('✅ Patient data:', patientData);
      console.log('✅ Cities data:', citiesData);
     
      // שמירת נתוני המטופל
      setPatientKey(patientData.patientKey);
      setPatientName(patientData.PatientName || '');
      setEmail(patientData.Email || '');
      setPhone(patientData.Phone || '');
     
      // שמירת נתוני כתובת
      if (patientData.Address) {
        setCityId(patientData.Address.CityId || '');
        setStreetId(patientData.Address.StreetId || '');
        setHouseNumber(patientData.Address.HouseNumber || '');
        setPostalCode(patientData.Address.PostalCode || '');
       
        // טעינת רחובות אם יש עיר
        if (patientData.Address.CityId) {
          const streetsData = await getStreetsByCity(patientData.Address.CityId);
          setStreets(streetsData.data || []);
        }
      }
     
      setCities(citiesData.data || []);
     
      // שמירת נתונים מקוריים להשוואה
      const original = {
        PatientName: patientData.PatientName || '',
        Email: patientData.Email || '',
        Phone: patientData.Phone || '',
        CityId: patientData.Address?.CityId || '',
        StreetId: patientData.Address?.StreetId || '',
        HouseNumber: patientData.Address?.HouseNumber || '',
        PostalCode: patientData.Address?.PostalCode || ''
      };
      setOriginalData(original);
     
      console.log('✅ Original data saved:', original);
     
    } catch (error) {
      console.error('❌ Error loading data:', error);
      handleApiError(error);
    } finally {
      setLoading(false);
    }
  };

  const handleCityChange = async (newCityId) => {
    console.log('🏙️ City changed to:', newCityId);
    setCityId(newCityId);
    setStreetId(''); // איפוס רחוב
   
    if (newCityId) {
      setLoadingStreets(true);
      try {
        const streetsData = await getStreetsByCity(newCityId);
        setStreets(streetsData.data || []);
      } catch (error) {
        console.error('❌ Error loading streets:', error);
        setStreets([]);
      } finally {
        setLoadingStreets(false);
      }
    } else {
      setStreets([]);
    }
  };

  const getCurrentData = () => {
    return {
      PatientName: patientName.trim(),
      Email: email.trim(),
      Phone: phone.trim(),
      CityId: cityId,
      StreetId: streetId,
      HouseNumber: houseNumber,
      PostalCode: postalCode.trim()
    };
  };

  const checkForChanges = () => {
    if (Object.keys(originalData).length === 0) return;
   
    const current = getCurrentData();
    const changed = {
      name: current.PatientName !== originalData.PatientName,
      email: current.Email !== originalData.Email,
      phone: current.Phone !== originalData.Phone,
      city: current.CityId !== originalData.CityId,
      street: current.StreetId !== originalData.StreetId,
      house: current.HouseNumber !== originalData.HouseNumber,
      postal: current.PostalCode !== originalData.PostalCode
    };
   
    const hasAnyChange = Object.values(changed).some(Boolean);
    setHasChanges(hasAnyChange);
  };

  const handleUpdate = async () => {
    console.log('🚀 Starting update...');
   
    if (!patientKey) {
      alert('Patient key missing');
      return;
    }

    const current = getCurrentData();
    console.log('📊 Current data:', current);
    console.log('📊 Original data:', originalData);

    // בניית אובייקט עדכון - רק שדות שהשתנו
    const updateData = {
      PatientKey: patientKey
    };

    // הוספת שדות שהשתנו
    if (current.PatientName !== originalData.PatientName) {
      if (!current.PatientName || current.PatientName.length < 2) {
        alert('Name must be at least 2 characters');
        return;
      }
      updateData.PatientName = current.PatientName;
    }

    if (current.Email !== originalData.Email) {
      if (!current.Email || !isValidEmail(current.Email)) {
        alert('Please enter a valid email');
        return;
      }
      updateData.Email = current.Email;
    }

    if (current.Phone !== originalData.Phone) {
      if (!current.Phone || current.Phone.length < 10) {
        alert('Phone must be at least 10 characters');
        return;
      }
      updateData.Phone = current.Phone;
    }

    // בדיקת שינויים בכתובת
    const addressChanged =
      current.CityId !== originalData.CityId ||
      current.StreetId !== originalData.StreetId ||
      current.HouseNumber !== originalData.HouseNumber ||
      current.PostalCode !== originalData.PostalCode;

    if (addressChanged) {
      // אם יש נתוני כתובת - חייבים להיות כולם או כולם ריקים
      const hasAnyAddressData = current.CityId || current.StreetId || current.HouseNumber || current.PostalCode;
      const hasAllAddressData = current.CityId && current.StreetId && current.HouseNumber && current.PostalCode;
     
      if (hasAnyAddressData && !hasAllAddressData) {
        alert('Please fill all address fields or leave all empty');
        return;
      }
     
      if (hasAllAddressData) {
        if (current.HouseNumber < 1 || current.HouseNumber > 9999) {
          alert('House number must be between 1 and 9999');
          return;
        }
       
        if (current.PostalCode.length < 4 || current.PostalCode.length > 10) {
          alert('Postal code must be between 4 and 10 characters');
          return;
        }

        updateData.Address = {
          CityId: parseInt(current.CityId),
          StreetId: parseInt(current.StreetId),
          HouseNumber: parseInt(current.HouseNumber),
          PostalCode: current.PostalCode
        };
      }
    }

    console.log('📤 Sending update:', updateData);
   
    setUpdating(true);
    try {
      const response = await updatePatient(updateData);
      console.log('✅ Update successful:', response);
     
      // עדכון הנתונים המקוריים
      setOriginalData(current);
      setHasChanges(false);
      alert('Profile updated successfully!');
     
    } catch (error) {
      console.error('❌ Update failed:', error);
      const errorMsg = error.response?.data?.message || error.message || 'Update failed';
      alert(`Update failed: ${errorMsg}`);
    } finally {
      setUpdating(false);
    }
  };

  const handleChangePassword = async () => {
    if (!currentPassword || !newPassword || !confirmPassword) {
      alert('Please fill all password fields');
      return;
    }

    if (newPassword.length < 4 || newPassword.length > 15) {
      alert('Password must be between 4 and 15 characters');
      return;
    }

    if (newPassword !== confirmPassword) {
      alert('Passwords do not match');
      return;
    }

    setChangingPassword(true);
    try {
      await changePassword({
        PatientKey: patientKey,
        CurrentPassword: currentPassword,
        NewPassword: newPassword,
        ConfirmPassword: confirmPassword
      });
     
      alert('Password changed successfully!');
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
    } catch (error) {
      console.error('❌ Password change failed:', error);
      const errorMsg = error.response?.data?.message || error.message || 'Password change failed';
      alert(`Password change failed: ${errorMsg}`);
    } finally {
      setChangingPassword(false);
    }
  };

  const isValidEmail = (email) => {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  };

  if (loading) {
    return <div style={{ padding: '20px' }}>Loading...</div>;
  }

  return (
    <div style={{ padding: '20px', maxWidth: '600px', margin: '0 auto' }}>
      <h2>My Profile</h2>
     
      {/* Personal Information */}
      <div style={{
        border: '1px solid #ddd',
        padding: '20px',
        marginBottom: '20px',
        borderRadius: '8px',
        backgroundColor: '#f9f9f9'
      }}>
        <h3>Personal Information</h3>
       
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
            value={patientName}
            onChange={(e) => setPatientName(e.target.value)}
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
            value={email}
            onChange={(e) => setEmail(e.target.value)}
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
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
            placeholder="Phone"
          />
        </div>
      </div>

      {/* Address */}
      <div style={{
        border: '1px solid #ddd',
        padding: '20px',
        marginBottom: '20px',
        borderRadius: '8px',
        backgroundColor: '#f9f9f9'
      }}>
        <h3>Address</h3>
       
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            City:
          </label>
          <select
            style={{
              width: '100%',
              padding: '8px',
              borderRadius: '4px',
              border: '1px solid #ddd'
            }}
            value={cityId}
            onChange={(e) => handleCityChange(e.target.value)}
          >
            <option value="">-- Select City --</option>
            {cities.map((city, index) => (
              <option key={`city-${city.CityId}-${index}`} value={city.CityId}>
                {city.Name}
              </option>
            ))}
          </select>
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
            Street:
          </label>
          <select
            style={{
              width: '100%',
              padding: '8px',
              borderRadius: '4px',
              border: '1px solid #ddd'
            }}
            value={streetId}
            onChange={(e) => setStreetId(e.target.value)}
            disabled={!cityId || loadingStreets}
          >
            <option value="">-- Select Street --</option>
            {streets.map((street, index) => (
              <option key={`street-${street.StreetId}-${index}`} value={street.StreetId}>
                {street.Name}
              </option>
            ))}
          </select>
          {loadingStreets && <div style={{ fontSize: '12px', color: '#666' }}>Loading streets...</div>}
        </div>

        <div style={{ display: 'flex', gap: '10px' }}>
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
              value={houseNumber}
              onChange={(e) => setHouseNumber(e.target.value)}
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
              value={postalCode}
              onChange={(e) => setPostalCode(e.target.value)}
              placeholder="Postal Code"
            />
          </div>
        </div>
      </div>

      {/* עדכון כפתור השמירה */}
      <button
        onClick={handleUpdate}
        disabled={updating || !hasChanges}
        style={{
          backgroundColor: updating ? '#ccc' : (!hasChanges ? '#6c757d' : '#007bff'),
          color: 'white',
          padding: '10px 20px',
          border: 'none',
          borderRadius: '4px',
          cursor: (updating || !hasChanges) ? 'not-allowed' : 'pointer',
          fontSize: '16px',
          marginBottom: '30px',
          width: '100%'
        }}
      >
        {updating ? 'Updating...' : (!hasChanges ? 'No Changes to Save' : 'Update Info')}
      </button>

      {/* Password Change */}
      <div style={{
        border: '1px solid #ddd',
        padding: '20px',
        borderRadius: '8px',
        backgroundColor: '#f9f9f9'
      }}>
        <h3>Change Password</h3>
       
        <div style={{ marginBottom: '15px' }}>
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