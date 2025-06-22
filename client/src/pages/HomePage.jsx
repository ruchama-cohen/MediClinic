// src/pages/HomePage.jsx
import React from 'react';
import { useNavigate } from 'react-router-dom';

function HomePage() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('token'); // Clear the token
    navigate('/'); // Go back to login page
  };

  return (
    <div style={{ textAlign: 'center', marginTop: 100 }}>
      <h1>Welcome to the Clinic System</h1>
      <p>You are successfully logged in.</p>
      <button onClick={handleLogout}>Logout</button>
    </div>
  );
}

export default HomePage;
