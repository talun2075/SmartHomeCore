using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using SmartHome.Classes.Shelly.Data;
using SmartHome.Classes.SmartHome.Util;

namespace SmartHome.Classes.Shelly
{
    public class ShellyWorker : IShellyWorker
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IncludeFields = true };
        private readonly Dictionary<string, string> _listofShellyUrls = new();
        private readonly List<Shelly1> _listOfShellys = new();
        public async Task<List<Shelly1>> Read()
        {
            try
            {
                if (!ReadShellyXML()) return _listOfShellys;
                foreach (string item in _listofShellyUrls.Keys)
                {
                    string retv = string.Empty;
                    try
                    {
                        retv = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, "http://" + item + "/settings");
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
                            s.Description = _listofShellyUrls[item];
                            if (_listOfShellys.Count > 0)
                            {
                                var shelly = _listOfShellys.FirstOrDefault(x => x.Name == s.Name);
                                if (shelly == null)
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
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("ShellyWorker:Read:", ex);
            }
            SmartHomeConstants.Shelly1 = _listOfShellys;
            return _listOfShellys;
        }


        public async Task<bool> Switch()
        {
            await Task.Delay(100);
            return true;
        }
        public async Task<bool> PowerGuestRoom(bool PowerOn = false)
        {
            List<string> shellys = new() { "gastrechts.shelly.tami", "gastlinks.shelly.tami" };
            return await PowerRooms(shellys, PowerOn);
        }
        public async Task<bool> PowerGuestRoomRight(bool PowerOn = false)
        {
            List<string> shellys = new() { "gastrechts.shelly.tami" };
            return await PowerRooms(shellys, PowerOn);
        }
        public async Task<bool> PowerGuestRoomLeft(bool PowerOn = false)
        {
            List<string> shellys = new() { "gastlinks.shelly.tami" };
            return await PowerRooms(shellys, PowerOn);
        }
        public async Task<bool> PowerRoom(string room, bool PowerOn = false)
        {
            List<string> shellys = new()
            {
                room
            };
            return await PowerRooms(shellys, PowerOn);
        }
        public async Task<bool> PowerRooms(List<string> shellys, bool PowerOn = false)
        {
            try
            {
                if (!SmartHomeConstants.Shelly1.Any())
                    await Read();

                foreach (string item in shellys)
                {
                    Shelly1 shelly = SmartHomeConstants.Shelly1.FirstOrDefault(x => x.Name.ToLower() == item.ToLower());
                    if (shelly == null) return false;
                    if (shelly.Relays.First().IsOn == PowerOn) return true;
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

        private bool ReadShellyXML()
        {
            try
            {
                //SmartHomeConstants.log.TraceLog("ReadButtonXML", "Start");
                string path = SmartHomeConstants.Env.ContentRootPath + "\\Configuration\\Shellys.xml";
                XmlDocument myXmlDocument = new();
                myXmlDocument.Load(path);
                //myXmlDocument.Load(mUrl + mXMLPath); //Load NOT LoadXml
                XmlNodeList buttonsconfig = myXmlDocument.SelectNodes("/Shellys/Shelly");
                if (buttonsconfig.Count == _listofShellyUrls.Count) return true;
                foreach (XmlNode item in buttonsconfig)
                {
                    var url = item.Attributes["URL"].Value;
                    var des = item.Attributes["Description"].Value;
                    if (!string.IsNullOrEmpty(url.ToString()) && !_listofShellyUrls.ContainsKey(url))
                    {
                        _listofShellyUrls.Add(url, des);
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