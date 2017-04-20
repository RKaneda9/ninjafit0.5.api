
namespace NinjaFit.Api.Models
{
    public class Wod
    {
        public string   Title    { get; set; }
        public string   TrackBy  { get; set; }
        public string[] Contents { get; set; }
    }

    public class WodSummary : Wod
    {
        public int WodCount { get; set; }
    }
}