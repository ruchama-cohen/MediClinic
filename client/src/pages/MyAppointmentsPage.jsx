import { useEffect, useState } from 'react';
import { getAppointmentsByUser, cancelAppointment } from '../services/appointmentService.js';
import { getPatientKeyFromToken } from '../utils/authUtils';

export default function MyAppointmentsPage() {
  const [appointments, setAppointments] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const patientKey = getPatientKeyFromToken();
    if (patientKey) {
      getAppointmentsByUser(patientKey)
        .then((data) => {
          setAppointments(data.data || []);
          setError(null);
        })
        .catch((err) => {
          console.error('Failed to fetch appointments:', err);
          setError('Failed to load appointments.');
        })
        .finally(() => setLoading(false));
    } else {
      setError('No patient key found. Please log in.');
      setLoading(false);
    }
  }, []);

  const handleCancel = async (id) => {
    try {
      await cancelAppointment(id);
      setAppointments((prev) => prev.filter((a) => a.id !== id && a.appointmentId !== id));
    } catch (error) {
      console.error('Failed to cancel appointment:', error);
      alert('Failed to cancel appointment. Please try again.');
    }
  };

  if (loading) return <p>Loading appointments...</p>;

  return (
    <div>
      <h2>My Appointments</h2>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      {appointments.length === 0 && !error ? (
        <p>No appointments found.</p>
      ) : (
        <ul>
          {appointments.map((a) => {
            const slot = a.slot || {};

            const doctorName = slot.providerKeyNavigation?.name || 'Unknown Doctor';
            const location = slot.branch?.branchName || 'Unknown Location';
            const date = slot.slotDate || a.date || 'Unknown Date';
            const time = slot.slotStart || a.time || 'Unknown Time';

            return (
              <li key={a.id || a.appointmentId}>
                {date} - {time}
                {' '}with <strong>{doctorName}</strong> at <strong>{location}</strong>
                <br />
                <button onClick={() => handleCancel(a.id || a.appointmentId)}>Cancel</button>
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
}
  src/pages/AppointmentBookingPage.jsx
        modified:   src/pages/MyAppointmentsPage.jsx
        modified:   src/pages/PatientProfilePage.jsx
        modified:   src/services/appointmentService.js
        modified:   src/services/authService.js
        modified:   src/services/patientService.js
        modified:   src/services/servicesService.js
        modified:   src/utils/authUtils.js 