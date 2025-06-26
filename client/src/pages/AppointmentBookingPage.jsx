import { useEffect, useState } from 'react';
import { getAvailableSlotsByService, bookAppointment } from '../services/appointmentService.js'; // ✅
import { getAllServices } from '../services/servicesService';
import { getPatientIdFromToken } from '../utils/authUtils';

export default function AppointmentBookingPage() {
    const [services, setServices] = useState([]);
    const [slots, setSlots] = useState([]);

    useEffect(() => {
        getAllServices().then(setServices);
    }, []);

    const handleServiceChange = async (e) => {
        const serviceId = e.target.value;
        const available = await getAvailableSlotsByService(serviceId);
        setSlots(available);
    };

    const handleBook = async (slotId) => {
        const patientId = getPatientIdFromToken();
        if (!patientId) {
            alert('No patient ID found. Please log in again.');
            return;
        }
        await bookAppointment(slotId, patientId);
        alert('Appointment successfully booked!');
    };

    return (
        <div>
            <h2>Book an Appointment</h2>
            <select onChange={handleServiceChange}>
                <option>Select Service</option>
                {Array.isArray(services) &&
                    services.map((s) => (
                        <option key={s.id} value={s.id}>{s.name}</option>
                    ))}

            </select>

            <ul>
                {slots.map((slot) => (
                    <li key={slot.id}>
                        {slot.date} - {slot.time}
                        <button onClick={() => handleBook(slot.id)}>Book</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}
