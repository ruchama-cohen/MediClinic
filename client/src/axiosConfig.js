// axiosConfig.js
import axios from 'axios';
const instance = axios.create({
  baseURL: 'https://localhost:7078/api',
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
});


instance.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token && token !== 'null' && token !== 'undefined') {
    config.headers.Authorization = `Bearer ${token}`;
    console.log('Attaching token:', config.headers.Authorization);
  } else {
    console.log('No valid token found in localStorage');
  }
  return config;
});

export default instance;
