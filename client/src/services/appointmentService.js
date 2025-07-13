import instance from '../axiosConfig.js';

// קבלת כל השירותים הזמינים
export async function getAvailableServices() {
  const response = await instance.get('/appointments/services');
  return response.data;
}

// קבלת רופאים לפי סוג שירות
export async function getProvidersByService(serviceId) {
  const response = await instance.get(`/appointments/providers/${serviceId}`);
  return response.data;
}

// קבלת ערים זמינות
export async function getAvailableCities() {
  const response = await instance.get('/appointments/cities');
  return response.data;
}

// קבלת זמני טיפול זמינים
export async function getTimePeriods() {
  const response = await instance.get('/appointments/time-periods');
  return response.data;
}

// חיפוש תורים זמינים עם סינונים
export async function searchAvailableSlots(searchParams) {
  const response = await instance.post('/appointments/search', searchParams);
  return response.data;
}

// הפונקציות הישנות נשארות לתאימות לאחור
export async function getAppointmentsByProviderName(doctorName) {
  const response = await instance.get(`/appointments/byProvider/${encodeURIComponent(doctorName)}`);
  return response.data;
}

export async function getAvailableSlotsByProviderAndCity(doctorName, cityName) {
  const response = await instance.get('/appointments/byProviderCity', {
    params: { doctorName, cityName },
  });
  return response.data;
}

export async function getAvailableSlotsByService(serviceId) {
  const response = await instance.get(`/appointments/byService/${serviceId}`);
  return response.data;
}

export async function bookAppointment(slotId, patientKey) {
  const response = await instance.post(`/appointments/book/${slotId}`, { PatientKey: patientKey });
  return response.data;
}

export async function cancelAppointment(appointmentId) {
  const response = await instance.delete(`/appointments/${appointmentId}`);
  return response.data;
}

export async function getAppointmentsByUser(patientKey) {
  const response = await instance.get(`/appointments/byUserKey/${patientKey}`);
  return response.data;
}

export async function generateSlotsForProvider(providerKey, startDate, endDate) {
  const response = await instance.post('/appointments/generateSlots', null, {
    params: {
      providerKey,
      startDate,
      endDate,
    },
  });
  return response.data;
}