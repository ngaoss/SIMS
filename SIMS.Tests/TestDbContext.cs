using Microsoft.EntityFrameworkCore;
using SIMS.Web.Data;

public static class TestDbContext
{
    public static ApplicationDbContext GetTestDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        var context = new ApplicationDbContext(options);
        
      

        return context;
    }

   
    
}