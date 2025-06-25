export async function login(email, password) {
  const response = await fetch('http://localhost:7078/api/auth/login', {  // <-- הוספתי await
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
body: JSON.stringify({ UserId: id, UserPassword: password }),
  });

  if (!response.ok) throw new Error('Login failed');
  return await response.json();
}
