import { useEffect, useState } from 'react';
import { getAvailableSlotsByService, bookAppointment } from '../services/appointmentService.js';
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
        console.log("Available slots response:", available);
        setSlots(Array.isArray(available.data) ? available.data : []);
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
                <option value="">Select Service</option>
                {Array.isArray(services) &&
                    services.map((s) => (
                        <option key={s.serviceId} value={s.serviceId}>
                            {s.serviceName}
                        </option>
                    ))}
            </select>

            {Array.isArray(slots) && slots.length === 0 && <p>No available slots.</p>}

            <ul>
                {Array.isArray(slots) &&
                    slots.map((slot) => (
                        <li key={slot.slotId}>
                            {slot.slotDate} - {slot.slotStart}:00
                            <button onClick={() => handleBook(slot.slotId)}>Book</button>
                        </li>


                    ))}
            </ul>
        </div>
    );
}
