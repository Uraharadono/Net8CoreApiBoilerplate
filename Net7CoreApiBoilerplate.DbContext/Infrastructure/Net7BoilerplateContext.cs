using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Net7CoreApiBoilerplate.DbContext.Entities;
using Net7CoreApiBoilerplate.DbContext.Entities.Identity;
using Net7CoreApiBoilerplate.DbContext.Interceptors;
using System.Diagnostics;

namespace Net7CoreApiBoilerplate.DbContext.Infrastructure
{
    public partial class Net7BoilerplateContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        public Net7BoilerplateContext(DbContextOptions<Net7BoilerplateContext> options) : base(options)
        {
        }

        public static Net7BoilerplateContext Create(string connection)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Net7BoilerplateContext>();
            optionsBuilder.UseSqlServer(connection);
            // optionsBuilder.AddInterceptors(Net7BoilerplateInterceptors.CreateInterceptors());

            // Setup our interceptors
            optionsBuilder.AddInterceptors(BloggingInterceptors.CreateInterceptors());

            // Helps me with debugging stuff
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.LogTo(message => Debug.WriteLine(message)); // https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging

            return new Net7BoilerplateContext(optionsBuilder.Options);
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Logging> Logging { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=Net7CoreApiBoilerplate;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=true;");

                // Setup our interceptors
                optionsBuilder.AddInterceptors(BloggingInterceptors.CreateInterceptors());

                // Helps me with debugging stuff
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.LogTo(message => Debug.WriteLine(message)); // https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS"); // override DB annotation if you like
            base.OnModelCreating(modelBuilder); // DO NOT - under any circumstances - REMOVE THIS LINE. This line basically calls our identity constructor first, and without it NOTHING REGARDING IDENTITY WORKS!

            #region Identity stuff
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");

            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims");
            modelBuilder.Entity<ApplicationUserToken>().ToTable("UserTokens");
            #endregion

            modelBuilder.Entity<Logging>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("Id");
            });

            #region Global filters
            // https://docs.microsoft.com/en-us/ef/core/querying/filters
            // modelBuilder.Entity<Blog>().HasQueryFilter(a => a.PublisherId == PublisherId);
            #endregion

            #region Sequences 
            modelBuilder.HasSequence<long>("BlogSeq")
                .StartsAt(100)
                .IncrementsBy(1)
                .HasMin(100);

            modelBuilder.Entity<Blog>()
                .Property(o => o.Id)
                .HasDefaultValueSql("NEXT VALUE FOR BlogSeq");

            // Sometimes we cannot set the sequence like we did above for the BlogSeq.
            // Reason can be that when scafolding our database it will mark PK of table with "ValueGeneratedNever" 
            // That's why we are going to create "LoggingInterceptor" to help us with inserting data for Logs table. 
            // Another reason for interceptor could be that we have an old system that is still being used while we rework this one. 
            // It could use some obscure logic to fetch next id for Logging record, and we have to reset Sequence before we insert anything. 
            // For more info why I needed this, read the note on top of the file "LoggingInterceptor.cs"
            modelBuilder.HasSequence<long>("LoggingSeq")
                        .StartsAt(2000000)
                        .IncrementsBy(1)
                        .HasMin(2000000);
            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
