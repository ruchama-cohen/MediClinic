import instance from '../axiosConfig';

export async function getPatient(patientKey) {
  const response = await instance.get(`/patient/by-key/${patientKey}`);
  return response.data;
}

export async function updatePatient(updatedFields) {
  console.log('Sending update request with data:', updatedFields);
  
  try {
    // חזרה לנתיב המקורי - עכשיו זה אמור לעבוד אחרי תיקון הקונטרולר
    const response = await instance.put('/patient/update', updatedFields);
    console.log('Update response:', response.data);
    return response;
  } catch (error) {
    console.error('Update error:', error.response?.data || error.message);
    throw error;
  }
}

export async function changePassword({ patientKey, currentPassword, newPassword }) {
  return instance.post('/patient/change-password', {
    PatientKey: patientKey,
    CurrentPassword: currentPassword,
    NewPassword: newPassword,
    ConfirmPassword: newPassword // השרת מצפה גם לשדה זה
  });
}