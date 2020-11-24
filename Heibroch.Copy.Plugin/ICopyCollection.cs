using System.Collections.Generic;

namespace Heibroch.Copy.Plugin
{
    public interface ICopyCollection
    {
        void Add(string text);

        List<string> QueryResults { get; }
    }
}
