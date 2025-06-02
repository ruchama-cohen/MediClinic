namespace BLL.API
{
    public interface IBL
    {
        IAuthService AuthService { get; set; }
        IPatientService PatientService { get; set; }
        IAppointmentService AppointmentService { get; set; }
        IClinicServiceService ClinicServiceService { get; set; }
        IPasswordService  PasswordService { get; set; }
        IPatientService patientService { get; set; }
    }
}