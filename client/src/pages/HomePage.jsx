// // src/pages/HomePage.jsx
import { useNavigate } from 'react-router-dom';
import { Link } from 'react-router-dom';
import { Heart, Calendar, User, FileText, Phone, Mail, MapPin, Clock, Award, Shield } from 'lucide-react';

export default function HomePage() {
  const navigate = useNavigate();
  
  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

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
          
          <div className="flex items-center space-x-4">
            <div className="hidden md:flex items-center space-x-6 text-sm text-gray-600">
              <div className="flex items-center space-x-1">
                <Phone className="h-4 w-4" />
                <span>1-800-HEALTH</span>
              </div>
              <div className="flex items-center space-x-1">
                <Clock className="h-4 w-4" />
                <span>24/7 Support</span>
              </div>
            </div>
            
            <button
              onClick={handleLogout}
              className="bg-gradient-to-r from-red-500 to-red-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:from-red-600 hover:to-red-700 transition-all duration-200 shadow-md hover:shadow-lg"
            >
              Logout
            </button>
          </div>
        </div>
      </div>
    </header>
  );

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
      <Header />
      
      <section className="relative py-16 px-4 sm:px-6 lg:px-8">
        <div className="max-w-7xl mx-auto">
          <div className="text-center mb-12">
            <h2 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
              Welcome to
              <span className="block bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
                MediCare Pro
              </span>
            </h2>
            <p className="text-xl text-gray-600 max-w-3xl mx-auto leading-relaxed">
              Your advanced digital healthcare solution. Appointments, medical tracking, and premium services all in one place.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-16">
            <Link 
              to="/appointments/book"
              className="group bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer border border-gray-100 hover:border-blue-200 transform hover:-translate-y-1 text-decoration-none"
            >
              <div className="bg-gradient-to-r from-blue-500 to-blue-600 p-3 rounded-xl w-fit mb-4 group-hover:from-blue-600 group-hover:to-blue-700 transition-all duration-300">
                <Calendar className="h-6 w-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">Book Appointment</h3>
              <p className="text-gray-600 text-sm">Schedule your medical appointment quickly and easily</p>
              <div className="mt-4 text-blue-600 text-sm font-medium group-hover:text-blue-700">
                Click to book →
              </div>
            </Link>

            <Link 
              to="/appointments/mine"
              className="group bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer border border-gray-100 hover:border-green-200 transform hover:-translate-y-1 text-decoration-none"
            >
              <div className="bg-gradient-to-r from-green-500 to-green-600 p-3 rounded-xl w-fit mb-4 group-hover:from-green-600 group-hover:to-green-700 transition-all duration-300">
                <FileText className="h-6 w-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">My Appointments</h3>
              <p className="text-gray-600 text-sm">View and manage your existing appointments</p>
              <div className="mt-4 text-green-600 text-sm font-medium group-hover:text-green-700">
                View appointments →
              </div>
            </Link>

            <Link 
              to="/services"
              className="group bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer border border-gray-100 hover:border-purple-200 transform hover:-translate-y-1 text-decoration-none"
            >
              <div className="bg-gradient-to-r from-purple-500 to-purple-600 p-3 rounded-xl w-fit mb-4 group-hover:from-purple-600 group-hover:to-purple-700 transition-all duration-300">
                <Award className="h-6 w-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">Medical Services</h3>
              <p className="text-gray-600 text-sm">Browse all available medical services</p>
              <div className="mt-4 text-purple-600 text-sm font-medium group-hover:text-purple-700">
                View services →
              </div>
            </Link>

            <Link 
              to="/profile"
              className="group bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer border border-gray-100 hover:border-orange-200 transform hover:-translate-y-1 text-decoration-none"
            >
              <div className="bg-gradient-to-r from-orange-500 to-orange-600 p-3 rounded-xl w-fit mb-4 group-hover:from-orange-600 group-hover:to-orange-700 transition-all duration-300">
                <User className="h-6 w-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">My Profile</h3>
              <p className="text-gray-600 text-sm">Edit your personal information</p>
              <div className="mt-4 text-orange-600 text-sm font-medium group-hover:text-orange-700">
                Edit profile →
              </div>
            </Link>
          </div>

          <div className="bg-white/60 backdrop-blur-sm rounded-3xl p-8 border border-gray-100">
            <h3 className="text-2xl font-bold text-center text-gray-900 mb-8">
              Why Choose MediCare Pro?
            </h3>
            
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="text-center">
                <div className="bg-gradient-to-r from-blue-100 to-blue-200 p-4 rounded-2xl w-fit mx-auto mb-4">
                  <Shield className="h-8 w-8 text-blue-600" />
                </div>
                <h4 className="text-lg font-semibold text-gray-900 mb-2">Advanced Security</h4>
                <p className="text-gray-600 text-sm">Complete protection for your privacy and medical data</p>
              </div>
              
              <div className="text-center">
                <div className="bg-gradient-to-r from-green-100 to-green-200 p-4 rounded-2xl w-fit mx-auto mb-4">
                  <Clock className="h-8 w-8 text-green-600" />
                </div>
                <h4 className="text-lg font-semibold text-gray-900 mb-2">24/7 Availability</h4>
                <p className="text-gray-600 text-sm">Access the system anytime, anywhere</p>
              </div>
              
              <div className="text-center">
                <div className="bg-gradient-to-r from-purple-100 to-purple-200 p-4 rounded-2xl w-fit mx-auto mb-4">
                  <Heart className="h-8 w-8 text-purple-600" />
                </div>
                <h4 className="text-lg font-semibold text-gray-900 mb-2">Personal Care</h4>
                <p className="text-gray-600 text-sm">Personalized tracking tailored to your medical needs</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <footer className="bg-gray-900 text-white py-12">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            <div>
              <div className="flex items-center space-x-3 mb-4">
                <div className="bg-gradient-to-r from-blue-600 to-green-600 p-2 rounded-xl">
                  <Heart className="h-5 w-5 text-white" />
                </div>
                <h3 className="text-lg font-bold">MediCare Pro</h3>
              </div>
              <p className="text-gray-400 text-sm">
                Your advanced digital healthcare solution
              </p>
            </div>
            
            <div>
              <h4 className="text-lg font-semibold mb-4">Services</h4>
              <ul className="space-y-2 text-sm text-gray-400">
                <li>Appointment Booking</li>
                <li>Medical Consultation</li>
                <li>Laboratory Tests</li>
                <li>Medical Imaging</li>
              </ul>
            </div>
            
            <div>
              <h4 className="text-lg font-semibold mb-4">Information</h4>
              <ul className="space-y-2 text-sm text-gray-400">
                <li>About Us</li>
                <li>Medical Team</li>
                <li>Locations</li>
                <li>Operating Hours</li>
              </ul>
            </div>
            
            <div>
              <h4 className="text-lg font-semibold mb-4">Contact</h4>
              <div className="space-y-3 text-sm text-gray-400">
                <div className="flex items-center space-x-2">
                  <Phone className="h-4 w-4" />
                  <span>1-800-HEALTH</span>
                </div>
                <div className="flex items-center space-x-2">
                  <Mail className="h-4 w-4" />
                  <span>info@medicare.pro</span>
                </div>
                <div className="flex items-center space-x-2">
                  <MapPin className="h-4 w-4" />
                  <span>123 Medical Street, New York</span>
                </div>
              </div>
            </div>
          </div>
          
          <div className="border-t border-gray-800 mt-8 pt-8 text-center text-sm text-gray-400">
            <p>&copy; 2024 MediCare Pro. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  );
}