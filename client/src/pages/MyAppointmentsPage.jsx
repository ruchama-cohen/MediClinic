// import { useEffect, useState } from 'react';
// import { getAppointmentsByUser, cancelAppointment } from '../services/appointmentService.js';
// import { getPatientKeyFromToken } from '../utils/authUtils';

// export default function() {
//   const [appointments, setAppointments] = useState([]);
//   const [error, setError] = useState(null);
//   const [loading, setLoading] = useState(true);

//   useEffect(() => {
//     const patientKey = getPatientKeyFromToken();
//     if (patientKey) {
//       getAppointmentsByUser(patientKey)
//         .then((data) => {
//           setAppointments(data.data || []);
//           setError(null);
//         })
//         .catch((err) => {
//           console.error('Failed to fetch appointments:', err);
//           setError('Failed to load appointments.');
//         })
//         .finally(() => setLoading(false));
//     } else {
//       setError('No patient key found. Please log in.');
//       setLoading(false);
//     }
//   }, []);

//   const handleCancel = async (id) => {
//     try {
//       await cancelAppointment(id);
//       setAppointments((prev) => prev.filter((a) => a.id !== id && a.appointmentId !== id));
//     } catch (error) {
//       console.error('Failed to cancel appointment:', error);
//       alert('Failed to cancel appointment. Please try again.');
//     }
//   };

//   if (loading) return <p>Loading appointments...</p>;

//   return (
//     <div>
//       <h2>My Appointments</h2>

//       {error && <p style={{ color: 'red' }}>{error}</p>}

//       {appointments.length === 0 && !error ? (
//         <p>No appointments found.</p>
//       ) : (
//         <ul>
//           {appointments.map((a) => {
//             const slot = a.slot || {};

//             const doctorName = slot.providerKeyNavigation?.name || 'Unknown Doctor';
//             const location = slot.branch?.branchName || 'Unknown Location';
//             const date = slot.slotDate || a.date || 'Unknown Date';
//             let timeRaw = slot.slotStart || a.time || 'Unknown Time';
//             // Format time to HH:mm
//             let time = timeRaw && typeof timeRaw === 'string' && timeRaw.length >= 5 ? timeRaw.substring(0,5) : timeRaw;

//             return (
//               <li key={a.id || a.appointmentId}>
//                 {date} - {time}
//                 {' '}with <strong>{doctorName}</strong> at <strong>{location}</strong>
//                 <br />
//                 <button onClick={() => handleCancel(a.id || a.appointmentId)}>Cancel</button>
//               </li>
//             );
//           })}
//         </ul>
//       )}
//     </div>
//   );
// }
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Heart, Calendar, X, ArrowLeft, User, MapPin, Clock } from 'lucide-react';
import { getAppointmentsByUser, cancelAppointment } from '../services/appointmentService.js';
import { getPatientKeyFromToken } from '../utils/authUtils';

export default function MyAppointmentsPage() {
  const navigate = useNavigate();
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
    if (!window.confirm('Are you sure you want to cancel this appointment?')) return;
    
    try {
      await cancelAppointment(id);
      setAppointments((prev) => prev.filter((a) => a.id !== id && a.appointmentId !== id));
      alert('Appointment cancelled successfully');
    } catch (error) {
      console.error('Failed to cancel appointment:', error);
      alert('Failed to cancel appointment. Please try again.');
    }
  };

  const formatDate = (dateStr) => {
    try {
      const date = new Date(dateStr);
      if (isNaN(date.getTime())) {
        return 'Invalid Date';
      }
      return date.toLocaleDateString('en-US', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });
    } catch (error) {
      console.error('Error formatting date:', error);
      return 'Invalid Date';
    }
  };

  const formatTime = (timeRaw) => {
    try {
      return timeRaw && typeof timeRaw === 'string' && timeRaw.length >= 5 ? timeRaw.substring(0,5) : timeRaw;
    } catch (error) {
      console.error('Error formatting time:', error);
      return 'Unknown Time';
    }
  };

  // Header Component
  const Header = () => (
    <header className="bg-white/80 backdrop-blur-md shadow-sm sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          <div className="flex items-center space-x-3">
            <div className="bg-gradient-to-r from-blue-600 to-green-600 p-2 rounded-xl">
              <Heart className="h-6 w-6 text-white" />
            </div>
            <h1 className="text-xl font-bold bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
              MediCare Pro
            </h1>
          </div>
          
          <button
            onClick={() => {
              localStorage.removeItem('token');
              navigate('/');
            }}
            className="bg-gradient-to-r from-red-500 to-red-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:from-red-600 hover:to-red-700 transition-all duration-200 shadow-md hover:shadow-lg"
          >
            Logout
          </button>
        </div>
      </div>
    </header>
  );

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
        <Header />
        <div className="flex items-center justify-center min-h-[60vh]">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Loading appointments...</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
      <Header />
      
      <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <button
          onClick={() => navigate('/home')}
          className="flex items-center space-x-2 text-blue-600 hover:text-blue-700 mb-6 transition-colors duration-200"
        >
          <ArrowLeft className="h-4 w-4" />
          <span>Back to Home</span>
        </button>

        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            My Appointments
          </h1>
          <p className="text-xl text-gray-600">
            View and manage all your appointments
          </p>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-2xl p-6 mb-8">
            <p className="text-red-600 font-medium">{error}</p>
          </div>
        )}

        {appointments.length === 0 && !error ? (
          <div className="bg-white rounded-3xl shadow-lg p-12 text-center">
            <div className="text-6xl mb-6">📅</div>
            <h3 className="text-2xl font-bold text-gray-900 mb-4">No Appointments</h3>
            <p className="text-gray-600 mb-8">You haven't booked any appointments yet</p>
            <button
              onClick={() => navigate('/appointments/book')}
              className="bg-gradient-to-r from-blue-600 to-blue-700 text-white px-8 py-3 rounded-xl font-medium hover:from-blue-700 hover:to-blue-800 transition-all duration-200 shadow-lg hover:shadow-xl"
            >
              Book Your First Appointment
            </button>
          </div>
        ) : (
          <div className="space-y-6">
            {appointments.map((appointment) => {
              const slot = appointment.slot || {};
              const doctorName = slot.providerKeyNavigation?.name || 'Unknown Doctor';
              const location = slot.branch?.branchName || 'Unknown Location';
              const date = slot.slotDate || appointment.date || 'Unknown Date';
              const timeRaw = slot.slotStart || appointment.time || 'Unknown Time';
              const time = formatTime(timeRaw);

              return (
                <div
                  key={appointment.id || appointment.appointmentId}
                  className="bg-white rounded-2xl shadow-lg p-6 border border-gray-100 hover:shadow-xl transition-all duration-300"
                >
                  <div className="flex justify-between items-start">
                    <div className="flex-1">
                      <div className="flex items-center space-x-4 mb-4">
                        <div className="bg-gradient-to-r from-green-500 to-green-600 p-3 rounded-xl">
                          <Calendar className="h-6 w-6 text-white" />
                        </div>
                        <div>
                          <h3 className="text-xl font-bold text-gray-900">
                            {formatDate(date)}
                          </h3>
                          <div className="flex items-center space-x-2 text-gray-600">
                            <Clock className="h-4 w-4" />
                            <span>Time: {time}</span>
                          </div>
                        </div>
                      </div>
                      
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                        <div>
                          <p className="text-sm text-gray-500 mb-1">Doctor</p>
                          <div className="flex items-center space-x-2">
                            <User className="h-4 w-4 text-gray-400" />
                            <p className="font-semibold text-gray-900">{doctorName}</p>
                          </div>
                        </div>
                        <div>
                          <p className="text-sm text-gray-500 mb-1">Location</p>
                          <div className="flex items-center space-x-2">
                            <MapPin className="h-4 w-4 text-gray-400" />
                            <p className="font-semibold text-gray-900">{location}</p>
                          </div>
                        </div>
                      </div>
                    </div>
                    
                    <button
                      onClick={() => handleCancel(appointment.id || appointment.appointmentId)}
                      className="bg-gradient-to-r from-red-500 to-red-600 text-white px-4 py-2 rounded-lg font-medium hover:from-red-600 hover:to-red-700 transition-all duration-200 shadow-md hover:shadow-lg flex items-center space-x-2"
                    >
                      <X className="h-4 w-4" />
                      <span>Cancel</span>
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}