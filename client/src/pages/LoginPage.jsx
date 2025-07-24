// // src/pages/LoginPage.jsx
// import { useState } from 'react';

// import { useNavigate } from 'react-router-dom';
// import { login } from '../services/authService';

// console.log("LoginPage loaded");

// function LoginPage() {
//   const [patientId, setPatientId] = useState('');
//   const [password, setPassword] = useState('');
//   const [error, setError] = useState('');
//   const navigate = useNavigate();

//   const handleLogin = async (e) => {
//     e.preventDefault();
//     console.log('Login clicked!');

//     setError('');

//     try {
//       const result = await login(patientId, password);

//       // Save token to local storage
//       localStorage.setItem('token', result.token);
//       console.log('Token saved:', result.token);
//      // Redirect to home page
//       navigate('/home');
//     } catch (err) {
//       console.error('Login error:', err.message);
//       setError(`Login failed: ${err.message}`);
//     }
//   };

//   return (
//     <div style={{ maxWidth: 300, margin: 'auto', marginTop: 100 }}>
//       <h2>Login</h2>
//       <form onSubmit={handleLogin}>
//         <div>
//           <label>ID:</label><br />
//           <input
//             type="text"
//             value={patientId}
//             onChange={(e) => setPatientId(e.target.value)}
//             required
//           />
//         </div>
//         <div>
//           <label>Password:</label><br />
//           <input
//             type="password"
//             value={password}
//             onChange={(e) => setPassword(e.target.value)}
//             required
//           />
//         </div>
//         {error && <div style={{ color: 'red' }}>{error}</div>}
//         <button type="submit">connect</button>
//       </form>
//     </div>
//   );
// }

// export default LoginPage;

// src/pages/LoginPage.jsx
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Heart, User, Lock, LogIn, Shield, Clock, Award } from 'lucide-react';
import { login } from '../services/authService';

console.log("LoginPage loaded");

function LoginPage() {
  const [patientId, setPatientId] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    console.log('Login clicked!');

    setError('');
    setLoading(true);

    try {
      const result = await login(patientId, password);

      // Save token to local storage
      localStorage.setItem('token', result.token);
      console.log('Token saved:', result.token);
      
      // Redirect to home page
      navigate('/home');
    } catch (err) {
      console.error('Login error:', err.message);
      setError(`Login failed: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
      {/* Header */}
      <header className="bg-white/80 backdrop-blur-md shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-center items-center h-16">
            <div className="flex items-center space-x-3">
              <div className="bg-gradient-to-r from-blue-600 to-green-600 p-2 rounded-xl">
                <Heart className="h-6 w-6 text-white" />
              </div>
              <h1 className="text-xl font-bold bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
                MediCare Pro
              </h1>
            </div>
          </div>
        </div>
      </header>

      <div className="flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
        <div className="max-w-6xl w-full">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
            
            {/* Left Side - Welcome Message */}
            <div className="text-center lg:text-left">
              <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-6">
                Welcome to
                <span className="block bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
                  MediCare Pro
                </span>
              </h2>
              <p className="text-xl text-gray-600 mb-8 leading-relaxed">
                Your advanced digital healthcare solution. Access appointments, medical records, and premium services all in one secure platform.
              </p>

              {/* Features */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                <div className="text-center">
                  <div className="bg-gradient-to-r from-blue-100 to-blue-200 p-4 rounded-2xl w-fit mx-auto mb-3">
                    <Shield className="h-8 w-8 text-blue-600" />
                  </div>
                  <h3 className="font-semibold text-gray-900 mb-2">Secure</h3>
                  <p className="text-sm text-gray-600">Advanced security for your medical data</p>
                </div>
                
                <div className="text-center">
                  <div className="bg-gradient-to-r from-green-100 to-green-200 p-4 rounded-2xl w-fit mx-auto mb-3">
                    <Clock className="h-8 w-8 text-green-600" />
                  </div>
                  <h3 className="font-semibold text-gray-900 mb-2">24/7 Access</h3>
                  <p className="text-sm text-gray-600">Available anytime, anywhere</p>
                </div>
                
                <div className="text-center">
                  <div className="bg-gradient-to-r from-purple-100 to-purple-200 p-4 rounded-2xl w-fit mx-auto mb-3">
                    <Award className="h-8 w-8 text-purple-600" />
                  </div>
                  <h3 className="font-semibold text-gray-900 mb-2">Premium Care</h3>
                  <p className="text-sm text-gray-600">Personalized healthcare experience</p>
                </div>
              </div>
            </div>

            {/* Right Side - Login Form */}
            <div className="max-w-md mx-auto w-full">
              <div className="bg-white rounded-3xl shadow-2xl p-8 border border-gray-100">
                <div className="text-center mb-8">
                  <div className="bg-gradient-to-r from-blue-600 to-green-600 p-3 rounded-2xl w-fit mx-auto mb-4">
                    <LogIn className="h-8 w-8 text-white" />
                  </div>
                  <h3 className="text-2xl font-bold text-gray-900 mb-2">Welcome Back</h3>
                  <p className="text-gray-600">Please sign in to your account</p>
                </div>

                <form onSubmit={handleLogin} className="space-y-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Patient ID
                    </label>
                    <div className="relative">
                      <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                        <User className="h-5 w-5 text-gray-400" />
                      </div>
                      <input
                        type="text"
                        value={patientId}
                        onChange={(e) => setPatientId(e.target.value)}
                        className="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200"
                        placeholder="Enter your patient ID"
                        required
                      />
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Password
                    </label>
                    <div className="relative">
                      <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                        <Lock className="h-5 w-5 text-gray-400" />
                      </div>
                      <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        className="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200"
                        placeholder="Enter your password"
                        required
                      />
                    </div>
                  </div>

                  {error && (
                    <div className="bg-red-50 border border-red-200 rounded-xl p-4">
                      <p className="text-red-600 text-sm font-medium">{error}</p>
                    </div>
                  )}

                  <button
                    type="submit"
                    disabled={loading}
                    className={`w-full py-3 px-4 rounded-xl font-medium transition-all duration-200 flex items-center justify-center space-x-2 ${
                      loading
                        ? 'bg-gray-400 text-white cursor-not-allowed'
                        : 'bg-gradient-to-r from-blue-600 to-green-600 text-white hover:from-blue-700 hover:to-green-700 shadow-lg hover:shadow-xl transform hover:-translate-y-0.5'
                    }`}
                  >
                    {loading ? (
                      <>
                        <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                        <span>Signing In...</span>
                      </>
                    ) : (
                      <>
                        <LogIn className="h-5 w-5" />
                        <span>Sign In</span>
                      </>
                    )}
                  </button>
                </form>

                <div className="mt-8 pt-6 border-t border-gray-200 text-center">
                  <p className="text-sm text-gray-600">
                    Need help? Contact our support team
                  </p>
                  <p className="text-sm text-blue-600 font-medium mt-1">
                    📞 1-800-HEALTH
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Footer */}
      <footer className="bg-gray-900 text-white py-8 mt-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <div className="flex items-center justify-center space-x-3 mb-4">
              <div className="bg-gradient-to-r from-blue-600 to-green-600 p-2 rounded-xl">
                <Heart className="h-5 w-5 text-white" />
              </div>
              <h3 className="text-lg font-bold">MediCare Pro</h3>
            </div>
            <p className="text-gray-400 text-sm mb-4">
              Your advanced digital healthcare solution
            </p>
            <p className="text-gray-500 text-xs">
              &copy; 2024 MediCare Pro. All rights reserved.
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
}

export default LoginPage;