import instance from '../axiosConfig.js'; // או הנתיב שבו מוגדר ה-instance
//src/services/appointmentService
// קבלת תורים לפי שם רופא
export async function getAppointmentsByProviderName(doctorName) {
  const response = await instance.get(`/appointments/byProvider/${encodeURIComponent(doctorName)}`);
  return response.data;
}

// קבלת זמני תורים פנויים לפי רופא ועיר
export async function getAvailableSlotsByProviderAndCity(doctorName, cityName) {
  const response = await instance.get('/appointments/byProviderCity', {
    params: { doctorName, cityName },
  });
  return response.data;
}

// קבלת זמני תורים פנויים לפי שירות
export async function getAvailableSlotsByService(serviceId) {
  const response = await instance.get(`/appointments/byService/${serviceId}`);
  return response.data;
}

// הזמנת תור (Booking)
export async function bookAppointment(slotId, patientKey) {
  const response = await instance.post(`/appointments/book/${slotId}`, { PatientKey: patientKey });
  return response.data;
}

// ביטול תור
export async function cancelAppointment(appointmentId) {
  const response = await instance.delete(`/appointments/${appointmentId}`);
  return response.data;
}

// קבלת תורים לפי משתמש (מטופל)
export async function getAppointmentsByUser(patientKey) {
  const response = await instance.get(`/appointments/byUser/${encodeURIComponent(patientKey)}`);
  return response.data;
}

// יצירת זמני תורים חדשים לספק שירות
export async function generateSlotsForProvider(providerKey, startDate, endDate) {
  const response = await instance.post('/appointments/generateSlots', null, {
    params: {
      providerKey,
      startDate, // צריך להיות בפורמט שמתאים ל-API, לדוגמה 'YYYY-MM-DD'
      endDate,
    },
  });
  return response.data;
}
