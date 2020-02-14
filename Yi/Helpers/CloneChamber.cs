using Newtonsoft.Json;

namespace Yi.Helpers
{
    public struct CloneChamber
    {
        public static T Clone<T>(T source)
        {
            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}