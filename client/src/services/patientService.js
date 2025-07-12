import instance from '../axiosConfig';

export async function getPatient(patientKey) {
  const response = await instance.get(`/patient/by-key/${patientKey}`);
  return response.data;
}

export async function updatePatient(updatedFields) {
  // שלח רק את השדות שהשתנו
  // השרת יטפל בעדכון רק של השדות שהתקבלו
  return instance.put('/patient/update', updatedFields);
}

export async function changePassword({ patientKey, currentPassword, newPassword }) {
  return instance.post('/patient/change-password', {
    PatientKey: patientKey,
    CurrentPassword: currentPassword,
    NewPassword: newPassword,
    ConfirmPassword: newPassword // השרת מצפה גם לשדה זה
  });
}