using System.Text.RegularExpressions;

namespace RuterApp.Lib
{
    public class Station
    {
        public string Name { get; private set; }

        public void SetStationName(RuterApiStationNameResult stationName)
        {
            string regex = "(\\[.*\\])";
            Name = Regex.Replace(stationName.Name, regex, "");
        }        
    }
}
