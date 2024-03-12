using Microsoft.EntityFrameworkCore;
using Novda.Server.Models;

namespace Novda.Server.Infrastructure;

public class NovdaDbContext(DbContextOptions<NovdaDbContext> options) : DbContext(options)
{
    public virtual DbSet<NovdaApplication> NovdaApplications { get; set; }
}
