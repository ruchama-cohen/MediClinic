// import { useEffect, useState } from 'react';
// import { getPatient, updatePatient, changePassword, getCities, getStreetsByCity } from '../services/patientService';
// import { getPatientKeyFromToken } from '../utils/authUtils';
// import { handleApiError } from '../utils/errorUtil';

// export default function PatientProfilePage() {
//   // נתוני המטופל
//   const [patientKey, setPatientKey] = useState(null);
//   const [patientName, setPatientName] = useState('');
//   const [email, setEmail] = useState('');
//   const [phone, setPhone] = useState('');
 
//   // כתובת
//   const [cityId, setCityId] = useState('');
//   const [streetId, setStreetId] = useState('');
//   const [houseNumber, setHouseNumber] = useState('');
//   const [postalCode, setPostalCode] = useState('');
 
//   // נתונים מקוריים להשוואה
//   const [originalData, setOriginalData] = useState({});
 
//   // רשימות
//   const [cities, setCities] = useState([]);
//   const [streets, setStreets] = useState([]);
 
//   // סיסמה
//   const [currentPassword, setCurrentPassword] = useState('');
//   const [newPassword, setNewPassword] = useState('');
//   const [confirmPassword, setConfirmPassword] = useState('');
 
//   // מצבי טעינה
//   const [loading, setLoading] = useState(true);
//   const [updating, setUpdating] = useState(false);
//   const [changingPassword, setChangingPassword] = useState(false);
//   const [loadingStreets, setLoadingStreets] = useState(false);
//   const [hasChanges, setHasChanges] = useState(false);

//   useEffect(() => {
//     loadInitialData();
//   }, []);

//   // עדכון מצב השינויים כאשר נתונים משתנים
//   useEffect(() => {
//     checkForChanges();
//   }, [patientName, email, phone, cityId, streetId, houseNumber, postalCode, originalData]);

//   const loadInitialData = async () => {
//     const patientKeyFromToken = getPatientKeyFromToken();
//     if (!patientKeyFromToken) {
//       console.error('❌ No patient key found');
//       setLoading(false);
//       return;
//     }

//     try {
//       console.log('🚀 Loading data for patient:', patientKeyFromToken);
     
//       const [patientData, citiesData] = await Promise.all([
//         getPatient(patientKeyFromToken),
//         getCities()
//       ]);
     
//       console.log('✅ Patient data:', patientData);
//       console.log('✅ Cities data:', citiesData);
     
//       // שמירת נתוני המטופל - תמיכה בשני סוגי השמות
//       setPatientKey(patientData.PatientKey || patientData.patientKey);
//       setPatientName(patientData.PatientName || patientData.patientName || '');
//       setEmail(patientData.Email || patientData.email || '');
//       setPhone(patientData.Phone || patientData.phone || '');
     
//       // שמירת נתוני כתובת - תמיכה בשני סוגי השמות
//       if (patientData.Address || patientData.address) {
//         const address = patientData.Address || patientData.address;
//         setCityId(address.CityId || address.cityId || '');
//         setStreetId(address.StreetId || address.streetId || '');
//         setHouseNumber(address.HouseNumber || address.houseNumber || '');
//         setPostalCode(address.PostalCode || address.postalCode || '');
       
//         // טעינת רחובות אם יש עיר
//         const addressCityId = address.CityId || address.cityId;
//         if (addressCityId) {
//           const streetsData = await getStreetsByCity(addressCityId);
//           setStreets(streetsData.data || []);
//         }
//       }
     
//       setCities(citiesData.data || []);
     
//       // שמירת נתונים מקוריים להשוואה
//       const address = patientData.Address || patientData.address;
//       const original = {
//         patientName: patientData.PatientName || patientData.patientName || '',
//         email: patientData.Email || patientData.email || '',
//         phone: patientData.Phone || patientData.phone || '',
//         cityId: address?.CityId || address?.cityId || '',
//         streetId: address?.StreetId || address?.streetId || '',
//         houseNumber: address?.HouseNumber || address?.houseNumber || '',
//         postalCode: address?.PostalCode || address?.postalCode || ''
//       };
//       setOriginalData(original);
     
//       console.log('✅ Original data saved:', original);
     
//     } catch (error) {
//       console.error('❌ Error loading data:', error);
//       handleApiError(error);
//     } finally {
//       setLoading(false);
//     }
//   };

//   const handleCityChange = async (newCityId) => {
//     console.log('🏙️ City changed to:', newCityId);
//     setCityId(newCityId);
//     setStreetId(''); // איפוס רחוב
   
//     if (newCityId) {
//       setLoadingStreets(true);
//       try {
//         const streetsData = await getStreetsByCity(newCityId);
//         setStreets(streetsData.data || []);
//       } catch (error) {
//         console.error('❌ Error loading streets:', error);
//         setStreets([]);
//       } finally {
//         setLoadingStreets(false);
//       }
//     } else {
//       setStreets([]);
//     }
//   };

//   const getCurrentData = () => {
//     return {
//       patientName: patientName.trim(),
//       email: email.trim(),
//       phone: phone.trim(),
//       cityId: cityId,
//       streetId: streetId,
//       houseNumber: houseNumber,
//       postalCode: postalCode.trim()
//     };
//   };

//   const checkForChanges = () => {
//     if (Object.keys(originalData).length === 0) return;
    
//     const current = getCurrentData();
//     const changed = {
//       name: current.patientName !== originalData.patientName,
//       email: current.email !== originalData.email,
//       phone: current.phone !== originalData.phone,
//       city: current.cityId !== originalData.cityId,
//       street: current.streetId !== originalData.streetId,
//       house: current.houseNumber !== originalData.houseNumber,
//       postal: current.postalCode !== originalData.postalCode
//     };
   
//     const hasAnyChange = Object.values(changed).some(Boolean);
//     setHasChanges(hasAnyChange);
//   };

//   const handleUpdate = async () => {
//     console.log('🚀 Starting update...');
   
//     if (!patientKey) {
//       alert('Patient key missing');
//       return;
//     }

//     const current = getCurrentData();
//     console.log('📊 Current data:', current);
//     console.log('📊 Original data:', originalData);

//     // בניית אובייקט עדכון - רק שדות שהשתנו
//     const updateData = {
//       PatientKey: patientKey  // השתמש ב-PascalCase עבור הקמיוניקציה עם השרת
//     };

//     // הוספת שדות שהשתנו - עם PascalCase
//     if (current.patientName !== originalData.patientName) {
//       if (!current.patientName || current.patientName.length < 2) {
//         alert('Name must be at least 2 characters');
//         return;
//       }
//       updateData.PatientName = current.patientName;
//     }

//     if (current.email !== originalData.email) {
//       if (!current.email || !isValidEmail(current.email)) {
//         alert('Please enter a valid email');
//         return;
//       }
//       updateData.Email = current.email;
//     }

//     if (current.phone !== originalData.phone) {
//       if (!current.phone || current.phone.length < 10) {
//         alert('Phone must be at least 10 characters');
//         return;
//       }
//       updateData.Phone = current.phone;
//     }

//     // בדיקת שינויים בכתובת
//     const addressChanged =
//       current.cityId !== originalData.cityId ||
//       current.streetId !== originalData.streetId ||
//       current.houseNumber !== originalData.houseNumber ||
//       current.postalCode !== originalData.postalCode;

//     if (addressChanged) {
//       // אם יש נתוני כתובת - חייבים להיות כולם או כולם ריקים
//       const hasAnyAddressData = current.cityId || current.streetId || current.houseNumber || current.postalCode;
//       const hasAllAddressData = current.cityId && current.streetId && current.houseNumber && current.postalCode;
      
//       if (hasAnyAddressData && !hasAllAddressData) {
//         alert('Please fill all address fields or leave all empty');
//         return;
//       }
      
//       if (hasAllAddressData) {
//         if (current.houseNumber < 1 || current.houseNumber > 9999) {
//           alert('House number must be between 1 and 9999');
//           return;
//         }
       
//         if (current.postalCode.length < 4 || current.postalCode.length > 10) {
//           alert('Postal code must be between 4 and 10 characters');
//           return;
//         }

//         updateData.Address = {
//           CityId: parseInt(current.cityId),
//           StreetId: parseInt(current.streetId),
//           HouseNumber: parseInt(current.houseNumber),
//           PostalCode: current.postalCode
//         };
//       }
//     }

//     console.log('📤 Sending update:', updateData);
   
//     setUpdating(true);
//     try {
//       const response = await updatePatient(updateData);
//       console.log('✅ Update successful:', response);
     
//       // עדכון הנתונים המקוריים
//       setOriginalData(current);
//       setHasChanges(false);
//       alert('Profile updated successfully!');
     
//     } catch (error) {
//       console.error('❌ Update failed:', error);
//       const errorMsg = error.response?.data?.message || error.message || 'Update failed';
//       alert(`Update failed: ${errorMsg}`);
//     } finally {
//       setUpdating(false);
//     }
//   };

//   const handleChangePassword = async () => {
//     if (!currentPassword || !newPassword || !confirmPassword) {
//       alert('Please fill all password fields');
//       return;
//     }

//     if (newPassword.length < 4 || newPassword.length > 15) {
//       alert('Password must be between 4 and 15 characters');
//       return;
//     }

//     if (newPassword !== confirmPassword) {
//       alert('Passwords do not match');
//       return;
//     }

//     setChangingPassword(true);
//     try {
//       await changePassword({
//         PatientKey: patientKey,  // השתמש ב-PascalCase
//         CurrentPassword: currentPassword,
//         NewPassword: newPassword,
//         ConfirmPassword: confirmPassword
//       });
     
//       alert('Password changed successfully!');
//       setCurrentPassword('');
//       setNewPassword('');
//       setConfirmPassword('');
//     } catch (error) {
//       console.error('❌ Password change failed:', error);
//       const errorMsg = error.response?.data?.message || error.message || 'Password change failed';
//       alert(`Password change failed: ${errorMsg}`);
//     } finally {
//       setChangingPassword(false);
//     }
//   };

//   const isValidEmail = (email) => {
//     return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
//   };

//   if (loading) {
//     return <div style={{ padding: '20px' }}>Loading...</div>;
//   }

//   return (
//     <div style={{ padding: '20px', maxWidth: '600px', margin: '0 auto' }}>
//       <h2>My Profile</h2>
     
//       {/* Personal Information */}
//       <div style={{
//         border: '1px solid #ddd',
//         padding: '20px',
//         marginBottom: '20px',
//         borderRadius: '8px',
//         backgroundColor: '#f9f9f9'
//       }}>
//         <h3>Personal Information</h3>
       
//         <div style={{ marginBottom: '15px' }}>
//           <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//             Full Name:
//           </label>
//           <input
//             style={{
//               width: '100%',
//               padding: '8px',
//               borderRadius: '4px',
//               border: '1px solid #ddd'
//             }}
//             value={patientName}
//             onChange={(e) => setPatientName(e.target.value)}
//             placeholder="Full Name"
//           />
//         </div>

//         <div style={{ marginBottom: '15px' }}>
//           <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//             Email:
//           </label>
//           <input
//             style={{
//               width: '100%',
//               padding: '8px',
//               borderRadius: '4px',
//               border: '1px solid #ddd'
//             }}
//             type="email"
//             value={email}
//             onChange={(e) => setEmail(e.target.value)}
//             placeholder="Email"
//           />
//         </div>

//         <div style={{ marginBottom: '15px' }}>
//           <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//             Phone:
//           </label>
//           <input
//             style={{
//               width: '100%',
//               padding: '8px',
//               borderRadius: '4px',
//               border: '1px solid #ddd'
//             }}
//             value={phone}
//             onChange={(e) => setPhone(e.target.value)}
//             placeholder="Phone"
//           />
//         </div>
//       </div>

//       {/* Address */}
//       <div style={{
//         border: '1px solid #ddd',
//         padding: '20px',
//         marginBottom: '20px',
//         borderRadius: '8px',
//         backgroundColor: '#f9f9f9'
//       }}>
//         <h3>Address</h3>
       
//         <div style={{ marginBottom: '15px' }}>
//           <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//             City:
//           </label>
//           <select
//             style={{
//               width: '100%',
//               padding: '8px',
//               borderRadius: '4px',
//               border: '1px solid #ddd'
//             }}
//             value={cityId}
//             onChange={(e) => handleCityChange(e.target.value)}
//           >
//             <option value="">-- Select City --</option>
//             {cities.map((city, index) => (
//               <option key={`city-${city.CityId || city.cityId}-${index}`} value={city.CityId || city.cityId}>
//                 {city.Name || city.name}
//               </option>
//             ))}
//           </select>
//         </div>

//         <div style={{ marginBottom: '15px' }}>
//           <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//             Street:
//           </label>
//           <select
//             style={{
//               width: '100%',
//               padding: '8px',
//               borderRadius: '4px',
//               border: '1px solid #ddd'
//             }}
//             value={streetId}
//             onChange={(e) => setStreetId(e.target.value)}
//             disabled={!cityId || loadingStreets}
//           >
//             <option value="">-- Select Street --</option>
//             {streets.map((street, index) => (
//               <option key={`street-${street.StreetId || street.streetId}-${index}`} value={street.StreetId || street.streetId}>
//                 {street.Name || street.name}
//               </option>
//             ))}
//           </select>
//           {loadingStreets && <div style={{ fontSize: '12px', color: '#666' }}>Loading streets...</div>}
//         </div>

//         <div style={{ display: 'flex', gap: '10px' }}>
//           <div style={{ flex: 1 }}>
//             <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//               House Number:
//             </label>
//             <input
//               style={{
//                 width: '100%',
//                 padding: '8px',
//                 borderRadius: '4px',
//                 border: '1px solid #ddd'
//               }}
//               type="number"
//               min="1"
//               max="9999"
//               value={houseNumber}
//               onChange={(e) => setHouseNumber(e.target.value)}
//               placeholder="House Number"
//             />
//           </div>

//           <div style={{ flex: 1 }}>
//             <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//               Postal Code:
//             </label>
//             <input
//               style={{
//                 width: '100%',
//                 padding: '8px',
//                 borderRadius: '4px',
//                 border: '1px solid #ddd'
//               }}
//               value={postalCode}
//               onChange={(e) => setPostalCode(e.target.value)}
//               placeholder="Postal Code"
//             />
//           </div>
//         </div>
//       </div>

//       {/* כפתור עדכון */}
//       <button
//         onClick={handleUpdate}
//         disabled={updating || !hasChanges}
//         style={{
//           backgroundColor: updating ? '#ccc' : (!hasChanges ? '#6c757d' : '#007bff'),
//           color: 'white',
//           padding: '10px 20px',
//           border: 'none',
//           borderRadius: '4px',
//           cursor: (updating || !hasChanges) ? 'not-allowed' : 'pointer',
//           fontSize: '16px',
//           marginBottom: '30px',
//           width: '100%'
//         }}
//       >
//         {updating ? 'Updating...' : (!hasChanges ? 'No Changes to Save' : 'Update Info')}
//       </button>

//       {/* Password Change */}
//       <div style={{
//         border: '1px solid #ddd',
//         padding: '20px',
//         borderRadius: '8px',
//         backgroundColor: '#f9f9f9'
//       }}>
//         <h3>Change Password</h3>
       
//         <div style={{ marginBottom: '15px' }}>
//           <input
//             style={{
//               width: '100%',
//               padding: '8px',
//               borderRadius: '4px',
//               border: '1px solid #ddd'
//             }}
//             type="password"
//             placeholder="Current Password"
//             value={currentPassword}
//             onChange={(e) => setCurrentPassword(e.target.value)}
//           />
//         </div>

//         <div style={{ marginBottom: '15px' }}>
//           <input
//             style={{
//               width: '100%',
//               padding: '8px',
//               borderRadius: '4px',
//               border: '1px solid #ddd'
//             }}
//             type="password"
//             placeholder="New Password (4-15 characters)"
//             value={newPassword}
//             onChange={(e) => setNewPassword(e.target.value)}
//           />
//         </div>

//         <div style={{ marginBottom: '15px' }}>
//           <input
//             style={{
//               width: '100%',
//               padding: '8px',
//               borderRadius: '4px',
//               border: '1px solid #ddd'
//             }}
//             type="password"
//             placeholder="Confirm New Password"
//             value={confirmPassword}
//             onChange={(e) => setConfirmPassword(e.target.value)}
//           />
//         </div>

//         <button
//           onClick={handleChangePassword}
//           disabled={changingPassword}
//           style={{
//             backgroundColor: changingPassword ? '#ccc' : '#28a745',
//             color: 'white',
//             padding: '10px 20px',
//             border: 'none',
//             borderRadius: '4px',
//             cursor: changingPassword ? 'not-allowed' : 'pointer',
//             fontSize: '16px',
//             width: '100%'
//           }}
//         >
//           {changingPassword ? 'Changing...' : 'Change Password'}
//         </button>
//       </div>
//     </div>
//   );
// }
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getPatient, updatePatient, changePassword, getCities, getStreetsByCity } from '../services/patientService';
import { getPatientKeyFromToken } from '../utils/authUtils';
import { handleApiError } from '../utils/errorUtil';
import { ArrowLeft, User, Mail, Phone, MapPin, Lock, Save, Eye, EyeOff, Heart } from 'lucide-react';

export default function PatientProfilePage() {
  const navigate = useNavigate();
  
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
  const [showCurrentPassword, setShowCurrentPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
 
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
     
      // שמירת נתוני המטופל - תמיכה בשני סוגי השמות
      setPatientKey(patientData.PatientKey || patientData.patientKey);
      setPatientName(patientData.PatientName || patientData.patientName || '');
      setEmail(patientData.Email || patientData.email || '');
      setPhone(patientData.Phone || patientData.phone || '');
     
      // שמירת נתוני כתובת - תמיכה בשני סוגי השמות
      if (patientData.Address || patientData.address) {
        const address = patientData.Address || patientData.address;
        setCityId(address.CityId || address.cityId || '');
        setStreetId(address.StreetId || address.streetId || '');
        setHouseNumber(address.HouseNumber || address.houseNumber || '');
        setPostalCode(address.PostalCode || address.postalCode || '');
       
        // טעינת רחובות אם יש עיר
        const addressCityId = address.CityId || address.cityId;
        if (addressCityId) {
          const streetsData = await getStreetsByCity(addressCityId);
          setStreets(streetsData.data || []);
        }
      }
     
      setCities(citiesData.data || []);
     
      // שמירת נתונים מקוריים להשוואה
      const address = patientData.Address || patientData.address;
      const original = {
        patientName: patientData.PatientName || patientData.patientName || '',
        email: patientData.Email || patientData.email || '',
        phone: patientData.Phone || patientData.phone || '',
        cityId: address?.CityId || address?.cityId || '',
        streetId: address?.StreetId || address?.streetId || '',
        houseNumber: address?.HouseNumber || address?.houseNumber || '',
        postalCode: address?.PostalCode || address?.postalCode || ''
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
      patientName: patientName.trim(),
      email: email.trim(),
      phone: phone.trim(),
      cityId: cityId,
      streetId: streetId,
      houseNumber: houseNumber,
      postalCode: postalCode.trim()
    };
  };

  const checkForChanges = () => {
    if (Object.keys(originalData).length === 0) return;
    
    const current = getCurrentData();
    const changed = {
      name: current.patientName !== originalData.patientName,
      email: current.email !== originalData.email,
      phone: current.phone !== originalData.phone,
      city: current.cityId !== originalData.cityId,
      street: current.streetId !== originalData.streetId,
      house: current.houseNumber !== originalData.houseNumber,
      postal: current.postalCode !== originalData.postalCode
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
      PatientKey: patientKey  // השתמש ב-PascalCase עבור הקמיוניקציה עם השרת
    };

    // הוספת שדות שהשתנו - עם PascalCase
    if (current.patientName !== originalData.patientName) {
      if (!current.patientName || current.patientName.length < 2) {
        alert('Name must be at least 2 characters');
        return;
      }
      updateData.PatientName = current.patientName;
    }

    if (current.email !== originalData.email) {
      if (!current.email || !isValidEmail(current.email)) {
        alert('Please enter a valid email');
        return;
      }
      updateData.Email = current.email;
    }

    if (current.phone !== originalData.phone) {
      if (!current.phone || current.phone.length < 10) {
        alert('Phone must be at least 10 characters');
        return;
      }
      updateData.Phone = current.phone;
    }

    // בדיקת שינויים בכתובת
    const addressChanged =
      current.cityId !== originalData.cityId ||
      current.streetId !== originalData.streetId ||
      current.houseNumber !== originalData.houseNumber ||
      current.postalCode !== originalData.postalCode;

    if (addressChanged) {
      // אם יש נתוני כתובת - חייבים להיות כולם או כולם ריקים
      const hasAnyAddressData = current.cityId || current.streetId || current.houseNumber || current.postalCode;
      const hasAllAddressData = current.cityId && current.streetId && current.houseNumber && current.postalCode;
      
      if (hasAnyAddressData && !hasAllAddressData) {
        alert('Please fill all address fields or leave all empty');
        return;
      }
      
      if (hasAllAddressData) {
        if (current.houseNumber < 1 || current.houseNumber > 9999) {
          alert('House number must be between 1 and 9999');
          return;
        }
       
        if (current.postalCode.length < 4 || current.postalCode.length > 10) {
          alert('Postal code must be between 4 and 10 characters');
          return;
        }

        updateData.Address = {
          CityId: parseInt(current.cityId),
          StreetId: parseInt(current.streetId),
          HouseNumber: parseInt(current.houseNumber),
          PostalCode: current.postalCode
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
        PatientKey: patientKey,  // השתמש ב-PascalCase
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

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Loading profile...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
      {/* Header */}
      <header className="bg-white/80 backdrop-blur-md shadow-sm sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center space-x-3">
              <button
                onClick={() => navigate('/home')}
                className="flex items-center space-x-2 text-blue-600 hover:text-blue-700 transition-colors"
              >
                <ArrowLeft className="h-5 w-5" />
                <span>Back to Home</span>
              </button>
            </div>
            
            <div className="flex items-center space-x-3">
              <div className="bg-gradient-to-r from-blue-600 to-green-600 p-2 rounded-xl">
                <Heart className="h-6 w-6 text-white" />
              </div>
              <h1 className="text-xl font-bold bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
                MediCare Pro
              </h1>
            </div>
            
            <button
              onClick={handleLogout}
              className="bg-gradient-to-r from-red-500 to-red-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:from-red-600 hover:to-red-700 transition-all duration-200 shadow-md hover:shadow-lg"
            >
              Logout
            </button>
          </div>
        </div>
      </header>

      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">My Profile</h1>
          <p className="text-xl text-gray-600">Edit and update your personal information</p>
        </div>

        {/* Personal Information */}
        <div className="bg-white rounded-3xl shadow-lg p-8 mb-8 border border-gray-100">
          <div className="flex items-center space-x-3 mb-8">
            <div className="bg-gradient-to-r from-blue-500 to-blue-600 p-3 rounded-xl">
              <User className="h-6 w-6 text-white" />
            </div>
            <h2 className="text-2xl font-bold text-gray-900">Personal Information</h2>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                <User className="inline h-4 w-4 mr-1" />
                Full Name
              </label>
              <input
                type="text"
                value={patientName}
                onChange={(e) => setPatientName(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200"
                placeholder="Enter your full name"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                <Mail className="inline h-4 w-4 mr-1" />
                Email Address
              </label>
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200"
                placeholder="Enter your email"
              />
            </div>

            <div className="md:col-span-1">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                <Phone className="inline h-4 w-4 mr-1" />
                Phone Number
              </label>
              <input
                type="tel"
                value={phone}
                onChange={(e) => setPhone(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200"
                placeholder="Enter your phone number"
              />
            </div>
          </div>
        </div>

        {/* Address Information */}
        <div className="bg-white rounded-3xl shadow-lg p-8 mb-8 border border-gray-100">
          <div className="flex items-center space-x-3 mb-8">
            <div className="bg-gradient-to-r from-green-500 to-green-600 p-3 rounded-xl">
              <MapPin className="h-6 w-6 text-white" />
            </div>
            <h2 className="text-2xl font-bold text-gray-900">Address Information</h2>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">City</label>
              <select
                value={cityId}
                onChange={(e) => handleCityChange(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent transition-all duration-200"
              >
                <option value="">-- Select City --</option>
                {cities.map((city, index) => (
                  <option key={`city-${city.CityId || city.cityId}-${index}`} value={city.CityId || city.cityId}>
                    {city.Name || city.name}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Street</label>
              <select
                value={streetId}
                onChange={(e) => setStreetId(e.target.value)}
                disabled={!cityId || loadingStreets}
                className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent transition-all duration-200 disabled:bg-gray-100 disabled:cursor-not-allowed"
              >
                <option value="">-- Select Street --</option>
                {streets.map((street, index) => (
                  <option key={`street-${street.StreetId || street.streetId}-${index}`} value={street.StreetId || street.streetId}>
                    {street.Name || street.name}
                  </option>
                ))}
              </select>
              {loadingStreets && (
                <div className="mt-1 text-sm text-blue-600 flex items-center">
                  <div className="animate-spin rounded-full h-3 w-3 border-b border-blue-600 mr-2"></div>
                  Loading streets...
                </div>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">House Number</label>
              <input
                type="number"
                min="1"
                max="9999"
                value={houseNumber}
                onChange={(e) => setHouseNumber(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent transition-all duration-200"
                placeholder="House number"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Postal Code</label>
              <input
                type="text"
                value={postalCode}
                onChange={(e) => setPostalCode(e.target.value)}
                className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent transition-all duration-200"
                placeholder="Postal code"
              />
            </div>
          </div>
        </div>

        {/* Update Button */}
        <div className="mb-8">
          <button
            onClick={handleUpdate}
            disabled={updating || !hasChanges}
            className={`w-full flex items-center justify-center space-x-2 py-4 px-6 rounded-xl font-medium text-white transition-all duration-200 shadow-lg hover:shadow-xl ${
              updating || !hasChanges
                ? 'bg-gray-400 cursor-not-allowed'
                : 'bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800'
            }`}
          >
            <Save className="h-5 w-5" />
            <span>
              {updating ? 'Updating...' : (!hasChanges ? 'No Changes to Save' : 'Update Profile')}
            </span>
          </button>
        </div>

        {/* Password Change */}
        <div className="bg-white rounded-3xl shadow-lg p-8 border border-gray-100">
          <div className="flex items-center space-x-3 mb-8">
            <div className="bg-gradient-to-r from-purple-500 to-purple-600 p-3 rounded-xl">
              <Lock className="h-6 w-6 text-white" />
            </div>
            <h2 className="text-2xl font-bold text-gray-900">Change Password</h2>
          </div>
          
          <div className="space-y-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Current Password</label>
              <div className="relative">
                <input
                  type={showCurrentPassword ? 'text' : 'password'}
                  value={currentPassword}
                  onChange={(e) => setCurrentPassword(e.target.value)}
                  className="w-full p-3 pr-12 border border-gray-300 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all duration-200"
                  placeholder="Enter current password"
                />
                <button
                  type="button"
                  onClick={() => setShowCurrentPassword(!showCurrentPassword)}
                  className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  {showCurrentPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                </button>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">New Password</label>
              <div className="relative">
                <input
                  type={showNewPassword ? 'text' : 'password'}
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  className="w-full p-3 pr-12 border border-gray-300 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all duration-200"
                  placeholder="Enter new password (4-15 characters)"
                />
                <button
                  type="button"
                  onClick={() => setShowNewPassword(!showNewPassword)}
                  className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  {showNewPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                </button>
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Confirm New Password</label>
              <div className="relative">
                <input
                  type={showConfirmPassword ? 'text' : 'password'}
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  className="w-full p-3 pr-12 border border-gray-300 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all duration-200"
                  placeholder="Confirm new password"
                />
                <button
                  type="button"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  {showConfirmPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                </button>
              </div>
            </div>

            <button
              onClick={handleChangePassword}
              disabled={changingPassword}
              className={`w-full flex items-center justify-center space-x-2 py-4 px-6 rounded-xl font-medium text-white transition-all duration-200 shadow-lg hover:shadow-xl ${
                changingPassword
                  ? 'bg-gray-400 cursor-not-allowed'
                  : 'bg-gradient-to-r from-purple-600 to-purple-700 hover:from-purple-700 hover:to-purple-800'
              }`}
            >
              <Lock className="h-5 w-5" />
              <span>{changingPassword ? 'Changing Password...' : 'Change Password'}</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}