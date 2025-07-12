// src/App.js
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import HomePage from './pages/HomePage';
import AppointmentBookingPage from './pages/AppointmentBookingPage';
import MyAppointmentsPage from './pages/MyAppointmentsPage';
import PatientProfilePage from './pages/PatientProfilePage';
import ServicesPage from './pages/ServicesPage'; 
import ProtectedRoute from './components/ProtectedRoute';

export default function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/home" element={
          <ProtectedRoute>
            <HomePage />
          </ProtectedRoute>
        } />
        <Route path="/appointments/book" element={
          <ProtectedRoute>
            <AppointmentBookingPage />
          </ProtectedRoute>
        } />
        <Route path="/appointments/mine" element={
          <ProtectedRoute>
            <MyAppointmentsPage />
          </ProtectedRoute>
        } />
        <Route path="/profile" element={
          <ProtectedRoute>
            <PatientProfilePage />
          </ProtectedRoute>
        } />
        <Route path="/services" element={
          <ProtectedRoute>
            <ServicesPage />
          </ProtectedRoute>
        } />
      </Routes>
    </Router>
  );
}