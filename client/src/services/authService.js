
export async function login(id, password) {
  try {
    const response = await fetch('https://localhost:7078/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      },
      body: JSON.stringify({
        UserId: id,
        UserPassword: password
      }),
    });

    const data = await response.json();
   
    if (!response.ok) {
      throw new Error(data.message || 'Login failed');
    }

    // בדוק שהתשובה מכילה token
    if (!data.token && !data.Token) {
      throw new Error('No token received from server');
    }

    return {
      token: data.token || data.Token,
      success: data.success,
      patient: data.patient || data.Patient
    };
  } catch (error) {
    console.error('Login service error:', error);
    throw error;
  }
}