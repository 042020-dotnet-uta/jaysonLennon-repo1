using System;

/// <summary>
/// Business rules that should be applied to the system.
/// </summary>
namespace StoreApp.Business
{
    /// <summary>
    /// Interface for various business rules.
    /// </summary>
    public interface IBusinessRules {
        /// <summary>
        /// Determines whether the name of a user passes validation rules.
        /// </summary>
        /// <param name="name">First or last name of a user.</param>
        /// <returns>Boolean indicating whether the name is acceptable.</returns>
        bool ValidateUserName(string name);
    }

    /// <summary>
    /// Main implementation of the business rules.
    /// </summary>
    public class BusinessRules : IBusinessRules {

        /// <summary>
        /// Determines whether the name of a user is valid.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <returns>Boolean indicating whether the name is acceptable.</returns>
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