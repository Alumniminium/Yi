using System.Diagnostics.CodeAnalysis;

namespace YiX.Database.JsonObjects
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Wind
    {
        public double speed { get; set; }
        public double deg { get; set; }
    }
}