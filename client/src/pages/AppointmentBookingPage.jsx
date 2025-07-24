// import { useEffect, useState } from 'react';
// import {
//   getAvailableServices,
//   getProvidersByService,
//   getAvailableCities,
//   getTimePeriods,
//   searchAvailableSlots,
//   bookAppointment
// } from '../services/appointmentService.js';
// import { getPatientKeyFromToken } from '../utils/authUtils';

// export default function AppointmentBookingPage() {
//   const [services, setServices] = useState([]);
//   const [providers, setProviders] = useState([]);
//   const [cities, setCities] = useState([]);
//   const [timePeriods, setTimePeriods] = useState([]);
//   const [availableSlots, setAvailableSlots] = useState([]);

//   const [selectedService, setSelectedService] = useState('');
//   const [selectedProvider, setSelectedProvider] = useState('');
//   const [selectedCity, setSelectedCity] = useState('');
//   const [selectedTimePeriod, setSelectedTimePeriod] = useState('');
//   const [selectedDate, setSelectedDate] = useState('');

//   const [loading, setLoading] = useState(false);
//   const [searching, setSearching] = useState(false);
//   const [showFilters, setShowFilters] = useState(false);

//   useEffect(() => {
//     loadInitialData();
//   }, []);

//   const loadInitialData = async () => {
//     try {
//       setLoading(true);
//       const [servicesRes, citiesRes, timePeriodsRes] = await Promise.all([
//         getAvailableServices(),
//         getAvailableCities(),
//         getTimePeriods()
//       ]);
//       setServices(servicesRes.data || []);
//       setCities(citiesRes.data || []);
//       setTimePeriods(timePeriodsRes.data || []);
//     } catch (error) {
//       console.error('Error loading initial data:', error);
//       alert(`Error loading page data: ${error.message}. Please check console for details.`);
//     } finally {
//       setLoading(false);
//     }
//   };

//   const handleServiceChange = async (serviceId) => {
//     setSelectedService(serviceId);
//     setSelectedProvider('');
//     setSelectedCity(''); // איפוס גם עיר
//     setSelectedTimePeriod(''); // איפוס גם זמן
//     setSelectedDate(''); // איפוס גם תאריך
//     setProviders([]);
//     setAvailableSlots([]);
   
//     if (serviceId) {
//       try {
//         const providersRes = await getProvidersByService(serviceId);
//         setProviders(providersRes.data || []);
//         setShowFilters(true);
//       } catch (error) {
//         console.error('Error loading providers:', error);
//         alert('Error loading providers for this service.');
//       }
//     } else {
//       setShowFilters(false);
//     }
//   };

//   const handleSearch = async () => {
//     if (!selectedService) {
//       alert('Please select a service first.');
//       return;
//     }
   
//     const searchParams = {
//       ServiceId: parseInt(selectedService)
//     };
   
//     // הוספת פרמטרים רק אם הם נבחרו
//     if (selectedProvider) searchParams.ProviderKey = parseInt(selectedProvider);
//     if (selectedCity) searchParams.CityName = selectedCity;
//     if (selectedTimePeriod) searchParams.TimePeriod = selectedTimePeriod;
//     if (selectedDate) searchParams.PreferredDate = selectedDate;

//     console.log('🔍 Search parameters:', searchParams);

//     try {
//       setSearching(true);
//       const result = await searchAvailableSlots(searchParams);
     
//       console.log('📦 Available slots from server:', result.data);
//       console.log('🕵️ First slot details:', result.data?.[0]);
     
//       setAvailableSlots(result.data || []);
     
//       if (!result.data || result.data.length === 0) {
//         alert('No available appointments found for the selected criteria. Try adjusting your filters.');
//       }
//     } catch (error) {
//       console.error('Error searching slots:', error);
//       alert('Error searching for available appointments. Please try again.');
//     } finally {
//       setSearching(false);
//     }
//   };

//   const handleBook = async (slotId) => {
//     const patientKey = getPatientKeyFromToken();
//     if (!patientKey) {
//       alert('No patient key found. Please log in again.');
//       return;
//     }
//     if (!window.confirm('Are you sure you want to book this appointment?')) return;
//     try {
//       await bookAppointment(slotId, patientKey);
//       alert('Appointment successfully booked!');
//       handleSearch(); // רענון התוצאות
//     } catch (error) {
//       console.error('Error booking appointment:', error);
//       alert('Failed to book appointment. Please try again.');
//     }
//   };

//   const clearFilters = () => {
//     setSelectedProvider('');
//     setSelectedCity('');
//     setSelectedTimePeriod('');
//     setSelectedDate('');
//     setAvailableSlots([]);
//   };

//   const formatDate = (dateStr) => {
//     try {
//       const date = new Date(dateStr);
//       if (isNaN(date.getTime())) {
//         return 'Invalid Date';
//       }
//       return date.toLocaleDateString('en-US', {
//         weekday: 'long',
//         year: 'numeric',
//         month: 'long',
//         day: 'numeric'
//       });
//     } catch (error) {
//       console.error('Error formatting date:', error);
//       return 'Invalid Date';
//     }
//   };

//   const formatTime = (timeStr) => {
//     try {
//       return timeStr?.substring(0, 5) || 'N/A';
//     } catch (error) {
//       console.error('Error formatting time:', error);
//       return 'N/A';
//     }
//   };

//   if (loading) return <div style={{ padding: '20px' }}>Loading...</div>;

//   return (
//     <div style={{ padding: '20px', maxWidth: '1000px', margin: '0 auto' }}>
//       <h2>Book an Appointment</h2>
     
//       {/* בחירת שירות */}
//       <div style={{ marginBottom: '20px' }}>
//         <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//           Select Service Type: *
//         </label>
//         <select
//           style={{
//             width: '100%',
//             padding: '10px',
//             borderRadius: '4px',
//             border: '1px solid #ddd',
//             fontSize: '16px'
//           }}
//           value={selectedService}
//           onChange={(e) => handleServiceChange(e.target.value)}
//         >
//           <option value="">-- Select Service --</option>
//           {services.map((service, index) => (
//             <option key={service.serviceId || `service-${index}`} value={service.serviceId}>
//               {service.serviceName}
//             </option>
//           ))}
//         </select>
//       </div>

//       {/* סינונים אופציונליים */}
//       {showFilters && (
//         <div style={{
//           border: '1px solid #ddd',
//           padding: '20px',
//           marginBottom: '20px',
//           borderRadius: '8px',
//           backgroundColor: '#f9f9f9'
//         }}>
//           <h3 style={{ marginTop: '0', marginBottom: '15px' }}>Optional Filters</h3>
//           <div style={{
//             display: 'grid',
//             gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
//             gap: '15px'
//           }}>
//             <div>
//               <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//                 Specific Provider:
//               </label>
//               <select
//                 style={{
//                   width: '100%',
//                   padding: '8px',
//                   borderRadius: '4px',
//                   border: '1px solid #ddd'
//                 }}
//                 value={selectedProvider}
//                 onChange={(e) => setSelectedProvider(e.target.value)}
//               >
//                 <option value="">-- Any Provider --</option>
//                 {providers.map((provider, index) => (
//                   <option key={provider.providerKey || `provider-${index}`} value={provider.providerKey}>
//                     {provider.providerName}
//                   </option>
//                 ))}
//               </select>
//             </div>
           
//             <div>
//               <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//                 City:
//               </label>
//               <select
//                 style={{
//                   width: '100%',
//                   padding: '8px',
//                   borderRadius: '4px',
//                   border: '1px solid #ddd'
//                 }}
//                 value={selectedCity}
//                 onChange={(e) => setSelectedCity(e.target.value)}
//               >
//                 <option value="">-- Any City --</option>
//                 {cities.map((city, index) => (
//                   <option key={city.cityId || `city-${index}`} value={city.cityName}>
//                     {city.cityName}
//                   </option>
//                 ))}
//               </select>
//             </div>
           
//             <div>
//               <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//                 Preferred Time:
//               </label>
//               <select
//                 style={{
//                   width: '100%',
//                   padding: '8px',
//                   borderRadius: '4px',
//                   border: '1px solid #ddd'
//                 }}
//                 value={selectedTimePeriod}
//                 onChange={(e) => setSelectedTimePeriod(e.target.value)}
//               >
//                 <option value="">-- Any Time --</option>
//                 {timePeriods.map((period, index) => (
//                   <option key={period.value || `period-${index}`} value={period.value}>
//                     {period.label}
//                   </option>
//                 ))}
//               </select>
//             </div>
           
//             <div>
//               <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
//                 Preferred Date:
//               </label>
//               <input
//                 type="date"
//                 style={{
//                   width: '100%',
//                   padding: '8px',
//                   borderRadius: '4px',
//                   border: '1px solid #ddd'
//                 }}
//                 value={selectedDate}
//                 onChange={(e) => setSelectedDate(e.target.value)}
//                 min={new Date().toISOString().split('T')[0]}
//               />
//             </div>
//           </div>
         
//           <div style={{ marginTop: '20px', display: 'flex', gap: '10px' }}>
//             <button
//               onClick={handleSearch}
//               disabled={searching}
//               style={{
//                 backgroundColor: '#007bff',
//                 color: 'white',
//                 padding: '10px 20px',
//                 border: 'none',
//                 borderRadius: '4px',
//                 cursor: searching ? 'not-allowed' : 'pointer'
//               }}
//             >
//               {searching ? 'Searching...' : 'Search Available Appointments'}
//             </button>
//             <button
//               onClick={clearFilters}
//               style={{
//                 backgroundColor: '#6c757d',
//                 color: 'white',
//                 padding: '10px 20px',
//                 border: 'none',
//                 borderRadius: '4px',
//                 cursor: 'pointer'
//               }}
//             >
//               Clear Filters
//             </button>
//           </div>
//         </div>
//       )}

//       {/* תוצאות החיפוש */}
//       {availableSlots.length > 0 && (
//         <div>
//           <h3>Available Appointments ({availableSlots.length} found)</h3>
//           <div style={{
//             display: 'grid',
//             gridTemplateColumns: 'repeat(auto-fit, minmax(400px, 1fr))',
//             gap: '15px',
//             marginTop: '20px'
//           }}>
//             {availableSlots.map((slot, index) => {
//               // Debug log outside JSX
//               console.log('🔎 Slot debug:', slot);
//               return (
//                 <div
//                   key={slot.SlotId || `slot-${index}`}
//                   style={{
//                     border: '1px solid #ddd',
//                     borderRadius: '8px',
//                     padding: '15px',
//                     backgroundColor: '#fff',
//                     boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
//                   }}
//                 >
//                   <div style={{ marginBottom: '10px' }}>
//                     <strong style={{ fontSize: '18px', color: '#007bff' }}>
//                       {formatDate(slot.SlotDate || slot.slotDate)}
//                     </strong>
//                   </div>
//                   <div><strong>Time:</strong> {formatTime(slot.SlotStart || slot.slotStart)} - {formatTime(slot.SlotEnd || slot.slotEnd)}</div>
//                   <div><strong>Provider:</strong> {slot.ProviderName || slot.providerName || 'Unknown Provider'}</div>
//                   <div><strong>Location:</strong> {slot.BranchName || slot.branchName || 'Unknown Location'}</div>
//                   <div><strong>Address:</strong> {slot.Address || slot.address || 'Unknown Address'}</div>
//                   <div><strong>City:</strong> {slot.CityName || slot.cityName || 'Unknown City'}</div>
//                   <div style={{marginTop:'10px',fontSize:'12px',color:'#888'}}>
//                     <strong>Raw Slot Data:</strong> <pre style={{whiteSpace:'pre-wrap'}}>{JSON.stringify(slot,null,2)}</pre>
//                   </div>
//                   <button
//                     onClick={() => handleBook(slot.SlotId || slot.slotId)}
//                     style={{
//                       backgroundColor: '#28a745',
//                       color: 'white',
//                       padding: '8px 16px',
//                       border: 'none',
//                       borderRadius: '4px',
//                       cursor: 'pointer',
//                       fontSize: '14px',
//                       width: '100%',
//                       marginTop: '10px'
//                     }}
//                   >
//                     Book This Appointment
//                   </button>
//                 </div>
//               );
//             })}
//           </div>
//         </div>
//       )}

//       {/* הודעה כשאין תוצאות */}
//       {availableSlots.length === 0 && selectedService && !searching && (
//         <div style={{
//           textAlign: 'center',
//           padding: '40px',
//           color: '#666',
//           backgroundColor: '#f8f9fa',
//           borderRadius: '8px',
//           border: '1px solid #dee2e6'
//         }}>
//           <h3>No Appointments Found</h3>
//           <p>Try adjusting your filters or selecting a different service.</p>
//         </div>
//       )}

//       {/* הודעת הסבר */}
//       {!selectedService && (
//         <div style={{
//           backgroundColor: '#e7f3ff',
//           border: '1px solid #b3d9ff',
//           borderRadius: '8px',
//           padding: '20px',
//           marginTop: '20px'
//         }}>
//           <h3 style={{ color: '#0066cc', marginTop: '0' }}>How to Book an Appointment</h3>
//           <ol style={{ color: '#0066cc', lineHeight: '1.6' }}>
//             <li>Select the type of medical service you need</li>
//             <li>Optionally, use filters to narrow your search</li>
//             <li>Click "Search Available Appointments"</li>
//             <li>Review the available options and click "Book"</li>
//           </ol>
//         </div>
//       )}
//     </div>
//   );
// }
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  getAvailableServices,
  getProvidersByService,
  getAvailableCities,
  getTimePeriods,
  searchAvailableSlots,
  bookAppointment
} from '../services/appointmentService.js';
import { getPatientKeyFromToken } from '../utils/authUtils';
import { 
  ArrowLeft, 
  Calendar, 
  Search, 
  MapPin, 
  Clock, 
  User, 
  Building, 
  Heart,
  CheckCircle,
  RefreshCw,
  Filter,
  X
} from 'lucide-react';

export default function AppointmentBookingPage() {
  const navigate = useNavigate();
  
  const [services, setServices] = useState([]);
  const [providers, setProviders] = useState([]);
  const [cities, setCities] = useState([]);
  const [timePeriods, setTimePeriods] = useState([]);
  const [availableSlots, setAvailableSlots] = useState([]);

  const [selectedService, setSelectedService] = useState('');
  const [selectedProvider, setSelectedProvider] = useState('');
  const [selectedCity, setSelectedCity] = useState('');
  const [selectedTimePeriod, setSelectedTimePeriod] = useState('');
  const [selectedDate, setSelectedDate] = useState('');

  const [loading, setLoading] = useState(false);
  const [searching, setSearching] = useState(false);
  const [showFilters, setShowFilters] = useState(false);

  useEffect(() => {
    loadInitialData();
  }, []);

  const loadInitialData = async () => {
    try {
      setLoading(true);
      const [servicesRes, citiesRes, timePeriodsRes] = await Promise.all([
        getAvailableServices(),
        getAvailableCities(),
        getTimePeriods()
      ]);
      setServices(servicesRes.data || []);
      setCities(citiesRes.data || []);
      setTimePeriods(timePeriodsRes.data || []);
    } catch (error) {
      console.error('Error loading initial data:', error);
      alert(`Error loading page data: ${error.message}. Please check console for details.`);
    } finally {
      setLoading(false);
    }
  };

  const handleServiceChange = async (serviceId) => {
    setSelectedService(serviceId);
    setSelectedProvider('');
    setSelectedCity('');
    setSelectedTimePeriod('');
    setSelectedDate('');
    setProviders([]);
    setAvailableSlots([]);
   
    if (serviceId) {
      try {
        const providersRes = await getProvidersByService(serviceId);
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
   
    // הוספת פרמטרים רק אם הם נבחרו
    if (selectedProvider) searchParams.ProviderKey = parseInt(selectedProvider);
    if (selectedCity) searchParams.CityName = selectedCity;
    if (selectedTimePeriod) searchParams.TimePeriod = selectedTimePeriod;
    if (selectedDate) searchParams.PreferredDate = selectedDate;

    console.log('🔍 Search parameters:', searchParams);

    try {
      setSearching(true);
      const result = await searchAvailableSlots(searchParams);
     
      console.log('📦 Available slots from server:', result.data);
      console.log('🕵️ First slot details:', result.data?.[0]);
     
      setAvailableSlots(result.data || []);
     
      if (!result.data || result.data.length === 0) {
        alert('No available appointments found for the selected criteria. Try adjusting your filters.');
      }
    } catch (error) {
      console.error('Error searching slots:', error);
      alert('Error searching for available appointments. Please try again.');
    } finally {
      setSearching(false);
    }
  };

  const handleBook = async (slotId) => {
    const patientKey = getPatientKeyFromToken();
    if (!patientKey) {
      alert('No patient key found. Please log in again.');
      return;
    }
    if (!window.confirm('Are you sure you want to book this appointment?')) return;
    try {
      await bookAppointment(slotId, patientKey);
      alert('Appointment successfully booked!');
      handleSearch(); // רענון התוצאות
    } catch (error) {
      console.error('Error booking appointment:', error);
      alert('Failed to book appointment. Please try again.');
    }
  };

  const clearFilters = () => {
    setSelectedProvider('');
    setSelectedCity('');
    setSelectedTimePeriod('');
    setSelectedDate('');
    setAvailableSlots([]);
  };

  const formatDate = (dateStr) => {
    try {
      const date = new Date(dateStr);
      if (isNaN(date.getTime())) {
        return 'Invalid Date';
      }
      return date.toLocaleDateString('en-US', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });
    } catch (error) {
      console.error('Error formatting date:', error);
      return 'Invalid Date';
    }
  };

  const formatTime = (timeStr) => {
    try {
      return timeStr?.substring(0, 5) || 'N/A';
    } catch (error) {
      console.error('Error formatting time:', error);
      return 'N/A';
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Loading data...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-green-50">
      {/* Header */}
      <header className="bg-white/80 backdrop-blur-md shadow-sm sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center space-x-3">
              <button
                onClick={() => navigate('/home')}
                className="flex items-center space-x-2 text-blue-600 hover:text-blue-700 transition-colors"
              >
                <ArrowLeft className="h-5 w-5" />
                <span>Back to Home</span>
              </button>
            </div>
            
            <div className="flex items-center space-x-3">
              <div className="bg-gradient-to-r from-blue-600 to-green-600 p-2 rounded-xl">
                <Heart className="h-6 w-6 text-white" />
              </div>
              <h1 className="text-xl font-bold bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
                MediCare Pro
              </h1>
            </div>
            
            <button
              onClick={handleLogout}
              className="bg-gradient-to-r from-red-500 to-red-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:from-red-600 hover:to-red-700 transition-all duration-200 shadow-md hover:shadow-lg"
            >
              Logout
            </button>
          </div>
        </div>
      </header>

      <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            Book Medical Appointment
          </h1>
          <p className="text-xl text-gray-600">
            Choose the medical service you need and book an appointment easily
          </p>
        </div>

        {/* Service Selection */}
        <div className="bg-white rounded-3xl shadow-lg p-8 mb-8 border border-gray-100">
          <div className="flex items-center space-x-3 mb-6">
            <div className="bg-gradient-to-r from-blue-500 to-blue-600 p-3 rounded-xl">
              <Calendar className="h-6 w-6 text-white" />
            </div>
            <h2 className="text-2xl font-bold text-gray-900">Select Medical Service *</h2>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {services.map((service, index) => (
              <div
                key={service.serviceId || index}
                onClick={() => handleServiceChange(service.serviceId)}
                className={`group relative p-6 rounded-2xl border-2 cursor-pointer transition-all duration-300 transform hover:-translate-y-1 ${
                  selectedService === service.serviceId
                    ? 'border-blue-500 bg-blue-50 shadow-lg scale-105'
                    : 'border-gray-200 hover:border-blue-300 hover:shadow-md'
                }`}
              >
                <div className="text-center">
                  <div className="text-4xl mb-4">🩺</div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-2 group-hover:text-blue-600">
                    {service.serviceName}
                  </h3>
                  <p className="text-sm text-gray-600">Click to select this service</p>
                </div>
                
                {selectedService === service.serviceId && (
                  <div className="absolute top-2 right-2">
                    <CheckCircle className="h-6 w-6 text-blue-500" />
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Filters */}
        {showFilters && (
          <div className="bg-white rounded-3xl shadow-lg p-8 mb-8 border border-gray-100">
            <div className="flex items-center justify-between mb-6">
              <div className="flex items-center space-x-3">
                <div className="bg-gradient-to-r from-green-500 to-green-600 p-3 rounded-xl">
                  <Filter className="h-6 w-6 text-white" />
                </div>
                <h2 className="text-2xl font-bold text-gray-900">Optional Filters</h2>
              </div>
              
              <button
                onClick={clearFilters}
                className="flex items-center space-x-2 text-gray-600 hover:text-red-600 transition-colors"
              >
                <X className="h-4 w-4" />
                <span>Clear All</span>
              </button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2 flex items-center">
                  <User className="h-4 w-4 mr-1" />
                  Specific Provider
                </label>
                <select 
                  className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent transition-all duration-200"
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
                <label className="block text-sm font-medium text-gray-700 mb-2 flex items-center">
                  <MapPin className="h-4 w-4 mr-1" />
                  City
                </label>
                <select 
                  className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent transition-all duration-200"
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
                <label className="block text-sm font-medium text-gray-700 mb-2 flex items-center">
                  <Clock className="h-4 w-4 mr-1" />
                  Preferred Time
                </label>
                <select 
                  className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent transition-all duration-200"
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
                <label className="block text-sm font-medium text-gray-700 mb-2 flex items-center">
                  <Calendar className="h-4 w-4 mr-1" />
                  Preferred Date
                </label>
                <input 
                  type="date"
                  className="w-full p-3 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-transparent transition-all duration-200"
                  value={selectedDate}
                  onChange={(e) => setSelectedDate(e.target.value)}
                  min={new Date().toISOString().split('T')[0]}
                />
              </div>
            </div>
            
            <div className="flex justify-center">
              <button
                onClick={handleSearch}
                disabled={searching}
                className="flex items-center space-x-2 bg-gradient-to-r from-green-600 to-green-700 text-white px-8 py-3 rounded-xl font-medium hover:from-green-700 hover:to-green-800 transition-all duration-200 shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {searching ? (
                  <RefreshCw className="h-5 w-5 animate-spin" />
                ) : (
                  <Search className="h-5 w-5" />
                )}
                <span>{searching ? 'Searching...' : 'Search Available Appointments'}</span>
              </button>
            </div>
          </div>
        )}

        {/* Available Slots */}
        {availableSlots.length > 0 && (
          <div className="bg-white rounded-3xl shadow-lg p-8 mb-8 border border-gray-100">
            <h2 className="text-2xl font-bold text-gray-900 mb-6 flex items-center">
              <CheckCircle className="h-7 w-7 text-green-500 mr-3" />
              Available Appointments ({availableSlots.length} found)
            </h2>
            
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {availableSlots.map((slot, index) => (
                <div
                  key={slot.SlotId || slot.slotId || index}
                  className="group p-6 rounded-2xl border-2 border-green-200 bg-green-50 hover:border-green-400 hover:bg-green-100 transition-all duration-300 transform hover:-translate-y-1 hover:shadow-lg"
                >
                  <div className="mb-4">
                    <div className="text-lg font-bold text-blue-600 mb-2 flex items-center">
                      <Calendar className="h-5 w-5 mr-2" />
                      {formatDate(slot.SlotDate || slot.slotDate)}
                    </div>
                    <div className="text-md font-semibold text-gray-800 flex items-center">
                      <Clock className="h-4 w-4 mr-2" />
                      {formatTime(slot.SlotStart || slot.slotStart)} - {formatTime(slot.SlotEnd || slot.slotEnd)}
                    </div>
                  </div>
                  
                  <div className="space-y-2 mb-4">
                    <div className="flex items-center text-gray-700">
                      <User className="h-4 w-4 mr-2 text-blue-500" />
                      <span className="font-semibold">
                        {slot.ProviderName || slot.providerName || 'Unknown Provider'}
                      </span>
                    </div>
                    <div className="flex items-center text-gray-600">
                      <Building className="h-4 w-4 mr-2 text-green-500" />
                      <span>{slot.BranchName || slot.branchName || 'Unknown Location'}</span>
                    </div>
                    <div className="flex items-center text-gray-600">
                      <MapPin className="h-4 w-4 mr-2 text-red-500" />
                      <span>{slot.CityName || slot.cityName || 'Unknown City'}</span>
                    </div>
                    {(slot.Address || slot.address) && (
                      <div className="text-sm text-gray-500 mt-2">
                        📍 {slot.Address || slot.address}
                      </div>
                    )}
                  </div>
                  
                  <button
                    onClick={() => handleBook(slot.SlotId || slot.slotId)}
                    className="w-full bg-gradient-to-r from-green-500 to-green-600 text-white py-3 px-4 rounded-xl font-medium hover:from-green-600 hover:to-green-700 transition-all duration-200 shadow-md hover:shadow-lg group-hover:scale-105"
                  >
                    Book This Appointment
                  </button>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* No Results */}
        {availableSlots.length === 0 && selectedService && !searching && (
          <div className="bg-white rounded-3xl shadow-lg p-12 text-center border border-gray-100">
            <div className="text-6xl mb-6">🔍</div>
            <h3 className="text-2xl font-bold text-gray-900 mb-4">No Appointments Found</h3>
            <p className="text-gray-600 mb-6">
              We couldn't find any available appointments matching your criteria.
            </p>
            <div className="text-sm text-gray-500">
              Try adjusting your filters or selecting a different service
            </div>
          </div>
        )}

        {/* Instructions */}
        {!selectedService && (
          <div className="bg-gradient-to-r from-blue-50 to-green-50 rounded-3xl p-8 border border-blue-200">
            <h3 className="text-2xl font-bold text-blue-900 mb-6 text-center">
              How to Book an Appointment
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              <div className="text-center">
                <div className="bg-blue-600 text-white rounded-full w-8 h-8 flex items-center justify-center text-lg font-bold mx-auto mb-3">1</div>
                <h4 className="font-semibold text-blue-800 mb-2">Select Service</h4>
                <p className="text-sm text-blue-700">Choose the type of medical service you need</p>
              </div>
              <div className="text-center">
                <div className="bg-green-600 text-white rounded-full w-8 h-8 flex items-center justify-center text-lg font-bold mx-auto mb-3">2</div>
                <h4 className="font-semibold text-green-800 mb-2">Set Filters</h4>
                <p className="text-sm text-green-700">Use optional filters to narrow your search</p>
              </div>
              <div className="text-center">
                <div className="bg-purple-600 text-white rounded-full w-8 h-8 flex items-center justify-center text-lg font-bold mx-auto mb-3">3</div>
                <h4 className="font-semibold text-purple-800 mb-2">Search</h4>
                <p className="text-sm text-purple-700">Click search to find available appointments</p>
              </div>
              <div className="text-center">
                <div className="bg-orange-600 text-white rounded-full w-8 h-8 flex items-center justify-center text-lg font-bold mx-auto mb-3">4</div>
                <h4 className="font-semibold text-orange-800 mb-2">Book</h4>
                <p className="text-sm text-orange-700">Choose your preferred slot and book it</p>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}