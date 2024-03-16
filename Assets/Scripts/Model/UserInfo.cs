using System;
using System.Collections.Generic;
using Enum;
using Firebase.Firestore;
using Utils;

namespace Model
{
  [Serializable]
  class UserInfo : Base
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public GendersEnum Gender { get; set; }
    public EducationalEnum EducationalLevel { get; set; }
    public Dictionary<string, bool> MedicalHistory { get; set; }
    public DementiaStageEnum DementiaStage { get; set; }


    public UserInfo(string username, string password, DateTime dob, GendersEnum gender, EducationalEnum educationalLevel,
     Dictionary<string, bool> medicalHistory, DementiaStageEnum dementiaStage) : base()
    {
      Username = username;
      Password = password;
      DateOfBirth = dob;
      Gender = gender;
      EducationalLevel = educationalLevel;
      MedicalHistory = medicalHistory;
      DementiaStage = dementiaStage;
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
        Username = this.Username,
        Password = this.Password,
        DateOfBirth = this.DateOfBirth.ToString("s"),
        Gender = this.Gender.ToString(),
        EducationalLevel = this.EducationalLevel.ToString(),
        MedicalHistory = this.MedicalHistory,
        DementiaStage = this.DementiaStage.ToString()
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
    [FirestoreProperty] public string Username { get; set; }
    [FirestoreProperty] public string Password { get; set; }
    [FirestoreProperty] public string DateOfBirth { get; set; }
    [FirestoreProperty] public string Gender { get; set; }
    [FirestoreProperty] public string EducationalLevel { get; set; }
    [FirestoreProperty] public Dictionary<string, bool> MedicalHistory { get; set; }
    [FirestoreProperty] public string DementiaStage { get; set; }

    public override string ToString()
    {
      return StringHelper.ToStringObj(this);
    }
  }
}