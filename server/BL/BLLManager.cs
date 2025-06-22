using BLL.API;
using BLL.Services;
using DAL.API;
using DAL.Models;
using DAL.Services;

namespace BL
{
    public class BlManager : IBL
    {
        public IAuthService AuthService { get; set; }
        public IPatientService PatientService { get; set; }
        public IAppointmentService AppointmentService { get; set; }
        public IClinicServiceService ClinicServiceService { get; set; }
        public IPasswordService PasswordService { get; set; }
        public IPatientService patientService { get; set; } // זה נראה כמו כפילות - כדאי לבדוק אם צריך

        // הוסף את השירותים החסרים שהקונטרולרים צריכים
        public IPatientsManagement PatientsManagement { get; set; }
        public IClinicServiceManagement ClinicServiceManagement { get; set; }
        public IServiceProviderManagement ServiceProviderManagement { get; set; }
        public IAppointmentManagement AppointmentManagement { get; set; }
        public IAppointmentsSlotManagement AppointmentsSlotManagement { get; set; }

        public BlManager()
        {
            DB_Manager db = new DB_Manager();
            IAddressManagement addressManagementDal = new AddressManagement(db);
            IAppointmentManagement appointmentManagementDal = new AppointmentManagement(db);
            IPatientsManagement patientsManagementDal = new PatientsManagement(db);
            IAppointmentsSlotManagement appointmentsSlotManagementDal = new AppointmentsSlotManagement(db);
            IBranchManagement branchManagementDal = new BranchManagement(db);
            IClinicServiceManagement clinicServiceManagementDal = new ClinicServiceManagement(db);
            IServiceProviderManagement serviceProviderManagementDal = new ServiceProviderManagement(db);
            IWorkHourManagement workHourManagementDal = new WorkHourManagement(db);
            IPasswordService passwordService = new PasswordService();
            AuthService = new AuthService(patientsManagementDal, passwordService);
            PatientService = new PatientService(
                patientsManagementDal,
                addressManagementDal,
                passwordService
            );
            AppointmentService = new AppointmentService(
                appointmentManagementDal,
                appointmentsSlotManagementDal,
                serviceProviderManagementDal,
                patientsManagementDal
            );
            ClinicServiceService = new ClinicServiceService(
                clinicServiceManagementDal
            );
            PasswordService = passwordService;
            PatientsManagement = patientsManagementDal;
            ClinicServiceManagement = clinicServiceManagementDal;
            ServiceProviderManagement = serviceProviderManagementDal;
            AppointmentManagement = appointmentManagementDal;
            AppointmentsSlotManagement = appointmentsSlotManagementDal;
        }
    }
}