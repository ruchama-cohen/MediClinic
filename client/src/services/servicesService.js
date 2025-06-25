//src/services/servicesService.js
import axios from '../axiosConfig';

// שליפת כל השירותים הרפואיים
export async function getAllServices() {
  const response = await axios.get('/service');
  return response.data.data;  // מחזיר רק את המערך של שירותים
}
