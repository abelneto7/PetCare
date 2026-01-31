using Microsoft.EntityFrameworkCore;

namespace PetCare.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets entram aqui depois (Tutor, Pet, RegistroVacina, Usuario...)
    }
}
