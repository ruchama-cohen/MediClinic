
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
            IAddressManagement addressManagement = new AddressManagement(db);

            ITrainerDal trainerDal = new DAL.Services.TrainerDal(db);
                // כאן צריך גם להזריק
                Trainers = new TrainerBL(trainerDal);

                IGymnastDal gymnastDal = new DAL.Services.GymnastDal(db);
                Gymnasts = new GymnastBL(gymnastDal);
            }
        }
    }

}
}
