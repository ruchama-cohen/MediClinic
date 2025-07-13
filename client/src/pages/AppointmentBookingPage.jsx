import { useEffect, useState } from 'react';
import { 
  getAvailableServices, 
  getProvidersByService, 
  getAvailableCities, 
  getTimePeriods,
  searchAvailableSlots,
  bookAppointment 
} from '../services/appointmentService.js';
import { getPatientKeyFromToken } from '../utils/authUtils';

export default function AppointmentBookingPage() {
  const [services, setServices] = useState([]);
  const [providers, setProviders] = useState([]);
  const [cities, setCities] = useState([]);
  const [timePeriods, setTimePeriods] = useState([]);
  const [availableSlots, setAvailableSlots] = useState([]);
  
  // סינונים
  const [selectedService, setSelectedService] = useState('');
  const [selectedProvider, setSelectedProvider] = useState('');
  const [selectedCity, setSelectedCity] = useState('');
  const [selectedTimePeriod, setSelectedTimePeriod] = useState('');
  const [selectedDate, setSelectedDate] = useState('');
  
  // מצבים
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
      alert('Error loading page data. Please refresh.');
    } finally {
      setLoading(false);
    }
  };

  const handleServiceChange = async (serviceId) => {
    setSelectedService(serviceId);
    setSelectedProvider(''); // איפוס בחירת רופא
    setProviders([]);
    setAvailableSlots([]); // איפוס תוצאות חיפוש
    
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

    // הוספת סינונים אופציונליים
    if (selectedProvider) {
      searchParams.ProviderKey = parseInt(selectedProvider);
    }
    if (selectedCity) {
      searchParams.CityName = selectedCity;
    }
    if (selectedTimePeriod) {
      searchParams.TimePeriod = selectedTimePeriod;
    }
    if (selectedDate) {
      searchParams.PreferredDate = selectedDate;
    }

    try {
      setSearching(true);
      const result = await searchAvailableSlots(searchParams);
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

    if (!window.confirm('Are you sure you want to book this appointment?')) {
      return;
    }

    try {
      await bookAppointment(slotId, patientKey);
      alert('Appointment successfully booked!');
      // רענון תוצאות החיפוש
      handleSearch();
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
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { 
      weekday: 'long', 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  };

  const formatTime = (timeStr) => {
    return timeStr.substring(0, 5); // HH:mm
  };

  if (loading) {
    return <div style={{ padding: '20px' }}>Loading...</div>;
  }

  return (
    <div style={{ padding: '20px', maxWidth: '1000px', margin: '0 auto' }}>
      <h2>Book an Appointment</h2>

      {/* בחירת שירות - חובה */}
      <div style={{ marginBottom: '20px' }}>
        <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
          Select Service Type: *
        </label>
        <select 
          style={{ 
            width: '100%', 
            padding: '10px', 
            borderRadius: '4px', 
            border: '1px solid #ddd',
            fontSize: '16px'
          }}
          value={selectedService}
          onChange={(e) => handleServiceChange(e.target.value)}
        >
          <option value="">-- Select Service --</option>
          {services.map((service) => (
            <option key={service.ServiceId} value={service.ServiceId}>
              {service.ServiceName}
            </option>
          ))}
        </select>
      </div>

      {/* סינונים אופציונליים */}
      {showFilters && (
        <div style={{ 
          border: '1px solid #ddd', 
          padding: '20px',
          marginBottom: '20px',
          borderRadius: '8px',
          backgroundColor: '#f9f9f9'
        }}>
          <h3 style={{ marginTop: '0', marginBottom: '15px' }}>Optional Filters</h3>
          <p style={{ color: '#666', marginBottom: '15px', fontSize: '14px' }}>
            Use these filters to narrow down your search. All are optional.
          </p>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '15px' }}>
            {/* בחירת רופא */}
            <div>
              <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
                Specific Provider:
              </label>
              <select 
                style={{ 
                  width: '100%', 
                  padding: '8px', 
                  borderRadius: '4px', 
                  border: '1px solid #ddd' 
                }}
                value={selectedProvider}
                onChange={(e) => setSelectedProvider(e.target.value)}
              >
                <option value="">-- Any Provider --</option>
                {providers.map((provider) => (
                  <option key={provider.ProviderKey} value={provider.ProviderKey}>
                    {provider.ProviderName}
                  </option>
                ))}
              </select>
            </div>

            {/* בחירת עיר */}
            <div>
              <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
                City:
              </label>
              <select 
                style={{ 
                  width: '100%', 
                  padding: '8px', 
                  borderRadius: '4px', 
                  border: '1px solid #ddd' 
                }}
                value={selectedCity}
                onChange={(e) => setSelectedCity(e.target.value)}
              >
                <option value="">-- Any City --</option>
                {cities.map((city) => (
                  <option key={city.CityId} value={city.CityName}>
                    {city.CityName}
                  </option>
                ))}
              </select>
            </div>

            {/* בחירת זמן */}
            <div>
              <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
                Preferred Time:
              </label>
              <select 
                style={{ 
                  width: '100%', 
                  padding: '8px', 
                  borderRadius: '4px', 
                  border: '1px solid #ddd' 
                }}
                value={selectedTimePeriod}
                onChange={(e) => setSelectedTimePeriod(e.target.value)}
              >
                <option value="">-- Any Time --</option>
                {timePeriods.map((period) => (
                  <option key={period.Value} value={period.Value}>
                    {period.Label}
                  </option>
                ))}
              </select>
            </div>

            {/* בחירת תאריך */}
            <div>
              <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
                Preferred Date:
              </label>
              <input
                type="date"
                style={{ 
                  width: '100%', 
                  padding: '8px', 
                  borderRadius: '4px', 
                  border: '1px solid #ddd' 
                }}
                value={selectedDate}
                onChange={(e) => setSelectedDate(e.target.value)}
                min={new Date().toISOString().split('T')[0]} // מחר ומעלה
              />
            </div>
          </div>

          {/* כפתורי פעולה */}
          <div style={{ marginTop: '20px', display: 'flex', gap: '10px' }}>
            <button 
              onClick={handleSearch}
              disabled={searching}
              style={{
                backgroundColor: searching ? '#ccc' : '#007bff',
                color: 'white',
                padding: '10px 20px',
                border: 'none',
                borderRadius: '4px',
                cursor: searching ? 'not-allowed' : 'pointer',
                fontSize: '16px'
              }}
            >
              {searching ? 'Searching...' : 'Search Available Appointments'}
            </button>

            <button 
              onClick={clearFilters}
              style={{
                backgroundColor: '#6c757d',
                color: 'white',
                padding: '10px 20px',
                border: 'none',
                borderRadius: '4px',
                cursor: 'pointer',
                fontSize: '16px'
              }}
            >
              Clear Filters
            </button>
          </div>
        </div>
      )}

      {/* תוצאות החיפוש */}
      {availableSlots.length > 0 && (
        <div>
          <h3>Available Appointments ({availableSlots.length} found)</h3>
          <div style={{ 
            display: 'grid', 
            gridTemplateColumns: 'repeat(auto-fit, minmax(400px, 1fr))', 
            gap: '15px',
            marginTop: '20px'
          }}>
            {availableSlots.map((slot) => (
              <div 
                key={slot.SlotId} 
                style={{ 
                  border: '1px solid #ddd',
                  borderRadius: '8px',
                  padding: '15px',
                  backgroundColor: '#fff',
                  boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
                }}
              >
                <div style={{ marginBottom: '10px' }}>
                  <strong style={{ fontSize: '18px', color: '#007bff' }}>
                    {formatDate(slot.SlotDate)}
                  </strong>
                </div>
                
                <div style={{ marginBottom: '8px' }}>
                  <strong>Time:</strong> {formatTime(slot.SlotStart)} - {formatTime(slot.SlotEnd)}
                </div>
                
                <div style={{ marginBottom: '8px' }}>
                  <strong>Provider:</strong> {slot.ProviderName}
                </div>
                
                <div style={{ marginBottom: '8px' }}>
                  <strong>Location:</strong> {slot.BranchName}
                </div>
                
                <div style={{ marginBottom: '15px', fontSize: '14px', color: '#666' }}>
                  <strong>Address:</strong> {slot.Address}
                </div>

                <button 
                  onClick={() => handleBook(slot.SlotId)}
                  style={{
                    backgroundColor: '#28a745',
                    color: 'white',
                    padding: '8px 16px',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: 'pointer',
                    fontSize: '14px',
                    width: '100%'
                  }}
                >
                  Book This Appointment
                </button>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* הודעה אם אין תוצאות */}
      {availableSlots.length === 0 && selectedService && !searching && (
        <div style={{ 
          textAlign: 'center', 
          padding: '40px', 
          color: '#666',
          backgroundColor: '#f8f9fa',
          borderRadius: '8px',
          border: '1px solid #dee2e6'
        }}>
          <h3>No Appointments Found</h3>
          <p>Try adjusting your filters or selecting a different service.</p>
        </div>
      )}

      {/* הוראות למשתמש */}
      {!selectedService && (
        <div style={{ 
          backgroundColor: '#e7f3ff', 
          border: '1px solid #b3d9ff',
          borderRadius: '8px',
          padding: '20px',
          marginTop: '20px'
        }}>
          <h3 style={{ color: '#0066cc', marginTop: '0' }}>How to Book an Appointment</h3>
          <ol style={{ color: '#0066cc', lineHeight: '1.6' }}>
            <li>Select the type of medical service you need</li>
            <li>Optionally, use filters to narrow your search:
              <ul>
                <li>Choose a specific provider if you have a preference</li>
                <li>Select a city location</li>
                <li>Pick your preferred time of day</li>
                <li>Choose a specific date</li>
              </ul>
            </li>
            <li>Click "Search Available Appointments"</li>
            <li>Review the available options and click "Book" on your preferred slot</li>
          </ol>
        </div>
      )}
    </div>
  );
}