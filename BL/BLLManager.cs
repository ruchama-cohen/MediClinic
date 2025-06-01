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

        public BlManager()
        {
            DB_Manager db = new DB_Manager();

            // יצירת כל ה-DAL services
            IAddressManagement addressManagementDal = new AddressManagement(db);
            IAppointmentManagement appointmentManagementDal = new AppointmentManagement(db);
            IPatientsManagement patientsManagementDal = new PatientsManagement(db);
            IAppointmentsSlotManagement appointmentsSlotManagementDal = new AppointmentsSlotManagement(db);
            IBranchManagement branchManagementDal = new BranchManagement(db);
            IClinicServiceManagement clinicServiceManagementDal = new ClinicServiceManagement(db);
            IServiceProviderManagement serviceProviderManagementDal = new ServiceProviderManagement(db);
            IWorkHourManagement workHourManagementDal = new WorkHourManagement(db);

            // יצירת Password Service
            IPasswordService passwordService = new PasswordService();

            // יצירת Auth Service
            AuthService = new AuthService(patientsManagementDal, passwordService);

            // יצירת Patient Service עם 3 פרמטרים בלבד
            PatientService = new PatientService(
                patientsManagementDal,
                addressManagementDal,
                passwordService
            );

            // יצירת Appointment Service
            AppointmentService = new AppointmentService(
                appointmentManagementDal,
                appointmentsSlotManagementDal,
                serviceProviderManagementDal,
                patientsManagementDal
            );
        }
    }
}