namespace DictionaryApplication.Models.DbModels
{
    public class DictionaryLanguagesInfo
    {
        public int UserDictionaryId { get; set; }
        public string StudiedLang { get; } = null!;
        public string TranslationLang { get; } = null!;
    }
}
