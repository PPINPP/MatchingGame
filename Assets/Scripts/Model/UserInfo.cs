using System;
using Enum;
using Firebase.Firestore;
using UnityEngine.Analytics;

namespace Model
{

  [Serializable]
  class UserInfo : Base
  {
    public string Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Genders Gender { get; set; }
    public Educational EducationalLevel { get; set; }
    public string MedicalHistory { get; set; }
    public string Dementia { get; set; }


    public UserInfo(string password, DateTime dob, Genders gender, Educational educationalLevel,
     string medicalHistory, string dementia) : base()
    {
      Password = password;
      DateOfBirth = dob;
      Gender = gender;
      EducationalLevel = educationalLevel;
      MedicalHistory = medicalHistory;
      Dementia = dementia;
    }

    public UserInfo() : base()
    {
    }

    public UserInfoFs ConvertUserInfoToUserInfoFs()
    {
      UserInfoFs userInfoFs = new UserInfoFs
      {
        Uuid = this.Uuid,
        DateCreated = this.DateCreated.ToString("s"),
        DateUpdated = this.DateUpdated.ToString("s"),
        Password = this.Password,
        DateOfBirth = this.DateOfBirth,
        Gender = this.Gender,
        EducationalLevel = this.EducationalLevel,
        MedicalHistory = this.MedicalHistory,
        Dementia = this.Dementia
      };

      return userInfoFs;
    }
  }

  [FirestoreData]
  public struct UserInfoFs
  {
    [FirestoreProperty] public string Uuid { get; set; }
    [FirestoreProperty] public string DateCreated { get; set; }
    [FirestoreProperty] public string DateUpdated { get; set; }
    [FirestoreProperty] public string Password { get; set; }
    [FirestoreProperty] public DateTime DateOfBirth { get; set; }
    [FirestoreProperty] public Genders Gender { get; set; }
    [FirestoreProperty] public Educational EducationalLevel { get; set; }
    [FirestoreProperty] public string MedicalHistory { get; set; }
    [FirestoreProperty] public string Dementia { get; set; }
  }
}