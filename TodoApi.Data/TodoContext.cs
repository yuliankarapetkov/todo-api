using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using TodoApi.Data.Models;

namespace TodoApi.Data
{
    public class TodoContext: IdentityDbContext<TodoUser>
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            :base (options)
        { }

        public DbSet<Todo> Todos { get; set; }
    }
}
