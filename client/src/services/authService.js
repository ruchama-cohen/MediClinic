// src/services/authService.js
export async function login(email, password) {
  const response = await fetch('http://localhost:7078/api/AuthController/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  });

  if (!response.ok) throw new Error('Login failed');
  return await response.json();
}
