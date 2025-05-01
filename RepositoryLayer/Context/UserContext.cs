using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Entity;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Context
{
    public class UserContext : DbContext
    {
        //at runtime the object is created on its own  ,(because of this dependency injection is possible
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<NotesEntity> Notes { get; set; }
        public DbSet<NoteCollaboratorEntity> NoteCollaborators { get; set; }
        public DbSet<LabelEntity> Labels { get; set; }
    }
}
