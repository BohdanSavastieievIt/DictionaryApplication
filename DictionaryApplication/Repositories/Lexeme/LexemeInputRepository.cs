using DictionaryApplication.Data;
using DictionaryApplication.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DictionaryApplication.Repositories
{
    public class LexemeInputRepository : ILexemeInputRepository
    {
        private readonly ApplicationDbContext _context;
        public LexemeInputRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        private void CheckEntity(object? entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }
        public async Task CreateAsync(int dictionaryId, LexemeInput lexemeInput)
        {
            var userDictionary = await _context.UserDictionaries.FirstOrDefaultAsync(x => x.Id == dictionaryId);
            CheckEntity(userDictionary);
            var lexeme = new Lexeme
            {
                DictionaryId = dictionaryId,
                LangId = userDictionary.StudiedLangId,
                Word = lexemeInput.Lexeme,
                Description = lexemeInput.Description
            };
            await _context.Lexemes.AddAsync(lexeme);

            foreach (var translationWord in lexemeInput.Translations)
            {
                var translation = new Lexeme
                {
                    DictionaryId = dictionaryId,
                    LangId = userDictionary.TranslationLangId,
                    Word = translationWord
                };

                var lexemeTranslationPair = new LexemeTranslationPair
                {
                    Lexeme = lexeme,
                    Translation = translation
                };

                await _context.Lexemes.AddAsync(translation);
                await _context.LexemeTranslationPairs.AddAsync(lexemeTranslationPair);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int lexemeId, LexemeInput lexemeInput)
        {
            var lexeme = await _context.Lexemes.FirstOrDefaultAsync(x => x.Id == lexemeId);
            CheckEntity(lexeme);

            lexeme.Word = lexemeInput.Lexeme;
            lexeme.Description = lexemeInput.Description;

            var oldTranslations = _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId).Select(x => x.Translation);
            _context.Lexemes.RemoveRange(oldTranslations);
            var oldPairs = _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId);
            _context.LexemeTranslationPairs.RemoveRange(oldPairs);

            foreach (var translationWord in lexemeInput.Translations)
            {
                var transLang = await _context.UserDictionaries.FirstAsync(d => d.Id == lexeme.DictionaryId);
                var translation = new Lexeme
                {
                    DictionaryId = lexeme.DictionaryId,
                    LangId = transLang.TranslationLangId,
                    Word = translationWord
                };

                var lexemeTranslationPair = new LexemeTranslationPair
                {
                    Lexeme = lexeme,
                    Translation = translation
                };

                await _context.Lexemes.AddAsync(translation);
                await _context.LexemeTranslationPairs.AddAsync(lexemeTranslationPair);
            }


            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int lexemeId)
        {
            var lexeme = await _context.Lexemes.FirstOrDefaultAsync(x => x.Id == lexemeId);
            CheckEntity(lexeme);

            List<Lexeme?> translations = await _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId).Select(x => x.Translation).ToListAsync();
            foreach (Lexeme? translation in translations)
            {
                if (translation != null)
                {
                    _context.Lexemes.Remove(translation);
                }
            }

            _context.Lexemes.Remove(lexeme);
            var pairs = _context.LexemeTranslationPairs.Where(x => x.LexemeId == lexemeId);
            _context.LexemeTranslationPairs.RemoveRange(pairs);

            _context.SaveChanges();

        }

        public async Task<LexemeInput?> GetByIdAsync(int lexemeId)
        {
            var lexeme = await _context.Lexemes.FirstOrDefaultAsync(x => x.Id == lexemeId);
            CheckEntity(lexeme);

            var translations = _context.Lexemes.Where(x =>
                        _context.LexemeTranslationPairs
                            .Where(y => y.LexemeId == lexeme.Id)
                            .Select(y => y.TranslationId).Contains(x.Id))
                        .Select(x => x.Word).ToList();

            var result = new LexemeInput
            {
                Lexeme = lexeme.Word,
                Translations = translations,
                Description = lexeme.Description
            };

            return result;
        }

        public async Task<List<LexemeInput>> GetAllAsync(params int[] userDictionaryIds)
        {
            var result = new List<LexemeInput>();
            List<int> studiedLexemeIds = await _context.Lexemes.Where(x => userDictionaryIds.Contains(x.DictionaryId)
                && x.LexemePairs != null && x.LexemePairs.Count > 0).Select(x => x.Id).ToListAsync();
            foreach (var lexemeId in studiedLexemeIds)
            {
                var lexeme = await GetByIdAsync(lexemeId);
                if (lexeme != null)
                {
                    result.Add(lexeme);
                }
            }

            return result;
        }
    }
}
