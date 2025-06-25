import axios from '../axiosConfig';
//src/services/appointmentService
// קבלת תורים לפי שם רופא
export async function getAppointmentsByProviderName(doctorName) {
  const response = await axios.get(`/appointments/byProvider/${encodeURIComponent(doctorName)}`);
  return response.data;
}

// קבלת זמני תורים פנויים לפי רופא ועיר
export async function getAvailableSlotsByProviderAndCity(doctorName, cityName) {
  const response = await axios.get('/appointments/byProviderCity', {
    params: { doctorName, cityName },
  });
  return response.data;
}

// קבלת זמני תורים פנויים לפי שירות
export async function getAvailableSlotsByService(serviceId) {
  const response = await axios.get(`/appointments/byService/${serviceId}`);
  return response.data;
}

// הזמנת תור (Booking)
export async function bookAppointment(slotId, patientId) {
  const response = await axios.post(`/appointments/book/${slotId}`, { PatientId: patientId });
  return response.data;
}

// ביטול תור
export async function cancelAppointment(appointmentId) {
  const response = await axios.delete(`/appointments/${appointmentId}`);
  return response.data;
}

// קבלת תורים לפי משתמש (מטופל)
export async function getAppointmentsByUser(patientId) {
  const response = await axios.get(`/appointments/byUser/${encodeURIComponent(patientId)}`);
  return response.data;
}

// יצירת זמני תורים חדשים לספק שירות
export async function generateSlotsForProvider(providerKey, startDate, endDate) {
  const response = await axios.post('/appointments/generateSlots', null, {
    params: {
      providerKey,
      startDate, // צריך להיות בפורמט שמתאים ל-API, לדוגמה 'YYYY-MM-DD'
      endDate,
    },
  });
  return response.data;
}
