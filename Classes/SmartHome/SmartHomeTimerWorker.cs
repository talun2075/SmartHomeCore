using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using SmartHome.Classes.SmartHome.Data;
using SmartHome.Classes.SmartHome.Interfaces;
using SmartHome.Classes.SmartHome.Util;

namespace SmartHome.Classes.SmartHome
{
    public class SmartHomeTimerWorker : ISmartHomeTimerWorker
    {
        #region Klassenvariablen
        private static List<SmartHomeTimer> timers = new();
        private ISmartHomeHelper shm;
        public SmartHomeTimerWorker(ISmartHomeHelper _shm)
        {
            shm = _shm;
        }
        #endregion Klassenvariablen
        public async Task<bool> CheckTimer()
        {
            //SmartHomeConstants.log.TraceLog("CheckTimer", "Start");
            timers = ReadTimerXml();
            try
            {
                foreach (SmartHomeTimer st in timers)
                {
                    if (!st.Active) continue;
                    if (CheckTime(st))
                    {
                        if (st.Logging)
                            SmartHomeConstants.log.TraceLog("CheckTimer", st.Name);
                        await CallTimer(st);
                        continue;
                    }

                }
                //SmartHomeConstants.log.TraceLog("CheckTimer", "Ende");
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SonosTimerWorker:CheckTimer", ex);
                return false;
            }
        }
        /// <summary>
        /// Überprüft beim übergebenen Timer ob dieser gerade zu laufen hat.
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        private bool CheckTime(SmartHomeTimer st)
        {
            try
            {
                DateTime curtime = DateTime.Now;
                TimeSpan lastrunnedtime = new((curtime - st.LastRuntime).Ticks);
                TimeSpan havetorun;

                //Annahme, der Checktimer wird alle 5 Minuten aufgerufen. 
                if (st.Repeat)
                {
                    if (st.LastRuntime.Ticks == 0) return true;
                    int TimerMinutesForRepat = st.Time.Hours * 60 + st.Time.Minutes;
                    if (lastrunnedtime.TotalMinutes > TimerMinutesForRepat) return true;
                    return false; //immer true wegen repeat
                }
                else
                {
                    //if (st.LastRuntime.Ticks > 0) return false;//Ist schon gelaufen daher return
                    //Prüfen wann der Timer laufen soll
                    havetorun = st.Time - curtime.TimeOfDay;
                    if (havetorun.TotalMinutes < 5 && havetorun.TotalMinutes > -3) return true; //Zeit vergangen (aber nicht Länger als 3 Minuten) oder in etwa 5 Minuten daher laufen.
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SonosTimerWorker:CheckTime", ex);
            }


            return false;
        }
        /// <summary>
        /// Wrapper für den Timer um zu bestimmen, was verarbeitet werden soll.
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        private async Task<bool> CallTimer(SmartHomeTimer st)
        {

            st.LastRuntime = DateTime.Now;
            return await WebCall(st);

        }
        /// <summary>
        /// Einlesen der Timer XML. Vorhandene Einträge werden nicht überschrieben sondern aktualisiert.
        /// </summary>
        /// <returns></returns>
        private List<SmartHomeTimer> ReadTimerXml()
        {
            try
            {
                //SmartHomeConstants.log.TraceLog("ReadTimerXml", "Start");
                string path = SmartHomeConstants.Env.ContentRootPath + "\\Configuration\\Timer.xml";
                XmlDocument myXmlDocument = new();
                myXmlDocument.Load(path);
                //myXmlDocument.Load(mUrl + mXMLPath); //Load NOT LoadXml
                XmlNodeList timersconfig = myXmlDocument.SelectNodes("/Times/Time");
                foreach (XmlNode item in timersconfig)
                {
                    string[] argsu = Array.Empty<string>();
                    string ar = item.Attributes["Arguments"]?.Value;
                    if (!string.IsNullOrEmpty(ar))
                    {
                        if (ar.Contains("|||"))
                        {
                            argsu = ar.Split(new string[] { "|||" }, StringSplitOptions.None);
                        }
                        else
                        {
                            argsu[0] = ar;
                        }
                    }
                    string rtucs = item.Attributes["RequestTypeUrlCalls"]?.Value;
                    SmartHomeConstants.RequestEnums rtuc = SmartHomeConstants.RequestEnums.GET;
                    if (!string.IsNullOrEmpty(rtucs))
                    {
                        Enum.TryParse(rtucs, out rtuc);
                    }
                    SmartHomeTimer st = new()
                    {
                        Name = item.Attributes["Name"].Value,
                        Time = TimeSpan.Parse(item.Attributes["When"].Value),
                        Repeat = item.Attributes["Repeat"]?.Value == "true",
                        Method = item.Attributes["Method"]?.Value ?? string.Empty,
                        Path = item.Attributes["Path"]?.Value ?? string.Empty,
                        Async = item.Attributes["Async"]?.Value == null || item.Attributes["Async"]?.Value == "true",
                        Logging = item.Attributes["Logging"]?.Value == "true",
                        Active = item.Attributes["Active"]?.Value == "true",
                        Arguments = argsu,
                        RequestTypeUrlCalls = rtuc
                    };
                    
                    if (!timers.Any())
                    {
                        // SmartHomeConstants.log.TraceLog("ReadTimerXml", "Timer add:"+st.Name);
                        timers.Add(st);
                    }
                    else
                    {
                        SmartHomeTimer curtimer = timers.FirstOrDefault(x => x.Name == st.Name);
                        if (curtimer == null)
                        {
                            //  SmartHomeConstants.log.TraceLog("ReadTimerXml", "Timer add:" + st.Name);
                            timers.Add(st);
                        }
                        else
                        {
                            curtimer.Method = st.Method;
                            curtimer.Arguments = st.Arguments;
                            curtimer.Repeat = st.Repeat;
                            curtimer.Time = st.Time;
                            curtimer.Async = st.Async;
                            curtimer.RequestTypeUrlCalls = st.RequestTypeUrlCalls;
                            curtimer.Path = st.Path;
                            // SmartHomeConstants.log.TraceLog("ReadTimerXml", "Timer Update:" + st.Name);
                        }
                    }



                }
                //  SmartHomeConstants.log.TraceLog("ReadTimerXml", "Ende");
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SonosTimerWorker:ReadTimerXml", ex);
            }
            return timers;
        }

        private async Task<bool> WebCall(SmartHomeTimer st)
        {
            if (st.Logging)
                SmartHomeConstants.log.TraceLog("WebCall", "Start:" + st.Name);
            string value = "";
            if (string.IsNullOrEmpty(st.Path)) return false;
            if (st.Arguments.Length > 0)
            {
                value = st.Arguments[0];
            }
            try
            {
                string retval = "ok";
                if (st.Async)
                {
                    retval = await SmartHomeConstants.ConnectToWeb(st.RequestTypeUrlCalls, st.Path, value);
                }
                else
                {
                    _ = SmartHomeConstants.ConnectToWeb(st.RequestTypeUrlCalls, st.Path, value);
                }
                if (retval.Contains("ok") || retval.ToLower() == "true")
                {
                    if (st.Logging)
                        SmartHomeConstants.log.TraceLog("WebCall", "Ende:" + st.Name);
                    return true;
                }
                else
                {
                    if (st.Logging)
                        SmartHomeConstants.log.TraceLog("WebCall", "Ende mit Fehlern:" + st.Name + " Retval:" + retval);
                    SmartHomeConstants.log.ServerErrorsAdd("TimerWorker", new Exception("Timer:" + st.Name + " Path:" + st.Path + " Wert:" + retval), "WebCall");
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("WebCall", ex, st.Name);
                return false;
            }
            return false;
        }
    }
}