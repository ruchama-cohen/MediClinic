import  { useEffect, useState } from 'react';
import { getAllServices } from '../services/servicesService';

export default function ServicesPage()
 {
  const [services, setServices] = useState([]);
 useEffect(() => {
  getAllServices()
    .then(data => {
      setServices(data);
    })
    .catch(console.error);
}, []);

  return (
  <div>
    <h2>Available Medical Services</h2>
    <ul>
      {Array.isArray(services) && services.map((s) => (
        <li key={s.serviceId}>{s.serviceName}</li>  // שימי לב לשמות השדות
      ))}
    </ul>
  </div>
);
}
      