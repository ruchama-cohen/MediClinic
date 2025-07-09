//src/services/authService.ts
import instance from '../axiosConfig';

export async function login(id, password) {
  try {
const response = await instance.post('/auth/login',
{
  UserId: id,
  UserPassword: password
});
const data = response.data;
    const token = data.token || data.Token;
    if (!token) {
      throw new Error('No token received from server');
    }
    localStorage.setItem('token', token);
    return {
      token,
      success: data.success,
      patient: data.patient || data.Patient
    };

  } catch (error) {
    console.error('Login error:', error);
    // Normalize error message
    const message = error.response?.data?.message || error.message || 'Login failed';
    throw new Error(message);
  }
}
