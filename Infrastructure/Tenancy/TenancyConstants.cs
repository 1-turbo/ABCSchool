namespace Infrastructure.Tenancy
{
    public class TenancyConstants
    {
        public const string TenantIdName = "tenant";
        public const string DefaultPassword = "Turbo@123";
        public const string FirstName = "Turbo";
        public const string LastName = "Ahmed"; 

        public static class Root
        {
            public const string Id = "root";
            public const string Name = "root";
            public const string Email = "admin.root@abcschool.com";
        }
    }
}
