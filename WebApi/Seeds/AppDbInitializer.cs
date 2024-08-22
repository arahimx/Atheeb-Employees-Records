using Infrastructure.Data;
using Core.Entity;

namespace RestApi.Seeds
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                if (!context!.Departments.Any())
                {
                    context.Departments.AddRange(
                    new Departments()
                    {
                        Title = "IT",
                    },
                   new Departments()
                   {
                       Title = "HR",
                   });
                    context.SaveChanges();
                }
                if (!context!.Skills.Any())
                {
                    context.Skills.AddRange(
                    new Skills()
                    {
                        Title = "C#",
                    },
                    new Skills()
                    {
                        Title = "Web Development",
                    },
                    new Skills()
                    {
                        Title = "WebApi",
                    },
                    new Skills()
                    {
                        Title = "JavaScript",
                    },
                    new Skills()
                    {
                        Title = "FrontEnd",
                    },
                    new Skills()
                    {
                        Title = "BackEnd",
                    },
                    new Skills()
                    {
                        Title = "Python",
                    },
                    new Skills()
                    {
                        Title = "Laravel",
                    },
                    new Skills()
                    {
                        Title = "PHP",
                    },
                    new Skills()
                    {
                        Title = "SQL",
                    },
                    new Skills()
                    {
                        Title = "NoSQL",
                    },
                    new Skills()
                    {
                        Title = "MySQL",
                    },
                    new Skills()
                    {
                        Title = "Scrum",
                    },
                   new Skills()
                   {
                       Title = "Agile",
                   });
                    context.SaveChanges();
                }
            }
        }

    }
}