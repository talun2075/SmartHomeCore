using System;
using System.Linq;
using System.Data.SQLite;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Domain;
using SmartHome.Classes.SmartHome.Util;
using SmartHome.Classes.SmartHome.Data;
using SmartHome.Classes.Receipt;

namespace SmartHome.Classes.Database
{
    public class DatabaseWrapper : IDatabaseWrapper
    {
        private SQLiteConnection conbuttons;
        private SQLiteConnection conReceipt;
        private readonly string dbButtons = @"URI=file:E:\talun\buttonsbatteryLevel.db";
        private readonly string dbReceipt = @"URI=file:E:\talun\receipts.db";
        public DatabaseWrapper(IOptionsMonitor<DatabaseOptions> options)
        {
            if (!string.IsNullOrEmpty(options.CurrentValue.PathBatttery))
            {
                dbButtons = options.CurrentValue.PathBatttery;
            }
            conbuttons = new SQLiteConnection(dbButtons);
            if (!string.IsNullOrEmpty(options.CurrentValue.PathReceipt))
            {
                dbReceipt = options.CurrentValue.PathReceipt;
            }
            conReceipt = new SQLiteConnection(dbReceipt);
        }
        #region Receipt
        private async Task<bool> OpenReceipt()
        {
            try
            {
                if(conReceipt == null)
                {
                    conReceipt = new SQLiteConnection(dbReceipt);
                }
                if (conReceipt.State == System.Data.ConnectionState.Open) return true;
                await conReceipt.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.Open", ex);
                return false;
            }

        }
        private async void CloseReceipt()
        {
            if (conReceipt != null && conReceipt.State != System.Data.ConnectionState.Closed)
                await conReceipt.CloseAsync();

        }
        public async Task<ReceiptDTO> ReadReceiptData(int receiptid)
        {
            ReceiptDTO ret = new ReceiptDTO();
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "select * from talun_rezepte where rezept_id ="+receiptid
                };
                var rdr = await rcmd.ExecuteReaderAsync();
                
                while (rdr.Read())
                {
                    var id = rdr.GetInt32(0);
                    var title = rdr.GetString(1);
                    var decription = rdr.GetString(2);
                    var duration = rdr.GetString(4);
                    var resttime = rdr.GetString(5);
                    var resttimeunit = rdr.GetString(6);

                    ret.ID = id; 
                    ret.Title = title; 
                    ret.Duration = duration;
                    ret.Decription = decription;
                    ret.RestTime.RestTime = string.IsNullOrEmpty(resttime)? 0 : Convert.ToInt32(resttime);
                    ret.RestTime.Unit = string.IsNullOrEmpty(resttimeunit) ? 0 : Convert.ToInt32(resttimeunit);

                }
                //todo: fehlende Zutaten und kategorien zufügen.

                return ret;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadButtons", ex);
                return ret;
            }
            finally
            {
                CloseReceipt();
            }


        }
        #endregion Receipt
        #region Buttons
        private async Task<bool> OpenButton()
        {
            try
            {
                if(conbuttons == null)
                {
                    conbuttons = new SQLiteConnection(dbButtons);
                }
                if (conbuttons.State == System.Data.ConnectionState.Open) return true;
                await conbuttons.OpenAsync();
                var cmd = new SQLiteCommand(conbuttons)
                {
                    CommandText = "CREATE TABLE IF NOT EXISTS buttons (id text PRIMARY KEY,name text NOT NULL,battery integer default 100, lastaction text, lastclick text, ip text);"
                };
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.Open", ex);
                return false;
            }

        }
        private async void CloseButton()
        {
            if (conbuttons != null && conbuttons.State != System.Data.ConnectionState.Closed)
                await conbuttons.CloseAsync();

        }

        /// <summary>
        /// Aktualisiert ein Button in die DB
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="name"></param>
        /// <param name="batteryvalue"></param>
        /// <param name="lastaction"></param>
        /// <returns></returns>
        public async Task<bool> UpdateButton(Button button)
        {
            try
            {
                await OpenButton();
                var cmd = new SQLiteCommand(conbuttons)
                {
                    CommandText = "REPLACE INTO buttons(id,name, battery,lastaction, lastclick,ip) VALUES('" + button.Mac + "','" + button.Name + "'," + button.Batterie + ",'" + button.LastAction + "','" + DateTime.Now.ToString() + "','" + button.IP + "')"
                };

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateButton", ex);
                return false;
            }
            finally { CloseButton(); }
        }
        /// <summary>
        /// Liest alle Buttons aus der Datenbank, wenn diese in KnowingButtons vorhanden sind.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ReadButtons()
        {
            try
            {
                await OpenButton();
                var rcmd = new SQLiteCommand(conbuttons)
                {
                    CommandText = "Select * from buttons"
                };
                var rdr = await rcmd.ExecuteReaderAsync();
                var nameOrdinal = rdr.GetOrdinal("name");
                while (rdr.Read())
                {
                    var id = rdr.GetString(0);
                    var name = rdr.GetString(nameOrdinal);
                    var battery = rdr.GetInt32(2);
                    var last = rdr.GetString(3);
                    var date = rdr.GetString(4);
                    var ip = rdr.GetString(5);
                    Button b = SmartHomeConstants.KnowingButtons.FirstOrDefault(x => x.Mac == id);
                    if (b != null)
                    {
                        b.Batterie = battery;
                        if (DateTime.TryParse(date, out DateTime dtout))
                            b.LastClick = dtout;
                        if (Enum.TryParse(last, out ButtonAction action))
                        {
                            b.LastAction = action;
                        }
                        b.IP = ip;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadButtons", ex);
                return false;
            }
            finally
            {
                CloseButton();
            }
        }
        #endregion Buttons
    }
}