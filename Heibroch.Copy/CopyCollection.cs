using Heibroch.Copy.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace Heibroch.Copy
{
    public class CopyCollection : ICopyCollection
    {
        public const int MaxCopyCount = 10;

        public List<string> QueryResults { get; }

        public CopyCollection() => QueryResults = new List<string>();

        public void Add(string text)
        {
            if (QueryResults.Contains(text)) return;

            var lastCopied = QueryResults.FirstOrDefault();
            if (lastCopied == null)
            {
                QueryResults.Add(text);
                return;
            }

            if (lastCopied == text) return;

            QueryResults.Insert(0, text);

            if (MaxCopyCount > QueryResults.Count) return;

            QueryResults.RemoveAt(QueryResults.Count - 1);
        }
    }
}
