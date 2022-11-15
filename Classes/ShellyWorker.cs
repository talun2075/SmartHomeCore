//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace SmartHome.Classes
{
    public class ShellyWorker
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new ()
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IncludeFields= true};
        private static readonly List<String> _listofShellyUrls = new ();
        private static readonly List<Shelly1> _listOfShellys = new ();
        public static async Task<List<Shelly1>> Read()
        {
            try
            {
                if (!ReadShellyXML()) return _listOfShellys;
                foreach (String item in _listofShellyUrls)
                {
                    String retv = String.Empty;
                    try
                    {
                        retv = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET,"http://"+item+"/settings");
                    }
                    catch (Exception ex)
                    {
                        SmartHomeConstants.log.ServerErrorsAdd("ShellyWorker:Read:ConnectToWeb:", ex);
                    }
                    try
                    {
                        //Shelly1 s = JsonConvert.DeserializeObject<Shelly1>(retv);
                        var s = JsonSerializer.Deserialize<Shelly1>(retv, _jsonSerializerOptions);
                        if (s != null)
                        {
                            if(_listOfShellys.Count > 0)
                            {
                                var shelly = _listOfShellys.FirstOrDefault(x => x.Name == s.Name);
                                if(shelly == null)
                                {
                                    _listOfShellys.Add(s);
                                }
                                else
                                {
                                    var ind = _listOfShellys.IndexOf(shelly);
                                    _listOfShellys[ind] = s;
                                }
                            }
                            else
                            {
                                _listOfShellys.Add(s);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SmartHomeConstants.log.ServerErrorsAdd("ShellyWorker:Read:Convert:", ex);
                    }
                    
                }
                return _listOfShellys;
            }
            catch(Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("ShellyWorker:Read:", ex);
            }
            return _listOfShellys;
        }


        public static async Task<Boolean> Switch()
        {
            await Task.Delay(100);
            return true;
        }

        public static async Task<Boolean> PowerGuestRoom(Boolean PowerOn = false)
        {
            try
            {
                if (!SmartHomeConstants.Shelly1.Any()) await Read();
                List<String> shellys = new() { "gastrechts.shelly.tami", "gastlinks.shelly.tami" };
                foreach (String item in shellys)
                {
                    Shelly1 shelly = SmartHomeConstants.Shelly1.FirstOrDefault(x => x.Name.ToLower() == item.ToLower());
                    shelly.Relays.First().IsOn = PowerOn;

                    string url = "http://" + item + "/relay/0?turn=";
                    if (PowerOn)
                    {
                        url += "on";
                    }
                    else
                    {
                        url += "off";
                    }
                    await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, url);
                }

                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("ShellyWorker", ex, "PowerGuestRoom");
                return false;
            }
        }

         private static Boolean ReadShellyXML()
        {
            try
            {
                //SmartHomeConstants.log.TraceLog("ReadButtonXML", "Start");
                string path = SmartHomeConstants.Env.ContentRootPath + "\\Configuration\\Shellys.xml";
                XmlDocument myXmlDocument = new ();
                myXmlDocument.Load(path);
                //myXmlDocument.Load(mUrl + mXMLPath); //Load NOT LoadXml
                XmlNodeList buttonsconfig = myXmlDocument.SelectNodes("/Shellys/Shelly");
                if (buttonsconfig.Count == _listofShellyUrls.Count) return true;
                foreach (XmlNode item in buttonsconfig)
                {
                    var v = item.Attributes["URL"].Value;
                    if (!string.IsNullOrEmpty(v.ToString()) && !_listofShellyUrls.Contains(v))
                    {
                        _listofShellyUrls.Add(v);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("ShellyWorker:ReadShellyXML:", ex);
                return false;
            }
        }
    }
}