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
    if (selectedProvider) searchParams.ProviderKey = parseInt(selectedProvider);
    if (selectedCity) searchParams.CityName = selectedCity;
    if (selectedTimePeriod) searchParams.TimePeriod = selectedTimePeriod;
    if (selectedDate) searchParams.PreferredDate = selectedDate;

    try {
      setSearching(true);
      const result = await searchAvailableSlots(searchParams);
    console.log('📦 hello');
        // ⬅️ כאן להוסיף:
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
    return date.toLocaleDateString('en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });
  };

  const formatTime = (timeStr) => timeStr?.substring(0, 5) || '';

  if (loading) return <div style={{ padding: '20px' }}>Loading...</div>;

  return (
    <div style={{ padding: '20px', maxWidth: '1000px', margin: '0 auto' }}>
      <h2>Book an Appointment</h2>
      {/* בחירת שירות */}
      <div style={{ marginBottom: '20px' }}>
        <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
          Select Service Type: *
        </label>
        <select
          style={{ width: '100%', padding: '10px', borderRadius: '4px', border: '1px solid #ddd', fontSize: '16px' }}
          value={selectedService}
          onChange={(e) => handleServiceChange(e.target.value)}
        >
          <option value="">-- Select Service --</option>
          {services.map((service, index) => (
            <option key={service.serviceId || `service-${index}`} value={service.serviceId}>
              {service.serviceName}
            </option>
          ))}
        </select>
      </div>

      {/* סינונים אופציונליים */}
      {showFilters && (
        <div style={{ border: '1px solid #ddd', padding: '20px', marginBottom: '20px', borderRadius: '8px', backgroundColor: '#f9f9f9' }}>
          <h3 style={{ marginTop: '0', marginBottom: '15px' }}>Optional Filters</h3>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '15px' }}>
            <div>
              <label>Specific Provider:</label>
              <select value={selectedProvider} onChange={(e) => setSelectedProvider(e.target.value)}>
                <option value="">-- Any Provider --</option>
                {providers.map((provider, index) => (
                  <option key={provider.providerKey || `provider-${index}`} value={provider.providerKey}>
                    {provider.providerName}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label>City:</label>
              <select value={selectedCity} onChange={(e) => setSelectedCity(e.target.value)}>
                <option value="">-- Any City --</option>
                {cities.map((city, index) => (
                  <option key={city.cityId || `city-${index}`} value={city.cityName}>
                    {city.cityName}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label>Preferred Time:</label>
              <select value={selectedTimePeriod} onChange={(e) => setSelectedTimePeriod(e.target.value)}>
                <option value="">-- Any Time --</option>
                {timePeriods.map((period, index) => (
                  <option key={period.value || `period-${index}`} value={period.value}>
                    {period.label}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label>Preferred Date:</label>
              <input type="date" value={selectedDate} onChange={(e) => setSelectedDate(e.target.value)} min={new Date().toISOString().split('T')[0]} />
            </div>
          </div>
          <div style={{ marginTop: '20px', display: 'flex', gap: '10px' }}>
            <button onClick={handleSearch} disabled={searching} style={{ backgroundColor: '#007bff', color: 'white' }}>
              {searching ? 'Searching...' : 'Search Available Appointments'}
            </button>
            <button onClick={clearFilters} style={{ backgroundColor: '#6c757d', color: 'white' }}>
              Clear Filters
            </button>
          </div>
        </div>
      )}

      {availableSlots.length > 0 && (
        <div>
          <h3>Available Appointments ({availableSlots.length} found)</h3>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(400px, 1fr))', gap: '15px', marginTop: '20px' }}>
            {availableSlots.map((slot, index) => (
              <div key={slot.SlotId || `slot-${index}`} style={{ border: '1px solid #ddd', borderRadius: '8px', padding: '15px', backgroundColor: '#fff', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
                <div style={{ marginBottom: '10px' }}>
                  <strong style={{ fontSize: '18px', color: '#007bff' }}>
                    {slot.SlotDate && !isNaN(new Date(slot.SlotDate)) ? formatDate(slot.SlotDate) : 'No Date'}
                  </strong>
                </div>
                <div><strong>Time:</strong> {slot.SlotStart && slot.SlotEnd ? `${formatTime(slot.SlotStart)} - ${formatTime(slot.SlotEnd)}` : '-'}</div>
                <div><strong>Provider:</strong> {slot.ProviderName || '-'}</div>
                <div><strong>Location:</strong> {slot.BranchName || '-'}</div>
                <div><strong>Address:</strong> {slot.Address || '-'}</div>
                <button onClick={() => handleBook(slot.SlotId)} style={{ backgroundColor: '#28a745', color: 'white', padding: '8px 16px', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '14px', width: '100%' }}>
                  Book This Appointment
                </button>
              </div>
            ))}
          </div>
        </div>
      )}

      {availableSlots.length === 0 && selectedService && !searching && (
        <div style={{ textAlign: 'center', padding: '40px', color: '#666', backgroundColor: '#f8f9fa', borderRadius: '8px', border: '1px solid #dee2e6' }}>
          <h3>No Appointments Found</h3>
          <p>Try adjusting your filters or selecting a different service.</p>
        </div>
      )}

      {!selectedService && (
        <div style={{ backgroundColor: '#e7f3ff', border: '1px solid #b3d9ff', borderRadius: '8px', padding: '20px', marginTop: '20px' }}>
          <h3 style={{ color: '#0066cc', marginTop: '0' }}>How to Book an Appointment</h3>
          <ol style={{ color: '#0066cc', lineHeight: '1.6' }}>
            <li>Select the type of medical service you need</li>
            <li>Optionally, use filters to narrow your search</li>
            <li>Click "Search Available Appointments"</li>
            <li>Review the available options and click "Book"</li>
          </ol>
        </div>
      )}
    </div>
  );
}
