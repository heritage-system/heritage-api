1. CREATE Specialty 

{
  "specialtyName": "Cardiology",
  "description": "Specialized in heart-related medical care"
}


2. CREATE DOCTOR
{
  "email": "doctor@example.com",
  "phone": "0123456789",
  "firstName": "John",
  "lastName": "Doe",
  "specialtyId": 1,
  "licenseNumber": "ABC123456",
  "degree": "MD",
  "consultationFee": 500000,
  "gender": "MALE",  // hoặc "FEMALE", "OTHER"
  "yearsOfExperience": 5,
  "bio": "Experienced general physician.",
  "avatar": "https://example.com/avatar.jpg"
}

3. CREATE USER
{
    "phone": "0123456789",
    "email": "admin@gmail.com",
    "password": "123456",
    "firstName": "ADMIN", 
    "lastName": "SYSTEM",
    "dob": "2003-10-02"
}

4. LOGIN 
{
    "phoneOrEmail": "admin@gmail.com",
    "password": "123456"
}

5. UPDATE PROFILE PETIENT
{
    "FirstName": "Đức Đẹp Trai",
    "LastName": "Update Profile",
    "Phone": "0987654321",
    "Gender": "MALE",
    "Dob": "2005-10-02"
}