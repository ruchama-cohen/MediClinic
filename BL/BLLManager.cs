
using BLL.API;
using BLL.Services;

using DAL.API;
using DAL.Models;
using DAL.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
     

namespace BL
    {
        // מיצג את כל שכבת הביאל. 
        public class BlManager : IBL
        {
            public IAuthService AuthService { get; set; }
            public IPatientService PatientService { get; set; }
            

            public BlManager()
        {
            DB_Manager db = new DB_Manager();
            IAddressManagement addressManagementDal = new AddressManagement(db);
            IAppointmentManagement appointmentManagementDal = new AppointmentManagement(db);
            IPatientsManagement patientsManagementDal = new PatientsManagement(db);
            IAppointmentsSlotManagement appointmentsSlotManagementDal = new AppointmentsSlotManagement(db);
            IBranchManagement branchManagementDal = new BranchManagement(db);
            IBranchToServiceProviderManagement branchToServiceProviderManagementDal = new BranchToServiceProviderManagement(db);
            IClinicServiceManagement clinicServiceManagementDal = new ClinicServiceManagement(db);
            IServiceProviderManagement serviceProviderManagementDal = new ServiceProviderManagement(db);
            IWorkHourManagement workHourManagementDal = new WorkHourManagement(db);

            // כאן צריך גם להזריק
            Trainers = new TrainerBL(trainerDal);

               
            }
        }
    }

}
}
