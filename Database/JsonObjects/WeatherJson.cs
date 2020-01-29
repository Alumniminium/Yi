using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace YiX.Database.JsonObjects
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class WeatherJson
    {
        public List<Weather> weather { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }

        public override string ToString() => $"Temp: {main.temp} | Wind Speed: {wind.speed} | Wind Angel: {wind.deg} | Weather: {weather.First().description}|{weather.First().main}";
    }
}