// import  { useEffect, useState } from 'react';
// import { getAllServices } from '../services/servicesService';

// export default function ServicesPage()
//  {
//   const [services, setServices] = useState([]);
//  useEffect(() => {
//   getAllServices()
//     .then(data => {
//       setServices(data);
//     })
//     .catch(console.error);
// }, []);

//   return (
//   <div>
//     <h2>Available Medical Services</h2>
//     <ul>
//       {Array.isArray(services) && services.map((s) => (
//         <li key={s.serviceId}>{s.serviceName}</li>  // שימי לב לשמות השדות
//       ))}
//     </ul>
//   </div>
// );
// }
      import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Heart, ArrowLeft, Calendar } from 'lucide-react';
import { getAllServices } from '../services/servicesService';

export default function ServicesPage() {
  const navigate = useNavigate();
  const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    getAllServices()
      .then(data => {
        setServices(Array.isArray(data) ? data : []);
        setError(null);
      })
      .catch(err => {
        console.error('Error loading services:', err);
        setError('Failed to load services');
      })
      .finally(() => setLoading(false));
  }, []);

  // Header Component
  const Header = () => (
    <header className="bg-white/80 backdrop-blur-md shadow-sm sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          <div className="flex items-center space-x-3">
            <div className="bg-gradient-to-r from-blue-600 to-green-600 p-2 rounded-xl">
              <Heart className="h-6 w-6 text-white" />
            </div>
            <h1 className="text-xl font-bold bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
              MediCare Pro
            </h1>
          </div>
          
          <button
            onClick={() => {
              localStorage.removeItem('token');
              navigate('/');
            }}
            className="bg-gradient-to-r from-red-500 to-red-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:from-red-600 hover:to-red-700 transition-all duration-200 shadow-md hover:shadow-lg"
          >
            Logout
          </button>
        </div>
      </div>
    </header>
  );

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
        <Header />
        <div className="flex items-center justify-center min-h-[60vh]">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Loading services...</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
      <Header />
      
      <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <button
          onClick={() => navigate('/home')}
          className="flex items-center space-x-2 text-blue-600 hover:text-blue-700 mb-6 transition-colors duration-200"
        >
          <ArrowLeft className="h-4 w-4" />
          <span>Back to Home</span>
        </button>

        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            Our Medical Services
          </h1>
          <p className="text-xl text-gray-600">
            Wide range of medical services at the highest level
          </p>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-2xl p-6 mb-8">
            <p className="text-red-600 font-medium">{error}</p>
          </div>
        )}

        {services.length === 0 && !error && !loading ? (
          <div className="bg-white rounded-3xl shadow-lg p-12 text-center">
            <div className="text-6xl mb-6">🏥</div>
            <h3 className="text-2xl font-bold text-gray-900 mb-4">No Services Available</h3>
            <p className="text-gray-600">Please try again later or contact support</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {services.map((service, index) => (
              <div
                key={service.serviceId || `service-${index}`}
                className="bg-white rounded-2xl shadow-lg p-8 border border-gray-100 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1"
              >
                <div className="text-6xl mb-4 text-center">🩺</div>
                <h3 className="text-xl font-bold text-gray-900 mb-4 text-center">
                  {service.serviceName}
                </h3>
                <p className="text-gray-600 text-center mb-6 leading-relaxed">
                  Professional {service.serviceName?.toLowerCase()} services with experienced specialists
                </p>
                <button
                  onClick={() => navigate('/appointments/book')}
                  className="w-full bg-gradient-to-r from-blue-600 to-blue-700 text-white py-3 px-4 rounded-xl font-medium hover:from-blue-700 hover:to-blue-800 transition-all duration-200 shadow-md hover:shadow-lg flex items-center justify-center space-x-2"
                >
                  <Calendar className="h-5 w-5" />
                  <span>Book Appointment</span>
                </button>
              </div>
            ))}
          </div>
        )}

        {/* Additional Information Section */}
        {services.length > 0 && (
          <div className="mt-16 bg-blue-50 rounded-3xl p-8 border border-blue-200">
            <h3 className="text-2xl font-bold text-blue-900 mb-6 text-center">
              Why Choose Our Services?
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="text-center">
                <div className="bg-blue-600 text-white rounded-full w-12 h-12 flex items-center justify-center mx-auto mb-4">
                  <span className="text-xl">👨‍⚕️</span>
                </div>
                <h4 className="text-lg font-semibold text-blue-900 mb-2">Expert Doctors</h4>
                <p className="text-blue-800 text-sm">Highly qualified medical professionals with years of experience</p>
              </div>
              
              <div className="text-center">
                <div className="bg-blue-600 text-white rounded-full w-12 h-12 flex items-center justify-center mx-auto mb-4">
                  <span className="text-xl">🏥</span>
                </div>
                <h4 className="text-lg font-semibold text-blue-900 mb-2">Modern Equipment</h4>
                <p className="text-blue-800 text-sm">State-of-the-art medical technology for accurate diagnosis</p>
              </div>
              
              <div className="text-center">
                <div className="bg-blue-600 text-white rounded-full w-12 h-12 flex items-center justify-center mx-auto mb-4">
                  <span className="text-xl">⭐</span>
                </div>
                <h4 className="text-lg font-semibold text-blue-900 mb-2">Quality Care</h4>
                <p className="text-blue-800 text-sm">Personalized treatment plans tailored to your specific needs</p>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}