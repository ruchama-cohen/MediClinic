// src/utils/authUtils.js
import { jwtDecode } from 'jwt-decode';

export function getPatientIdFromToken() {
  const token = localStorage.getItem('token');
  if (!token) return null;

  try {
    const decoded = jwtDecode(token);
    console.log('Decoded token:', decoded);
    return decoded.PatientId;  
  } catch (err) {
    console.error("Error decoding token:", err);
    return null;
  }
}
