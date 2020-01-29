using System.Diagnostics.CodeAnalysis;

namespace YiX.Database.JsonObjects
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Weather
    {
        public string description { get; set; }
        public string main { get; set; }
    }
}