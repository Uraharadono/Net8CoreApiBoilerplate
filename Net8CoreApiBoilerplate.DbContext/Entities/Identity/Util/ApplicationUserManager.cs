namespace Net8CoreApiBoilerplate.DbContext.Entities.Identity.Util
{
    // NOTE TO SELF: I don't need this because I figured how to use it all in UserSeeder.cs


    //public class ApplicationUserManager : Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>
    //{
    //    public ApplicationUserManager(Microsoft.AspNetCore.Identity.IUserStore<ApplicationUser> store)
    //        : base(store)
    //    {
    //    }

    //    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, DbContext context)
    //    {
    //        if (!(context is MyContext ctx)) return null;

    //        var manager = new ApplicationUserManager(new ApplicationUserStore(ctx));

    //        /*
    //        // Configure validation logic for usernames
    //        manager.UserValidator = new UserValidator<User, long>(manager)
    //        {
    //            AllowOnlyAlphanumericUserNames = false,
    //            RequireUniqueEmail = true
    //        };

    //        // Configure validation logic for passwords
    //        manager.PasswordValidator = new PasswordValidator { RequiredLength = 6 };

    //        // Configure user lockout defaults
    //        manager.UserLockoutEnabledByDefault = false;
    //        manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //        manager.MaxFailedAccessAttemptsBeforeLockout = 5;

    //        // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
    //        // You can write your own provider and plug it in here.
    //        //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User, int>
    //        //{
    //        //    MessageFormat = "Your security code is {0}"
    //        //});

    //        //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User, int>
    //        //{
    //        //    Subject = "Security Code",
    //        //    BodyFormat = "Your security code is {0}"
    //        //});

    //        var dataProtectionProvider = options.DataProtectionProvider;

    //        if (dataProtectionProvider != null)
    //            manager.UserTokenProvider = new DataProtectorTokenProvider<User, long>(dataProtectionProvider.Create("EmployeeMonitor Identity"));
    //        */

    //        return manager;
    //    }
    //}
}
