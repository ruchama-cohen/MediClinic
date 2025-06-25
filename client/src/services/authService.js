export async function login(id, password) {
  const response = await fetch('https://localhost:7078/api/auth/login', {  // שינוי ל-HTTPS

    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ UserId: id, UserPassword: password }),
  });

  if (!response.ok) throw new Error('Login failed');
  return await response.json();
}
