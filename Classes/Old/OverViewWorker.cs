using System.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.Classes.SmartHome.Util;

namespace SmartHome.Classes.Old
{
    public static class OverViewWorker
    {

        public static bool ReadOverViewConfig()
        {
            try
            {
                if (OverViews.Any()) return true;
                string path = SmartHomeConstants.Env.ContentRootPath + "\\Configuration\\OverViewConfig.xml";
                XmlDocument myXmlDocument = new();
                myXmlDocument.Load(path);
                //myXmlDocument.Load(mUrl + mXMLPath); //Load NOT LoadXml
                XmlNodeList rooms = myXmlDocument.SelectNodes("OverView/Room");
                foreach (XmlNode room in rooms)
                {
                    OverviewRoom overviewRoom = new();
                    var v = room.Attributes["Name"].Value;
                    if (string.IsNullOrEmpty(v)) continue;
                    overviewRoom.Room = v;
                    foreach (XmlNode roomChild in room.ChildNodes)
                    {
                        OverView ov = new();
                        ov.Name = roomChild.Attributes["Name"].Value;
                        ov.GetCurrentValue = roomChild.Attributes["GetCurrentValue"].Value.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();

                        ov.Off = roomChild.Attributes["Off"].Value.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();
                        ov.On = roomChild.Attributes["On"].Value.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();

                        if (ov.IsNotEmpty)
                            overviewRoom.Controllers.Add(ov);
                    }
                    OverViews.Add(overviewRoom);
                }
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("ShellyWorker:ReadShellyXML:", ex);
                return false;
            }
        }
        public static List<OverviewRoom> OverViews { get; set; } = new List<OverviewRoom>();
    }
}
