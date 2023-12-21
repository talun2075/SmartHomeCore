using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartHome.Classes;
using SmartHome.Classes.SmartHome.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SmartHome.Controllers
{
    public class LinksController : Controller
    {
        private static string envpath;
        private static List<Link> linklist = new();

        public LinksController(IWebHostEnvironment env)
        {
            envpath = env.ContentRootPath;
        }
        public IActionResult Index()
        {
            Read();
            ViewBag.Links = linklist;
            ViewBag.Columns = linklist.Last().Column;
            ViewBag.Title = "Link Übersicht";
            ViewBag.png = "favicon.png";
            ViewBag.svg = "home.svg";
            ViewBag.png16 = "favicon.png";
            ViewBag.png32 = "favicon.png";
            ViewBag.NavClass = "navSixt";
            return View();
        }



        private bool Read()
        {
            string path = envpath + "\\Configuration\\Links.xml";
            XmlDocument myXmlDocument = new();
            myXmlDocument.Load(path);
            //myXmlDocument.Load(mUrl + mXMLPath); //Load NOT LoadXml
            XmlNodeList linkconfig = myXmlDocument.SelectNodes("/links/link");
            if (linkconfig.Count == linklist.Count) return true;
            foreach (XmlNode item in linkconfig)
            {
                try
                {
                    Link l = new();
                    l.BildPfad = item.Attributes["BildPfad"]?.Value ?? string.Empty;
                    l.Typ = item.Attributes["Typ"].Value;
                    var col = item.Attributes["Column"]?.Value;
                    int colvalue;
                    if (int.TryParse(col, out colvalue))
                    {
                        l.Column = colvalue;
                    }
                    if (item.HasChildNodes)
                    {
                        foreach (XmlNode child in item)
                        {
                            try
                            {
                                LinkUri lu = new();
                                lu.Anzeige = child.Attributes["Anzeige"].Value;
                                lu.Uri = child.Attributes["Uri"].Value;
                                lu.Bild = child.Attributes["Bild"]?.Value != null ? child.Attributes["Bild"].Value : string.Empty;
                                l.Links.Add(lu);
                            }
                            catch (Exception ex)
                            {
                                SmartHomeConstants.log.ServerErrorsAdd("Links", ex, "Read->Child");
                                continue;
                            }

                        }
                    }
                    linklist.Add(l);
                }
                catch (Exception ex)
                {
                    SmartHomeConstants.log.ServerErrorsAdd("Links", ex, "Read");
                    continue;
                }
            }
            linklist = linklist.OrderBy(o => o.Column).ToList();
            return true;
        }

    }
}
