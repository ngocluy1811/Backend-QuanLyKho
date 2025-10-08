using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FertilizerWarehouseAPI.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
         optionsBuilder.UseNpgsql("Host=dpg-d3ic4sbipnbc73e2d6hg-a.oregon-postgres.render.com;Database=fertilizer_warehouse;Username=postgre;Password=heTbBQvCtjDUazLiQyuF5vWHnFQ8b3G7;Port=5432;SSL Mode=Require;Trust Server Certificate=True;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
