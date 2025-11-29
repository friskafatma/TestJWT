using Microsoft.EntityFrameworkCore;
using JWTApi.Data;

namespace TestProject
{
    public static class TestHelpers
    {
        public static AppDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}
