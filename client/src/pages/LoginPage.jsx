// src/pages/LoginPage.jsx
import { useState } from 'react';

import { useNavigate } from 'react-router-dom';
import { login } from '../services/authService';

console.log("LoginPage loaded");

function LoginPage() {
  const [patientId, setPatientId] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    console.log('Login clicked!');

    setError('');

    try {
      const result = await login(patientId, password);

      // Save token to local storage
      localStorage.setItem('token', result.token);
      console.log('Token saved:', result.token);
c      // Redirect to home page
      navigate('/home');
    } catch (err) {
      console.error('Login error:', err.message);
      setError(`Login failed: ${err.message}`);
    }
  };

  return (
    <div style={{ maxWidth: 300, margin: 'auto', marginTop: 100 }}>
      <h2>Login</h2>
      <form onSubmit={handleLogin}>
        <div>
          <label>ID:</label><br />
          <input
            type="text"
            value={patientId}
            onChange={(e) => setPatientId(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Password:</label><br />
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        {error && <div style={{ color: 'red' }}>{error}</div>}
        <button type="submit">connect</button>
      </form>
    </div>
  );
}

export default LoginPage;

