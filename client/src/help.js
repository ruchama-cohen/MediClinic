import React, { useState, useEffect } from 'react';
import { Heart, Calendar, User, FileText, Phone, Mail, MapPin, Clock, Award, Shield, Search, CheckCircle, X, ArrowRight, Home } from 'lucide-react';

const ModernClinicSystem = () => {
  const [currentPage, setCurrentPage] = useState('home');

  // API configuration
  const API_BASE_URL = 'https://localhost:7078/api';

  // Utility function to get patient key from JWT token
  const getPatientKeyFromToken = () => {
    const token = localStorage.getItem('token');
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.PatientKey;
    } catch (err) {
      console.error("Error decoding token:", err);
      return null;
    }
  };

  // API service functions
  const apiService = {
    getAvailableServices: async () => {
      const response = await fetch(`${API_BASE_URL}/appointments/services`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.json();
    },

    getProvidersByService: async (serviceId) => {
      const response = await fetch(`${API_BASE_URL}/appointments/providers/${serviceId}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.json();
    },

    getAvailableCities: async () => {
      const response = await fetch(`${API_BASE_URL}/appointments/cities`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.json();
    },

    getTimePeriods: async () => {
      const response = await fetch(`${API_BASE_URL}/appointments/time-periods`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.json();
    },

    searchAvailableSlots: async (searchParams) => {
      const response = await fetch(`${API_BASE_URL}/appointments/search`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(searchParams)
      });
      return response.json();
    },

    bookAppointment: async (slotId, patientKey) => {
      const response = await fetch(`${API_BASE_URL}/appointments/book/${slotId}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ PatientKey: patientKey })
      });
      return response.json();
    },

    getAppointmentsByUser: async (patientKey) => {
      const response = await fetch(`${API_BASE_URL}/appointments/byUserKey/${patientKey}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.json();
    },

    cancelAppointment: async (appointmentId) => {
      const response = await fetch(`${API_BASE_URL}/appointments/${appointmentId}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.json();
    },

    getAllServices: async () => {
      const response = await fetch(`${API_BASE_URL}/service`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.json();
    }
  };

  // Header Component
  const Header = () => (
    <header className="bg-white/80 backdrop-blur-md shadow-sm sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          <div 
            className="flex items-center space-x-3 cursor-pointer"
            onClick={() => setCurrentPage('home')}
          >
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
              onClick={() => {
                localStorage.removeItem('token');
                window.location.href = '/';
              }}
              className="bg-gradient-to-r from-red-500 to-red-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:from-red-600 hover:to-red-700 transition-all duration-200 shadow-md hover:shadow-lg"
            >
              Logout
            </button>
          </div>
        </div>
      </div>
    </header>
  );

  // Home Page
  const HomePage = () => (
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
            <div 
              onClick={() => setCurrentPage('booking')}
              className="group bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer border border-gray-100 hover:border-blue-200 transform hover:-translate-y-1"
            >
              <div className="bg-gradient-to-r from-blue-500 to-blue-600 p-3 rounded-xl w-fit mb-4 group-hover:from-blue-600 group-hover:to-blue-700 transition-all duration-300">
                <Calendar className="h-6 w-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">Book Appointment</h3>
              <p className="text-gray-600 text-sm">Schedule your medical appointment quickly and easily</p>
              <div className="mt-4 text-blue-600 text-sm font-medium group-hover:text-blue-700">
                Click to book →
              </div>
            </div>

            <div 
              onClick={() => setCurrentPage('appointments')}
              className="group bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer border border-gray-100 hover:border-green-200 transform hover:-translate-y-1"
            >
              <div className="bg-gradient-to-r from-green-500 to-green-600 p-3 rounded-xl w-fit mb-4 group-hover:from-green-600 group-hover:to-green-700 transition-all duration-300">
                <FileText className="h-6 w-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">My Appointments</h3>
              <p className="text-gray-600 text-sm">View and manage your existing appointments</p>
              <div className="mt-4 text-green-600 text-sm font-medium group-hover:text-green-700">
                View appointments →
              </div>
            </div>

            <div 
              onClick={() => setCurrentPage('services')}
              className="group bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer border border-gray-100 hover:border-purple-200 transform hover:-translate-y-1"
            >
              <div className="bg-gradient-to-r from-purple-500 to-purple-600 p-3 rounded-xl w-fit mb-4 group-hover:from-purple-600 group-hover:to-purple-700 transition-all duration-300">
                <Award className="h-6 w-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">Medical Services</h3>
              <p className="text-gray-600 text-sm">Browse all available medical services</p>
              <div className="mt-4 text-purple-600 text-sm font-medium group-hover:text-purple-700">
                View services →
              </div>
            </div>

            <div 
              onClick={() => setCurrentPage('profile')}
              className="group bg-white rounded-2xl p-6 shadow-lg hover:shadow-2xl transition-all duration-300 cursor-pointer border border-gray-100 hover:border-orange-200 transform hover:-translate-y-1"
            >
              <div className="bg-gradient-to-r from-orange-500 to-orange-600 p-3 rounded-xl w-fit mb-4 group-hover:from-orange-600 group-hover:to-orange-700 transition-all duration-300">
                <User className="h-6 w-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold text-gray-900 mb-2">My Profile</h3>
              <p className="text-gray-600 text-sm">Edit your personal information</p>
              <div className="mt-4 text-orange-600 text-sm font-medium group-hover:text-orange-700">
                Edit profile →
              </div>
            </div>
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

  // Appointment Booking Page
  const AppointmentBookingPage = () => {
    const [selectedService, setSelectedService] = useState('');
    const [selectedProvider, setSelectedProvider] = useState('');
    const [selectedCity, setSelectedCity] = useState('');
    const [selectedTimePeriod, setSelectedTimePeriod] = useState('');
    const [selectedDate, setSelectedDate] = useState('');
    
    const [services, setServices] = useState([]);
    const [providers, setProviders] = useState([]);
    const [cities, setCities] = useState([]);
    const [timePeriods, setTimePeriods] = useState([]);
    const [availableSlots, setAvailableSlots] = useState([]);
    
    const [loading, setLoading] = useState(true);
    const [searching, setSearching] = useState(false);
    const [showFilters, setShowFilters] = useState(false);

    useEffect(() => {
      loadInitialData();
    }, []);

    const loadInitialData = async () => {
      try {
        setLoading(true);
        const [servicesRes, citiesRes, timePeriodsRes] = await Promise.all([
          apiService.getAvailableServices(),
          apiService.getAvailableCities(),
          apiService.getTimePeriods()
        ]);
        
        setServices(servicesRes.data || []);
        setCities(citiesRes.data || []);
        setTimePeriods(timePeriodsRes.data || []);
      } catch (error) {
        console.error('Error loading initial data:', error);
        alert('Error loading page data. Please refresh the page.');
      } finally {
        setLoading(false);
      }
    };

    const handleServiceChange = async (serviceId) => {
      setSelectedService(serviceId);
      setSelectedProvider('');
      setProviders([]);
      setAvailableSlots([]);
      
      if (serviceId) {
        try {
          const providersRes = await apiService.getProvidersByService(serviceId);
          setProviders(providersRes.data || []);
          setShowFilters(true);
        } catch (error) {
          console.error('Error loading providers:', error);
          alert('Error loading providers for this service.');
        }
      } else {
        setShowFilters(false);
      }
    };

    const handleSearch = async () => {
      if (!selectedService) {
        alert('Please select a service first.');
        return;
      }
      
      const searchParams = {
        ServiceId: parseInt(selectedService)
      };
      
      if (selectedProvider) searchParams.ProviderKey = parseInt(selectedProvider);
      if (selectedCity) searchParams.CityName = selectedCity;
      if (selectedTimePeriod) searchParams.TimePeriod = selectedTimePeriod;
      if (selectedDate) searchParams.PreferredDate = selectedDate;

      try {
        setSearching(true);
        const result = await apiService.searchAvailableSlots(searchParams);
        setAvailableSlots(result.data || []);
        
        if (!result.data || result.data.length === 0) {
          alert('No available appointments found. Try adjusting your filters.');
        }
      } catch (error) {
        console.error('Error searching slots:', error);
        alert('Error searching for appointments. Please try again.');
      } finally {
        setSearching(false);
      }
    };

    const handleBook = async (slotId) => {
      const patientKey = getPatientKeyFromToken();
      if (!patientKey) {
        alert('Please log in again.');
        return;
      }
      if (!window.confirm('Are you sure you want to book this appointment?')) return;
      
      try {
        await apiService.bookAppointment(slotId, patientKey);
        alert('Appointment successfully booked!');
        handleSearch();
      } catch (error) {
        console.error('Error booking appointment:', error);
        alert('Failed to book appointment. Please try again.');
      }
    };

    const formatDate = (dateStr) => {
      try {
        const date = new Date(dateStr);
        return date.toLocaleDateString('en-US', {
          weekday: 'long',
          year: 'numeric',
          month: 'long',
          day: 'numeric'
        });
      } catch (error) {
        return 'Invalid Date';
      }
    };

    const formatTime = (timeStr) => {
      return timeStr?.substring(0, 5) || 'N/A';
    };

    if (loading) {
      return (
        <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
          <Header />
          <div className="flex items-center justify-center min-h-[60vh]">
            <div className="text-center">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
              <p className="text-gray-600">Loading data...</p>
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
            onClick={() => setCurrentPage('home')}
            className="flex items-center space-x-2 text-blue-600 hover:text-blue-700 mb-6"
          >
            <ArrowRight className="h-4 w-4" />
            <span>Back to Home</span>
          </button>

          <div className="text-center mb-12">
            <h1 className="text-4xl font-bold text-gray-900 mb-4">
              Book Medical Appointment
            </h1>
            <p className="text-xl text-gray-600">
              Choose the medical service you need and book an appointment easily
            </p>
          </div>

          <div className="bg-white rounded-3xl shadow-lg p-8 mb-8">
            <h2 className="text-2xl font-bold text-gray-900 mb-6">Select Medical Service *</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {services.map((service, index) => (
                <div
                  key={service.serviceId || index}
                  onClick={() => handleServiceChange(service.serviceId)}
                  className={`p-6 rounded-2xl border-2 cursor-pointer transition-all duration-300 transform hover:-translate-y-1 ${
                    selectedService === service.serviceId
                      ? 'border-blue-500 bg-blue-50 shadow-lg'
                      : 'border-gray-200 hover:border-blue-300 hover:shadow-md'
                  }`}
                >
                  <div className="text-4xl mb-3">🩺</div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-2">{service.serviceName}</h3>
                  <p className="text-sm text-gray-600">Click to select</p>
                </div>
              ))}
            </div>
          </div>

          {showFilters && (
            <div className="bg-white rounded-3xl shadow-lg p-8 mb-8">
              <h2 className="text-2xl font-bold text-gray-900 mb-6">Optional Filters</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Specific Provider</label>
                  <select 
                    className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    value={selectedProvider}
                    onChange={(e) => setSelectedProvider(e.target.value)}
                  >
                    <option value="">Any Provider</option>
                    {providers.map((provider, index) => (
                      <option key={provider.providerKey || index} value={provider.providerKey}>
                        {provider.providerName}
                      </option>
                    ))}
                  </select>
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">City</label>
                  <select 
                    className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    value={selectedCity}
                    onChange={(e) => setSelectedCity(e.target.value)}
                  >
                    <option value="">Any City</option>
                    {cities.map((city, index) => (
                      <option key={city.cityId || index} value={city.cityName}>
                        {city.cityName}
                      </option>
                    ))}
                  </select>
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Preferred Time</label>
                  <select 
                    className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    value={selectedTimePeriod}
                    onChange={(e) => setSelectedTimePeriod(e.target.value)}
                  >
                    <option value="">Any Time</option>
                    {timePeriods.map((period, index) => (
                      <option key={period.value || index} value={period.value}>
                        {period.label}
                      </option>
                    ))}
                  </select>
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Preferred Date</label>
                  <input 
                    type="date"
                    className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    value={selectedDate}
                    onChange={(e) => setSelectedDate(e.target.value)}
                    min={new Date().toISOString().split('T')[0]}
                  />
                </div>
              </div>
              
              <div className="flex space-x-4 mt-6">
                <button
                  onClick={handleSearch}
                  disabled={searching}
                  className="bg-gradient-to-r from-blue-600 to-blue-700 text-white px-8 py-3 rounded-xl font-medium hover:from-blue-700 hover:to-blue-800 transition-all duration-200 shadow-lg hover:shadow-xl flex items-center space-x-2 disabled:opacity-50"
                >
                  <Search className="h-5 w-5" />
                  <span>{searching ? 'Searching...' : 'Search Available Appointments'}</span>
                </button>
                
                <button
                  onClick={() => {
                    setSelectedProvider('');
                    setSelectedCity('');
                    setSelectedTimePeriod('');
                    setSelectedDate('');
                    setAvailableSlots([]);
                  }}
                  className="bg-gray-500 text-white px-6 py-3 rounded-xl font-medium hover:bg-gray-600 transition-all duration-200"
                >
                  Clear Filters
                </button>
              </div>
            </div>
          )}

          {availableSlots.length > 0 && (
            <div className="bg-white rounded-3xl shadow-lg p-8">
              <h2 className="text-2xl font-bold text-gray-900 mb-6">
                Available Appointments ({availableSlots.length} found)
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {availableSlots.map((slot, index) => (
                  <div
                    key={slot.SlotId || slot.slotId || index}
                    className="p-6 rounded-2xl border-2 border-green-200 bg-green-50 hover:border-green-400 transition-all duration-300 transform hover:-translate-y-1"
                  >
                    <div className="mb-4">
                      <div className="text-2xl font-bold text-blue-600 mb-2">
                        {formatDate(slot.SlotDate || slot.slotDate)}
                      </div>
                      <div className="text-lg font-semibold text-gray-800">
                        {formatTime(slot.SlotStart || slot.slotStart)} - {formatTime(slot.SlotEnd || slot.slotEnd)}
                      </div>
                    </div>
                    
                    <div className="space-y-2 mb-4">
                      <div className="font-semibold text-gray-900">
                        {slot.ProviderName || slot.providerName || 'Unknown Provider'}
                      </div>
                      <div className="text-sm text-gray-600">
                        📍 {slot.BranchName || slot.branchName || 'Unknown Location'}
                      </div>
                      <div className="text-sm text-gray-600">
                        🏙️ {slot.CityName || slot.cityName || 'Unknown City'}
                      </div>
                      {(slot.Address || slot.address) && (
                        <div className="text-sm text-gray-600">
                          📮 {slot.Address || slot.address}
                        </div>
                      )}
                    </div>
                    
                    <button
                      onClick={() => handleBook(slot.SlotId || slot.slotId)}
                      className="w-full bg-gradient-to-r from-green-500 to-green-600 text-white py-3 px-4 rounded-xl font-medium hover:from-green-600 hover:to-green-700 transition-all duration-200 shadow-md hover:shadow-lg"
                    >
                      Book This Appointment
                    </button>
                  </div>
                ))}
              </div>
            </div>
          )}

          {availableSlots.length === 0 && selectedService && !searching && (
            <div className="bg-white rounded-3xl shadow-lg p-8 text-center">
              <div className="text-6xl mb-4">🔍</div>
              <h3 className="text-2xl font-bold text-gray-900 mb-4">No Appointments Found</h3>
              <p className="text-gray-600">Try adjusting your filters or selecting a different service</p>
            </div>
          )}

          {!selectedService && (
            <div className="bg-blue-50 rounded-3xl p-8 border border-blue-200">
              <h3 className="text-2xl font-bold text-blue-900 mb-4">How to Book an Appointment?</h3>
              <ol className="space-y-3 text-blue-800">
                <li className="flex items-start space-x-3">
                  <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm font-bold">1</span>
                  <span>Select the type of medical service you need</span>
                </li>
                <li className="flex items-start space-x-3">
                  <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm font-bold">2</span>
                  <span>Use filters to narrow down results (optional)</span>
                </li>
                <li className="flex items-start space-x-3">
                  <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm font-bold">3</span>
                  <span>Click "Search Available Appointments"</span>
                </li>
                <li className="flex items-start space-x-3">
                  <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm font-bold">4</span>
                  <span>Choose the suitable appointment and click "Book"</span>
                </li>
              </ol>
            </div>
          )}
        </div>
      </div>
    );
  };

  // My Appointments Page
  const MyAppointmentsPage = () => {
    const [appointments, setAppointments] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
      loadAppointments();
    }, []);

    const loadAppointments = async () => {
      try {
        setLoading(true);
        const patientKey = getPatientKeyFromToken();
        
        if (!patientKey) {
          alert('Please log in again');
          return;
        }

        const response = await apiService.getAppointmentsByUser(patientKey);
        setAppointments(response.data || []);
      } catch (err) {
        console.error('Failed to fetch appointments:', err);
        alert('Failed to load appointments');
      } finally {
        setLoading(false);
      }
    };

    const handleCancelAppointment = async (appointmentId) => {
      if (!window.confirm('Are you sure you want to cancel this appointment?')) return;
      
      try {
        await apiService.cancelAppointment(appointmentId);
        alert('Appointment cancelled successfully');
        setAppointments(prev => prev.filter(a => 
          (a.id || a.appointmentId) !== appointmentId
        ));
      } catch (error) {
        console.error('Failed to cancel appointment:', error);
        alert('Failed to cancel appointment. Please try again.');
      }
    };

    const formatDate = (dateStr) => {
      try {
        const date = new Date(dateStr);
        return date.toLocaleDateString('en-US', {
          weekday: 'long',
          year: 'numeric',
          month: 'long',
          day: 'numeric'
        });
      } catch (error) {
        return 'Invalid Date';
      }
    };

    if (loading) {
      return (
        <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
          <Header />
          <div className="flex items-center justify-center min-h-[60vh]">
            <div className="text-center">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
              <p className="text-gray-600">Loading appointments...</p>
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
            onClick={() => setCurrentPage('home')}
            className="flex items-center space-x-2 text-blue-600 hover:text-blue-700 mb-6"
          >
            <ArrowRight className="h-4 w-4" />
            <span>Back to Home</span>
          </button>

          <div className="text-center mb-12">
            <h1 className="text-4xl font-bold text-gray-900 mb-4">
              My Appointments
            </h1>
            <p className="text-xl text-gray-600">
              View and manage all your appointments
            </p>
          </div>

          {appointments.length === 0 ? (
            <div className="bg-white rounded-3xl shadow-lg p-12 text-center">
              <div className="text-6xl mb-6">📅</div>
              <h3 className="text-2xl font-bold text-gray-900 mb-4">No Appointments</h3>
              <p className="text-gray-600 mb-8">You haven't booked any appointments yet</p>
              <button
                onClick={() => setCurrentPage('booking')}
                className="bg-gradient-to-r from-blue-600 to-blue-700 text-white px-8 py-3 rounded-xl font-medium hover:from-blue-700 hover:to-blue-800 transition-all duration-200 shadow-lg hover:shadow-xl"
              >
                Book Your First Appointment
              </button>
            </div>
          ) : (
            <div className="space-y-6">
              {appointments.map((appointment) => {
                const slot = appointment.slot || {};
                const doctorName = slot.providerKeyNavigation?.name || 'Unknown Doctor';
                const location = slot.branch?.branchName || 'Unknown Location';
                const date = slot.slotDate || appointment.date || 'Unknown Date';
                const timeRaw = slot.slotStart || appointment.time || 'Unknown Time';
                const time = timeRaw && typeof timeRaw === 'string' && timeRaw.length >= 5 ? timeRaw.substring(0,5) : timeRaw;

                return (
                  <div
                    key={appointment.id || appointment.appointmentId}
                    className="bg-white rounded-2xl shadow-lg p-6 border border-gray-100 hover:shadow-xl transition-all duration-300"
                  >
                    <div className="flex justify-between items-start">
                      <div className="flex-1">
                        <div className="flex items-center space-x-4 mb-4">
                          <div className="bg-gradient-to-r from-green-500 to-green-600 p-3 rounded-xl">
                            <Calendar className="h-6 w-6 text-white" />
                          </div>
                          <div>
                            <h3 className="text-xl font-bold text-gray-900">
                              {formatDate(date)}
                            </h3>
                            <p className="text-gray-600">
                              Time: {time}
                            </p>
                          </div>
                        </div>
                        
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                          <div>
                            <p className="text-sm text-gray-500 mb-1">Doctor</p>
                            <p className="font-semibold text-gray-900">{doctorName}</p>
                          </div>
                          <div>
                            <p className="text-sm text-gray-500 mb-1">Location</p>
                            <p className="font-semibold text-gray-900">{location}</p>
                          </div>
                        </div>
                      </div>
                      
                      <button
                        onClick={() => handleCancelAppointment(appointment.id || appointment.appointmentId)}
                        className="bg-gradient-to-r from-red-500 to-red-600 text-white px-4 py-2 rounded-lg font-medium hover:from-red-600 hover:to-red-700 transition-all duration-200 shadow-md hover:shadow-lg flex items-center space-x-2"
                      >
                        <X className="h-4 w-4" />
                        <span>Cancel</span>
                      </button>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>
    );
  };

  // Services Page
  const ServicesPage = () => {
    const [services, setServices] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
      loadServices();
    }, []);

    const loadServices = async () => {
      try {
        setLoading(true);
        const response = await apiService.getAllServices();
        setServices(response.data || []);
      } catch (error) {
        console.error('Error loading services:', error);
        alert('Error loading services');
      } finally {
        setLoading(false);
      }
    };

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
            onClick={() => setCurrentPage('home')}
            className="flex items-center space-x-2 text-blue-600 hover:text-blue-700 mb-6"
          >
            <ArrowRight className="h-4 w-4" />
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

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {services.map((service) => (
              <div
                key={service.serviceId}
                className="bg-white rounded-2xl shadow-lg p-8 border border-gray-100 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1"
              >
                <div className="text-6xl mb-4 text-center">🩺</div>
                <h3 className="text-xl font-bold text-gray-900 mb-4 text-center">
                  {service.serviceName}
                </h3>
                <p className="text-gray-600 text-center mb-6 leading-relaxed">
                  Professional {service.serviceName.toLowerCase()} services with experienced specialists
                </p>
                <button
                  onClick={() => setCurrentPage('booking')}
                  className="w-full bg-gradient-to-r from-blue-600 to-blue-700 text-white py-3 px-4 rounded-xl font-medium hover:from-blue-700 hover:to-blue-800 transition-all duration-200 shadow-md hover:shadow-lg"
                >
                  Book Appointment
                </button>
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  };

  // Profile Page
  const ProfilePage = () => {
    const [patientData, setPatientData] = useState({
      name: 'John Smith',
      email: 'john@example.com',
      phone: '555-1234',
      address: '123 Main Street, New York'
    });
    const [editing, setEditing] = useState(false);

    const handleSave = () => {
      setEditing(false);
      alert('Information saved successfully!');
    };

    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
        <Header />
        
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <button
            onClick={() => setCurrentPage('home')}
            className="flex items-center space-x-2 text-blue-600 hover:text-blue-700 mb-6"
          >
            <ArrowRight className="h-4 w-4" />
            <span>Back to Home</span>
          </button>

          <div className="text-center mb-12">
            <h1 className="text-4xl font-bold text-gray-900 mb-4">
              My Profile
            </h1>
            <p className="text-xl text-gray-600">
              Edit and update your personal information
            </p>
          </div>

          <div className="bg-white rounded-3xl shadow-lg p-8">
            <div className="flex justify-between items-center mb-8">
              <h2 className="text-2xl font-bold text-gray-900">Personal Information</h2>
              <button
                onClick={() => setEditing(!editing)}
                className="bg-gradient-to-r from-blue-600 to-blue-700 text-white px-6 py-2 rounded-lg font-medium hover:from-blue-700 hover:to-blue-800 transition-all duration-200"
              >
                {editing ? 'Cancel' : 'Edit'}
              </button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Full Name</label>
                <input
                  type="text"
                  value={patientData.name}
                  onChange={(e) => setPatientData({...patientData, name: e.target.value})}
                  disabled={!editing}
                  className={`w-full p-3 border border-gray-300 rounded-xl ${
                    editing ? 'focus:ring-2 focus:ring-blue-500 focus:border-transparent' : 'bg-gray-50'
                  }`}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Email</label>
                <input
                  type="email"
                  value={patientData.email}
                  onChange={(e) => setPatientData({...patientData, email: e.target.value})}
                  disabled={!editing}
                  className={`w-full p-3 border border-gray-300 rounded-xl ${
                    editing ? 'focus:ring-2 focus:ring-blue-500 focus:border-transparent' : 'bg-gray-50'
                  }`}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Phone</label>
                <input
                  type="tel"
                  value={patientData.phone}
                  onChange={(e) => setPatientData({...patientData, phone: e.target.value})}
                  disabled={!editing}
                  className={`w-full p-3 border border-gray-300 rounded-xl ${
                    editing ? 'focus:ring-2 focus:ring-blue-500 focus:border-transparent' : 'bg-gray-50'
                  }`}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Address</label>
                <input
                  type="text"
                  value={patientData.address}
                  onChange={(e) => setPatientData({...patientData, address: e.target.value})}
                  disabled={!editing}
                  className={`w-full p-3 border border-gray-300 rounded-xl ${
                    editing ? 'focus:ring-2 focus:ring-blue-500 focus:border-transparent' : 'bg-gray-50'
                  }`}
                />
              </div>
            </div>

            {editing && (
              <div className="mt-8 flex justify-end space-x-4">
                <button
                  onClick={() => setEditing(false)}
                  className="bg-gray-500 text-white px-6 py-3 rounded-xl font-medium hover:bg-gray-600 transition-all duration-200"
                >
                  Cancel
                </button>
                <button
                  onClick={handleSave}
                  className="bg-gradient-to-r from-green-500 to-green-600 text-white px-6 py-3 rounded-xl font-medium hover:from-green-600 hover:to-green-700 transition-all duration-200"
                >
                  Save Changes
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    );
  };

  // Main render logic
  const renderCurrentPage = () => {
    switch (currentPage) {
      case 'booking':
        return <AppointmentBookingPage />;
      case 'appointments':
        return <MyAppointmentsPage />;
      case 'services':
        return <ServicesPage />;
      case 'profile':
        return <ProfilePage />;
      default:
        return <HomePage />;
    }
  };

  return renderCurrentPage();
};

export default ModernClinicSystem;