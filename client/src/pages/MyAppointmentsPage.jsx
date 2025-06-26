import { useEffect, useState } from 'react';
import { getAppointmentsByUser, cancelAppointment } from '../services/appointmentService.js';
 import { getPatientIdFromToken } from '../utils/authUtils';
 
export default function MyAppointmentsPage() {
  const [appointments, setAppointments] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {

   const patientId =getPatientIdFromToken();
    console.log('Patient ID:', patientId); // Debugging line to check patient ID
    if (patientId) {
      getAppointmentsByUser(patientId)
        .then((data) => {
          setAppointments(data.data || []);  // לפי מבנה התגובה שלך ב-API
          setError(null);
        })
        .catch((err) => {
          console.error('Failed to fetch appointments:', err);
          setError('Failed to load appointments.');
        })
        .finally(() => setLoading(false));
    } else {
      setError('No patient ID found. Please log in.');
      setLoading(false);
    }
  }, []);

  const handleCancel = async (id) => {
    try {
      await cancelAppointment(id);
      setAppointments((prev) => prev.filter((a) => a.id !== id));
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
          {appointments.map((a) => (
            <li key={a.id}>
              {a.date} - {a.time} with {a.providerName}
              <button onClick={() => handleCancel(a.id)}>Cancel</button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
