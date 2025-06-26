// src/utils/authUtils.js
import jwtDecode from 'jwt-decode';

export function getPatientIdFromToken() {
  const token = localStorage.getItem('token');
  if (!token) return null;

  try {
    const decoded = jwtDecode(token);
    return decoded.patientId || decoded.sub || null;
  } catch (error) {
    console.error('Failed to decode token:', error);
    return null;
  }
}
