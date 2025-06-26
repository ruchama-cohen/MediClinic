// src/pages/HomePage.jsx
import { useNavigate } from 'react-router-dom';
import { Link } from 'react-router-dom';

export default function HomePage() {
   const navigate = useNavigate();
  const handleLogout = () => {
    localStorage.removeItem('token'); // Clear the token
    navigate('/'); // Go back to login page
  };

  return (
    <div>
    <h1>Welcome</h1>
      <Link to="/appointments/book">Book Appointment</Link><br />
      <Link to="/appointments/mine">My Appointments</Link><br />
      <Link to="/services">Medical Services</Link><br />
      <Link to="/profile">My Profile</Link>
            <button onClick={handleLogout}>Logout</button>
    </div>    
  );
}

