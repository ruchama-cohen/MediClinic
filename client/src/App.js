// src/routes.jsx
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import HomePage from './pages/HomePage';
import AppointmentBookingPage from './pages/AppointmentBookingPage';
import MyAppointmentsPage from './pages/MyAppointmentsPage';
import PatientProfilePage from './pages/PatientProfilePage';
import ServicesPage from './pages/ServicesPage'; 

export default function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/home" element={<HomePage />} />
        <Route path="/appointments/book" element={<AppointmentBookingPage />} />
        <Route path="/appointments/mine" element={<MyAppointmentsPage />} />
        <Route path="/profile" element={<PatientProfilePage />} />
        <Route path="/services" element={<ServicesPage />} />
      </Routes>
    </Router>
  );
}

