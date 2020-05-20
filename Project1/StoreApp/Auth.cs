namespace StoreApp.Auth
{
    /// <summary>
    /// Constants used for accessing authorization claims.
    /// </summary>
    public static class Claim
    {
        public const string UserName = "UserName";
        public const string UserId = "UserId";
        public const string Role = "Role";
    }

    /// <summary>
    /// Constants used for determining user roles.
    /// </summary>
    public static class Role
    {
        public const string Customer = "Customer";
        public const string Administrator = "Administrator";
        public const string Guest = "Guest";
    }
}