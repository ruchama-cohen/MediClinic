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
  
  // הוסף לוג לראות את הנתיב המלא שנשלח
  console.log('Request URL:', config.baseURL + config.url);
  console.log('Request Method:', config.method);
  console.log('Request Data:', config.data);
  
  return config;
});

instance.interceptors.response.use(
  (response) => {
    console.log('Response received:', response.status, response.data);
    return response;
  },
  (error) => {
    console.error('Response error:', error.response?.status, error.response?.data);
    return Promise.reject(error);
  }
);

export default instance;