using System;

namespace StoreApp.Business
{
    public interface IBusinessRules {
        bool ValidateUserName(string name);
    }

    public class BusinessRules : IBusinessRules {
        public bool ValidateUserName(string name)
        {
            if (name.Trim().Length == 0 || name == null) return false;

            foreach (char c in name.ToCharArray())
            {
                if (!Char.IsLetter(c))
                {
                    if (c == '.' || c == ' ' || c == '-') continue;
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}