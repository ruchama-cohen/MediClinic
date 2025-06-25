import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { login } from '../services/authService';

function LoginPage() {
  const [id, setID] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const result = await login(id, password);

      // Save token to local storage
      localStorage.setItem('token', result.token);

      // Redirect to home page
      navigate('/home');
    } catch (err) {
      if (err.response && err.response.status === 401) {
        setError('id or password incorrect');
      } else {
        setError('שגיאה כללית. נסה שוב');
      }
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
            value={id}
            onChange={(e) => setID(e.target.value)}
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
        <button type="submit">Login</button>
      </form>
    </div>
  );
}

export default LoginPage;
