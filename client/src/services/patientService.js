import instance from '../axiosConfig';

export async function getPatient(patientKey) {
  const response = await instance.get(`/patient/by-key/${patientKey}`);
  return response.data;
}

export async function updatePatient(patient) {
  // בדיקה אם יש נתוני כתובת שהוזנו
  const hasAddressData = patient.Address && (
    patient.Address.CityName || 
    patient.Address.StreetName || 
    patient.Address.HouseNumber || 
    patient.Address.PostalCode
  );

  // הכנת הבקשה לפי הפורמט הנדרש של השרת
  const requestData = {
    PatientKey: patient.PatientKey,
    PatientName: patient.PatientName,
    Email: patient.Email,
    Phone: patient.Phone,
    // שלח כתובת רק אם יש נתונים
    Address: hasAddressData ? {
      CityName: patient.Address.CityName || '',
      StreetName: patient.Address.StreetName || '',
      HouseNumber: parseInt(patient.Address.HouseNumber) || 0,
      PostalCode: patient.Address.PostalCode || ''
    } : null
  };

  return instance.put('/patient/update', requestData);
}

export async function changePassword({ patientKey, currentPassword, newPassword }) {
  return instance.post('/patient/change-password', {
    PatientKey: patientKey,
    CurrentPassword: currentPassword,
    NewPassword: newPassword,
    ConfirmPassword: newPassword // השרת מצפה גם לשדה זה
  });
}