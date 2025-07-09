//src/services/servicesService.js
import instance from '../axiosConfig';

// שליפת כל השירותים הרפואיים
export async function getAllServices() {
  const response = await instance.get('/service');
  return response.data.data;  // מחזיר רק את המערך של שירותים
}
