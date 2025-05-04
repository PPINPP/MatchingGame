using System.Collections.Generic;
using Firebase.Firestore;

namespace RegistryForm
{
    [FirestoreData]
    public class UserRegisterForm
    {
        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public string Password { get; set; }

        [FirestoreProperty]
        public string DateOfBirth { get; set; }

        [FirestoreProperty]
        public string Gender { get; set; }

        [FirestoreProperty]
        public Dictionary<string, bool> MedicalHistory { get; set; }

        [FirestoreProperty]
        public string DementiaStage { get; set; }

        [FirestoreProperty]
        public string EducationalLevel { get; set; }

        [FirestoreProperty]
        public string Uuid { get; set; }

        [FirestoreProperty]
        public string DateCreated { get; set; }

        [FirestoreProperty]
        public string DateUpdated { get; set; }

        [FirestoreProperty]
        public string Classify { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }
        [FirestoreProperty]
        public bool TutorialPassed { get; set; }
        [FirestoreProperty]
        public int DayPassed {get; set;}
        [FirestoreProperty]
        public List<int> FuzzyProperties {get;set;}
        [FirestoreProperty]
        public bool IsFirstLogin {get;set;}
        [FirestoreProperty]
        public string LastLogin {get;set;}
    }
}
