using Microsoft.EntityFrameworkCore;
using SyncroForge.Models;
using Taskk = SyncroForge.Models.Task;


namespace SyncroForge.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions options) : base(options) { }


        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>().HasOne(i => i.User).WithMany(j => j.RefreshTokens).HasForeignKey(k => k.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().HasIndex(i => i.Email).IsUnique();
           modelBuilder.Entity<Employee>().HasOne(i => i.Company).WithMany(c=>c.Employees).HasForeignKey(k => k.CompanyId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Company>().HasOne(i => i.Creator).WithMany(c => c.CreatedCompanies).HasForeignKey(k => k.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Department>().HasOne(i => i.Company).WithMany(c => c.Departments).HasForeignKey(k => k.CompanyId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Company>().HasIndex(i => i.Name).IsUnique();

            modelBuilder.Entity<Employee>().HasOne(i => i.User).WithMany(c => c.Employees).HasForeignKey(k => k.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Employee>().HasOne(i=>i.Rule).WithMany(e=>e.Employees).HasForeignKey(u => u.RuleId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<DepartmentEmployee>().HasOne(i => i.Employee).WithMany(e => e.DepartmentEmployees).HasForeignKey(u => u.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<DepartmentEmployee>().HasOne(i => i.Department).WithMany(e => e.DepartmentEmployees).HasForeignKey(u => u.DepartmentId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Taskk>().HasOne(i => i.Department).WithMany(d=>d.Tasks).HasForeignKey(u=>u.DepartmentId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Taskk>().HasOne(i => i.ParentTask).WithMany(t => t.SubTasks).HasForeignKey(u => u.ParentTaskId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<TaskHistory>().HasOne(i => i.Task).WithMany(t => t.Histories).HasForeignKey(u => u.TaskId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<TaskHistory>().HasOne(i => i.Employee).WithMany(t => t.TaskHistories).HasForeignKey(u => u.EmployeeId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Taskk>().HasOne(i => i.Assignee).WithMany(t => t.AssignedTasks).HasForeignKey(u => u.AssigneeId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Taskk>().HasOne(i => i.Creator).WithMany(t => t.CreatedTasks).HasForeignKey(u => u.CreatedById).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Taskk>().HasOne(i=>i.Status).WithMany(t=>t.Tasks).HasForeignKey(u=>u.StatusId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CompanyInviteUser>().HasOne(i=>i.Company).WithMany(j=>j.invitedUsers).HasForeignKey(k=>k.CompanyId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CompanyInviteUser>().HasOne(i => i.User).WithMany(j => j.Invites).HasForeignKey(k => k.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Attendance>().HasOne(i=>i.Employee).WithMany(j=>j.Attendances).HasForeignKey(k=>k.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Salary>().HasOne(i => i.Employee).WithMany(j => j.Salaries).HasForeignKey(k => k.EmployeeId).OnDelete(DeleteBehavior.NoAction);



        }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Rule> Rule { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentEmployee> DepartmentEmployees { get; set; }
        public DbSet<Taskk> Tasks { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<Count> Counts { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<CompanyInviteUser> CompaniesInviteduser { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Salary> Salaries { get; set; }


    }
}
