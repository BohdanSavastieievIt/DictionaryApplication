using DictionaryApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace DictionaryApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Language> Languages { get; set; }
        public new DbSet<User> Users { get; set; }
        public DbSet<UserDictionary> UserDictionaries { get; set; }
        public DbSet<Lexeme> Lexemes { get; set; }
        public DbSet<LexemeDefinition> LexemeDefinitions { get; set; }
        public DbSet<LexemeUsageExample> LexemeUsageExamples { get; set; }
        public DbSet<DictionaryLexemePair> DictionaryLexemePairs { get; set; }
        public DbSet<LexemePair> LexemePairs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
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
                .HasIndex(e => new { e.StudiedLangId, e.TranslationLangId })
                .IsUnique();

            builder.Entity<Language>()
                .HasMany(lg => lg.Lexemes)
                .WithOne(l => l.LexemeLanguage)
                .HasForeignKey(l => l.LangId);

            // User configuring
            builder.Entity<User>()
                .HasOne(u => u.IdentityUser)
                .WithOne()
                .HasForeignKey<User>(u => u.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .Property(fn => fn.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<User>()
                .Property(fn => fn.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<User>()
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

            // Lexeme configuring
            builder.Entity<Lexeme>()
                .Property(l => l.Word)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<Lexeme>()
                .Property(l => l.TotalTestAttempts)
                .HasDefaultValue(0);

            builder.Entity<Lexeme>()
                .Property(l => l.CorrectTestAttempts)
                .HasDefaultValue(0);

            builder.Entity<Lexeme>()
                .HasMany(ld => ld.LexemeDefinitions)
                .WithOne(l => l.Lexeme)
                .HasForeignKey(ld => ld.LexemeId)
                .OnDelete(DeleteBehavior.Cascade);

            // LexemeDefinition configuring

            builder.Entity<LexemeDefinition>()
                .Property(ld => ld.Definition)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Entity<LexemeDefinition>()
                .HasMany(ld => ld.LexemeUsageExamples)
                .WithOne(l => l.LexemeDefinition)
                .HasForeignKey(ld => ld.LexemeDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            // LexemeUsageExample configuring
            builder.Entity<LexemeUsageExample>()
                .Property(ld => ld.UsageExample)
                .HasMaxLength(1000)
                .IsRequired();

            // DictionaryLexemePair configuring
            builder.Entity<DictionaryLexemePair>()
                .HasKey(dlp => new { dlp.UserDictionaryId, dlp.LexemeId });

            builder.Entity<UserDictionary>()
                .HasMany(ud => ud.DictionaryLexemePairs)
                .WithOne(dlp => dlp.UserDictionary)
                .HasForeignKey(dlp => dlp.UserDictionaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Lexeme>()
                .HasMany(ud => ud.DictionaryLexemePairs)
                .WithOne(dlp => dlp.Lexeme)
                .HasForeignKey(dlp => dlp.LexemeId)
                .OnDelete(DeleteBehavior.Cascade);

            // LexemePair configuring
            builder.Entity<LexemePair>()
                .HasKey(lp => new { lp.Lexeme1Id, lp.Lexeme2Id });

            builder.Entity<Lexeme>()
                .HasMany(ud => ud.Lexeme1Pairs)
                .WithOne(dlp => dlp.Lexeme1)
                .HasForeignKey(dlp => dlp.Lexeme1Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Lexeme>()
                .HasMany(ud => ud.Lexeme2Pairs)
                .WithOne(dlp => dlp.Lexeme2)
                .HasForeignKey(dlp => dlp.Lexeme2Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Lexeme>()
                .ToSqlQuery(@"CREATE TRIGGER TRG_DeleteLexemePairs 
                  ON Lexemes
                  INSTEAD OF DELETE
                  AS
                  BEGIN
                      DELETE lp
                      FROM LexemePairs lp
                      JOIN deleted d ON lp.Lexeme1Id = d.Id OR lp.Lexeme2Id = d.Id
                  END;");

            builder.Entity<LexemePair>()
                .ToTable(t => t.HasCheckConstraint("CHK_Dictionary_Languages_Not_Equal", "Lexeme1Id <> Lexeme2Id"))
                .HasIndex(e => new { e.Lexeme1Id, e.Lexeme2Id })
                .IsUnique();
            
            builder.Entity<LexemePair>()
                .Property(p => p.LexemeRelationType)
                .HasConversion<string>();
        }
    }
}