import instance from '../axiosConfig';


export async function getPatient(patientKey) {
  const response = await instance.get(`/patient/by-key/${patientKey}`);
  return response.data;
}


export async function updatePatient(patient) {
  return instance.put('/patient/update', {
    PatientKey: patient.PatientKey,
    PatientName: patient.PatientName,
    Email: patient.Email,
    Phone: patient.Phone,
    Address: patient.Address
      ? {
          CityName: patient.Address.CityName,
          StreetName: patient.Address.StreetName,
          HouseNumber: patient.Address.HouseNumber,
          PostalCode: patient.Address.PostalCode,
        }
      : null,
  });
}

export async function changePassword({ patientKey, currentPassword, newPassword }) {
  return instance.post('/patient/change-password', {
    PatientKey: patientKey,
    CurrentPassword: currentPassword,
    NewPassword: newPassword,
  });
}
