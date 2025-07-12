import { jwtDecode } from 'jwt-decode';

export function getPatientKeyFromToken() {
  const token = localStorage.getItem('token');
  if (!token) return null;
  try {
    const decoded = jwtDecode(token);
    return decoded.PatientKey;
  } catch (err) {
    console.error("Error decoding token:", err);
    return null;
  }
}

export function getPatientIdFromToken() {
  const token = localStorage.getItem('token');
  if (!token) return null;
  try {
    const decoded = jwtDecode(token);
    return decoded.PatientId;
  } catch (err) {
    console.error("Error decoding token:", err);
    return null;
  }
}
