// src/components/ProtectedRoute.jsx
import { Navigate } from 'react-router-dom';
import { getPatientKeyFromToken } from '../utils/authUtils';

function ProtectedRoute({ children }) {
  const token = localStorage.getItem('token');
  const patientKey = getPatientKeyFromToken();

  if (!token || !patientKey) {
    // If no token or valid patient key, redirect to login
    return <Navigate to="/" replace />;
  }

  // If token and patient key exist, allow access
  return children;
}

export default ProtectedRoute;