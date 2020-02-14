using System.Diagnostics.CodeAnalysis;

namespace Yi.Database.JsonObjects
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Weather
    {
        public string description { get; set; }
        public string main { get; set; }
    }
}