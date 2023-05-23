using DictionaryApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using DictionaryApplication.Models;

namespace DictionaryApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Language> Languages { get; set; }
        public new DbSet<ApplicationUser> Users { get; set; }
        public DbSet<UserDictionary> UserDictionaries { get; set; }
        public DbSet<Lexeme> Lexemes { get; set; }
        public DbSet<LexemeTranslationPair> LexemeTranslationPairs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
            SeedData.Initialize(this);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Language configuring
            builder.Entity<Language>()
                .Property(l => l.LangCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.Entity<Language>()
                .Property(n => n.Name)
                .HasMaxLength(30)
                .IsRequired();

            builder.Entity<Language>()
                .Property(l => l.Location)
                .HasMaxLength(1000);

            builder.Entity<Language>()
                .HasMany(l => l.StudiedUserDictionaries)
                .WithOne(d => d.StudiedLanguage)
                .HasForeignKey(d => d.StudiedLangId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Language>()
                .HasMany(l => l.TranslationUserDictionaries)
                .WithOne(d => d.TranslationLanguage)
                .HasForeignKey(d => d.TranslationLangId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserDictionary>()
                .ToTable(t => t.HasCheckConstraint("CHK_Dictionary_Languages_Not_Equal", "StudiedLangId <> TranslationLangId"))
                .HasIndex(e => new { e.StudiedLangId, e.TranslationLangId });

            builder.Entity<Language>()
                .HasMany(lg => lg.Lexemes)
                .WithOne(l => l.LexemeLanguage)
                .HasForeignKey(l => l.LangId);

            // User configuring

            builder.Entity<ApplicationUser>()
                .Property(fn => fn.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .Property(fn => fn.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .HasMany(ud => ud.UserDictionaries)
                .WithOne(u => u.User)
                .HasForeignKey(ud => ud.UserId);

            // UserDictionary configuring
            builder.Entity<UserDictionary>()
                .Property(ud => ud.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<UserDictionary>()
                .Property(ud => ud.Description)
                .HasMaxLength(1000);

            builder.Entity<UserDictionary>()
                .HasMany(l => l.Lexemes)
                .WithOne(ud => ud.Dictionary)
                .HasForeignKey(ud => ud.DictionaryId);

            // Lexeme configuring
            builder.Entity<Lexeme>()
                .Property(l => l.Word)
                .HasMaxLength(100)
                .IsRequired();

            builder.Entity<Lexeme>()
                .Property(l => l.Description)
                .HasMaxLength(1000);

            builder.Entity<Lexeme>()
                .Property(l => l.TotalTestAttempts)
                .HasDefaultValue(0);

            builder.Entity<Lexeme>()
                .Property(l => l.CorrectTestAttempts)
                .HasDefaultValue(0);

            // LexemeTranslationPair configuring
            builder.Entity<LexemeTranslationPair>()
                .HasKey(lp => new { lp.LexemeId, lp.TranslationId });

            builder.Entity<Lexeme>()
                .HasMany(ud => ud.LexemePairs)
                .WithOne(dlp => dlp.Lexeme)
                .HasForeignKey(dlp => dlp.LexemeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Lexeme>()
                .HasMany(ud => ud.TranslationPairs)
                .WithOne(dlp => dlp.Translation)
                .HasForeignKey(dlp => dlp.TranslationId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Lexeme>()
            //    .ToSqlQuery(@"CREATE TRIGGER TRG_DeleteLexemePairs 
            //      ON Lexemes
            //      INSTEAD OF DELETE
            //      AS
            //      BEGIN
            //          DELETE lp
            //          FROM LexemePairs lp
            //          JOIN deleted d ON lp.Lexeme1Id = d.Id OR lp.Lexeme2Id = d.Id
            //      END;");

            builder.Entity<LexemeTranslationPair>()
                .ToTable(t => t.HasCheckConstraint("CHK_Dictionary_Languages_Not_Equal", "LexemeId <> TranslationId"))
                .HasIndex(e => new { e.LexemeId, e.TranslationId })
                .IsUnique();

            RenameIdentityTables(builder);
        }

        protected void RenameIdentityTables(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Dict");
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }
    }
}