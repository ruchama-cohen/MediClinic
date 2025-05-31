using BLL.API;
using BLL.Services;
using DAL.API;
using DAL.Models;
using DAL.Services;
using WebAPI.Services;

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
            PatientService = new PatientService(patientsManagementDal);
            AppointmentService = new AppointmentService(
                appointmentManagementDal,
                appointmentsSlotManagementDal,
                serviceProviderManagementDal,
                patientsManagementDal
            );
        }
    }
}
