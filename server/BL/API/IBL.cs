﻿using DAL.API;

namespace BLL.API
{
    public interface IBL
    {
        IAuthService AuthService { get; set; }
        IPatientService PatientService { get; set; }
        IAppointmentService AppointmentService { get; set; }
        IClinicServiceService ClinicServiceService { get; set; }
        IPasswordService PasswordService { get; set; }
        ICityStreetService CityStreetService { get; set; } // הוספנו

        // DAL Services
        IPatientsManagement PatientsManagement { get; set; }
        IClinicServiceManagement ClinicServiceManagement { get; set; }
        IServiceProviderManagement ServiceProviderManagement { get; set; }
        IAppointmentManagement AppointmentManagement { get; set; }
        IAppointmentsSlotManagement AppointmentsSlotManagement { get; set; }
        IBranchManagement BranchManagement { get; set; }
        IAddressManagement AddressManagement { get; set; }
    }
}