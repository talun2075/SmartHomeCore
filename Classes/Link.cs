using System.Collections.Generic;

namespace SmartHome.Classes
{
    public class Link
    {
        public int Column { get; set; } = 1;
        public string Typ { get; set; }
        public List<LinkUri> Links { get; set; } = new();
        public string BildPfad { get; set; }
    }


    public class LinkUri
    {
        public string Uri { get; set; }
        public string Anzeige { get; set; }
        public string Bild { get; set; }
    }
}
