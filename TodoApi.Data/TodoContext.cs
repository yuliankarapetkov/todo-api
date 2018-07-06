﻿using Microsoft.EntityFrameworkCore;

using TodoApi.Data.Models;

namespace TodoApi.Data
{
    public class TodoContext: DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            :base (options)
        { }

        public DbSet<Todo> Todos { get; set; }
    }
}
