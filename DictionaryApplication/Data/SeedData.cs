using DictionaryApplication.Data;
using DictionaryApplication.Models;

namespace DictionaryApplication.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (!context.Languages.Any())
            {
                var languages = new List<Language>
                {
                    new Language { LangCode = "ENG", Name = "English" },
                    new Language { LangCode = "RUS", Name = "Russian" },
                    new Language { LangCode = "UAH", Name = "Ukrainian" },
                    new Language { LangCode = "SPA", Name = "Spanish" },
                    new Language { LangCode = "GER", Name = "German" },

                };

                context.Languages.AddRange(languages);
                context.SaveChanges();
            }
        }
    }

}
