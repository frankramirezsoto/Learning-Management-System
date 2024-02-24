using CanvasLMS.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;

namespace CanvasLMS.Data
{
    public class LMSDbContext : DbContext
    {
        public LMSDbContext(DbContextOptions<LMSDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Adding Identity Specification 
            modelBuilder.Entity<Professor>()
                .Property(e => e.Id)
                .ValueGeneratedNever(); // Turns off identity specification for Professor

            modelBuilder.Entity<Student>()
                .Property(e => e.Id)
                .ValueGeneratedNever(); // Turns off identity for Student

            modelBuilder.Entity<AcademicLevel>()
            .HasKey(x => x.Id)
            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity<Faculty>()
            .HasKey(x => x.Id)
            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity<Career>()
            .HasKey(x => x.Id)
            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity<CourseCycle>()
            .HasKey(x => x.Id)
            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity<EvaluationItem>()
            .HasKey(x => x.Id)
            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity<EvaluationTask>()
            .HasKey(x => x.Id)
            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity<ModulePath>()
            .HasKey(x => x.Id)
            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
            modelBuilder.Entity<TaskSubmission>()
            .HasKey(x => x.Id)
            .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //Setting the relationships for the CourseCycle entity
            modelBuilder.Entity<Course>()
            .HasMany(e => e.Cycles)
            .WithMany(e => e.Courses)
            .UsingEntity<CourseCycle>();

            modelBuilder.Entity<Cycle>()
            .HasMany(e => e.Courses)
            .WithMany(e => e.Cycles)
            .UsingEntity<CourseCycle>();

            //Setting Relationship for the Attendance Entity with the Class
            modelBuilder.Entity<Attendance>()
            .HasOne(e => e.Class)
            .WithMany(e => e.Attendance)
            .HasForeignKey(r => new { r.ClassId, r.CourseCycleId });

            //Setting Relationship for the ModulePath Entity with the Module
            modelBuilder.Entity<ModulePath>()
            .HasOne(e => e.Module)
            .WithMany(e => e.ModulePaths)
            .HasForeignKey(r => new { r.ModuleId, r.CourseCycleId });

            //Seeding Database with Data
            //Professor User
            modelBuilder.Entity<Professor>().HasData(new Professor
            {
                Id = 166666666,
                FirstName = "Frank",
                LastName = "Ramirez",
                Email = "framirezs869@ulacit.ed.cr",
                Password = "Admin123!",
            });

            //Student User
            modelBuilder.Entity<Student>().HasData(new Student
            {
                Id = 116800869,
                FirstName = "Josue",
                LastName = "Ramirez",
                Email = "josue.ramirez@ulacit.ed.cr",
                Password = "Ulacit123!",
            });

            //Adding Faculties
            modelBuilder.Entity<Faculty>().HasData(new Faculty
            {
                Id = 1,
                Name = "Engineering"

            });

            //Adding Academic Levels
            modelBuilder.Entity<AcademicLevel>().HasData(new AcademicLevel
            {
                Id = 1,
                Name = "Bachelor Degree"
            });

            //Adding Careers
            modelBuilder.Entity<Career>().HasData(new Career
            {
                Id = 101,
                Name = "Computer Science",
                FacultyId = 1,
                AcademicLevelId = 1
            });

            modelBuilder.Entity<Career>().HasData(new Career
            {
                Id = 102,
                Name = "Business Intelligence",
                FacultyId = 1,
                AcademicLevelId = 1
            });
            //Adding Cycles
            modelBuilder.Entity<Cycle>().HasData(new Cycle
            {
                Id = "1CO24",
                StartDate = new DateTime(2024, 1, 15),
                EndDate = new DateTime(2024, 4, 15),
                isCurrentCycle = true,
            });
            //Adding Courses
            modelBuilder.Entity<Course>().HasData(new Course
            {
                Id = "165003",
                Name = "Web Applications Design",
                Description = "Web apps design course is a comprehensive program that teaches students the principles and practical skills needed to create visually appealing and user-friendly web applications. Participants will learn key concepts in user interface (UI) and user experience (UX) design, including wireframing, prototyping, and usability testing. The course may cover design tools and software, as well as coding languages relevant to web development. Throughout the course, students will gain hands-on experience by working on real-world projects, honing their ability to design engaging and functional web interfaces. Whether you're a beginner or looking to enhance your design skills, this course provides a solid foundation for creating effective and aesthetically pleasing web applications."
            });

            //Adding Courses
            modelBuilder.Entity<Course>().HasData(new Course
            {
                Id = "169005",
                Name = "System Design, Analysis and Implementation",
                Description = "System Design, Analysis, and Implementation is a comprehensive program that equips students with the knowledge and skills necessary to architect, analyze, and implement complex systems. Participants will delve into the fundamentals of system design, learning how to conceptualize and model systems, conduct thorough analysis of requirements, and develop effective solutions. The course covers various methodologies for system design and emphasizes the importance of a systematic approach to problem-solving. Students will gain practical experience in implementing systems, including coding and testing, and understand the principles of scalability, efficiency, and maintainability. By the end of the course, participants will be well-versed in the entire lifecycle of system development, from initial design to successful implementation, preparing them for roles in software development, IT consulting, and other technology-driven fields."
            });
            //Adding Career Course
            modelBuilder.Entity<Career>()
            .HasMany(s => s.Courses)
            .WithMany(c => c.Careers)
            .UsingEntity(j => j.ToTable("CareerCourse")
                           .HasData(
                               new { CareersId = 101, CoursesId = "169005" },
                               new { CareersId = 101, CoursesId = "165003" }
            ));
            //Adding Professor Career
            modelBuilder.Entity<Career>()
            .HasMany(s => s.Professors)
            .WithMany(c => c.Careers)
            .UsingEntity(j => j.ToTable("ProfessorCareer")
                           .HasData(
                               new { ProfessorsId = 166666666, CareersId = 101 },
                               new { ProfessorsId = 166666666, CareersId = 102 }
            ));
        }

        public DbSet<Professor> Professors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<AcademicLevel> AcademicLevels { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Career> Careers { get; set; }
        public DbSet<Cycle> Cycles { get; set; }
        public DbSet<CourseCycle> CourseCycles { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<EvaluationItem> EvaluationItems { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModulePath> ModulePaths { get; set; }
        public DbSet<EvaluationTask> EvaluationTasks { get; set; }
        public DbSet<TaskSubmission> TaskSubmissions { get; set; }
    }
}
