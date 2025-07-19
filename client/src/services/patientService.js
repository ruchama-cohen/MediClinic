import instance from '../axiosConfig';

export async function getPatient(patientKey) {
  console.log('🔍 Getting patient for key:', patientKey);
  const response = await instance.get(`/patient/by-key/${patientKey}`);
  console.log('✅ Patient data received:', response.data);
  return response.data;
}

export async function updatePatient(updatedFields) {
  console.log('📝 Sending update request with data:', updatedFields);
 
  try {
    const response = await instance.put('/patient/update', updatedFields);
    console.log('✅ Update response:', response.data);
    return response;
  } catch (error) {
    console.error('❌ Update error:', error.response?.data || error.message);
    throw error;
  }
}

export async function changePassword({ PatientKey, CurrentPassword, NewPassword, ConfirmPassword }) {
  console.log('🔐 Changing password for patient:', PatientKey);
  return instance.post('/patient/change-password', {
    PatientKey: PatientKey,
    CurrentPassword: CurrentPassword,
    NewPassword: NewPassword,
    ConfirmPassword: ConfirmPassword
  });
}

export async function getCities() {
  console.log('🏙️ Getting cities list');
  const response = await instance.get('/patient/cities');
  console.log('✅ Cities received:', response.data);
  return response.data;
}

export async function getStreetsByCity(cityId) {
  console.log('🛣️ Getting streets for city:', cityId);
  const response = await instance.get(`/patient/streets/${cityId}`);
  console.log('✅ Streets received:', response.data);
  return response.data;
}