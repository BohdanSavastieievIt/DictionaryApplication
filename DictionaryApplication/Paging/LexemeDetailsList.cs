using DictionaryApplication.Models;

namespace DictionaryApplication.Paging
{
    public class LexemeDetailsList
    {
        public IEnumerable<LexemeDetails> LexemeDetails { get; set; } = null!;
        public PagingInfo PagingInfo { get; set; } = null!;
    }
}
