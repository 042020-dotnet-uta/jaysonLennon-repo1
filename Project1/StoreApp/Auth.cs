namespace StoreApp.Auth
{
    public static class Claim
    {
        public const string UserName = "UserName";
        public const string UserId = "UserId";
        public const string Role = "Role";
        public const string LastPermissionUpdate = "LastPermissionUpdate";
    }

    public static class Role
    {
        public const string Customer = "Customer";
        public const string Administrator = "Administrator";
        public const string Guest = "Guest";
    }
}