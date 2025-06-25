//src/services/patientService.js
import axios from '../axiosConfig';

export async function getPatient(patientId) {
  const response = await axios.get(`/patient/${patientId}`);
  return response.data;
}

export async function updatePatient(data) {
  const response = await axios.put('/patient/update', data);
  return response.data;
}

export async function changePassword(data) {
  const response = await axios.post('/patient/change-password', data);
  return response.data;
}
