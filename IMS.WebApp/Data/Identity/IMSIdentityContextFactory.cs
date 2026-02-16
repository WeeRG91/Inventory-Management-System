using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IMS.WebApp.Data.Identity
{
    public class IMSIdentityContextFactory : IDesignTimeDbContextFactory<IMSIdentityContext>
    {
        public IMSIdentityContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IMSIdentityContext>();

            optionsBuilder.UseSqlite("Data Source=ims_identity.db");

            return new IMSIdentityContext(optionsBuilder.Options);
        }
    }
}
