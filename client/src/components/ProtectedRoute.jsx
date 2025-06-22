// src/components/ProtectedRoute.jsx
import { Navigate } from 'react-router-dom';

function ProtectedRoute({ children }) {
  const token = localStorage.getItem('token');

  if (!token) {
    // If no token, redirect to login
    return <Navigate to="/" replace />;
  }

  // If token exists, allow access
  return children;
}

export default ProtectedRoute;
