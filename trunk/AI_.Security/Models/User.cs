using System;
using AI_.Data;

namespace AI_.Security.Models
{
    public class User : ModelBase
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PasswordQuestion { get; set; }

        public string PasswordAnswer { get; set; }

        public bool IsApproved { get; set; }

        public object ProviderUserKey { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsLocked { get; set; }

        public DateTime LastActivityDate { get; set; }

        #region Equality members

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.UserName, UserName)
                   && Equals(other.Password, Password)
                   && Equals(other.Email, Email)
                   && Equals(other.PasswordQuestion, PasswordQuestion)
                   && Equals(other.PasswordAnswer, PasswordAnswer)
                   && other.IsApproved.Equals(IsApproved)
                   && Equals(other.ProviderUserKey, ProviderUserKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (User)) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (UserName != null ? UserName.GetHashCode() : 0);
                result = (result*397) ^ (Password != null ? Password.GetHashCode() : 0);
                result = (result*397) ^ (Email != null ? Email.GetHashCode() : 0);
                result = (result*397) ^ (PasswordQuestion != null ? PasswordQuestion.GetHashCode() : 0);
                result = (result*397) ^ (PasswordAnswer != null ? PasswordAnswer.GetHashCode() : 0);
                result = (result*397) ^ IsApproved.GetHashCode();
                result = (result*397) ^ (ProviderUserKey != null ? ProviderUserKey.GetHashCode() : 0);
                return result;
            }
        }

        public static bool operator ==(User left, User right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(User left, User right)
        {
            return !Equals(left, right);
        }

        #endregion
    }


    public class ModelBase : IIdentifiable<int>
    {
        #region IIdentifiable<int> Members

        public int ID { get; protected set; }

        #endregion
    }
}