using Microsoft.EntityFrameworkCore;

namespace ATH.DAL.DataAccess;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options) {}
}