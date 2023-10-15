using System;
using System.Linq;
using System.Data.SQLite;
using System.Threading.Tasks;
using SmartHome.DataClasses;

namespace SmartHome.Classes
{
    public static class DatabaseWrapper
    {
        private static SQLiteConnection conn;
        private static SQLiteCommand cmd;
        private static SQLiteCommand rcmd;
        private static readonly string cs =@"URI=file:E:\talun\buttonsbatteryLevel.db";//todo: move to configuration
        public static async Task<Boolean> Open()
        {
            try
            {
                conn = new SQLiteConnection(cs);
                await conn.OpenAsync();
                cmd = new SQLiteCommand(conn)
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

        public static void Close()
        {
            if (conn != null || conn.State != System.Data.ConnectionState.Closed)
                conn.Close();
                
        }
        ///// <summary>
        ///// Update Batterie Wert
        ///// </summary>
        ///// <param name="mac"></param>
        ///// <param name="batteryvalue"></param>
        ///// <returns></returns>
        //public static async Task<Boolean> UpdateBattery(Button button)
        //{
        //    try
        //    {
        //        if (cmd == null || conn == null || conn.State != System.Data.ConnectionState.Open) await Open();
        //        cmd.CommandText = "Update buttons set battery=" + button.Batterie + " where id ='" + button.Mac + "'";
        //        await cmd.ExecuteNonQueryAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateButton", ex);
        //        return false;
        //    }
        //}
        /// <summary>
        /// Aktualisiert ein Button in die DB
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="name"></param>
        /// <param name="batteryvalue"></param>
        /// <param name="lastaction"></param>
        /// <returns></returns>
        public static async Task<Boolean> UpdateButton(Button button)
        {
            try
            {
                if (cmd == null || conn == null || conn.State != System.Data.ConnectionState.Open) await Open();
                cmd.CommandText = "REPLACE INTO buttons(id,name, battery,lastaction, lastclick,ip) VALUES('" + button.Mac + "','" + button.Name + "'," + button.Batterie + ",'" + button.LastAction + "','" + DateTime.Now.ToString() + "','" + button.IP + "')";
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch(Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateButton", ex);
                return false;
            }
        }
        /// <summary>
        /// Liest alle Buttons aus der Datenbank, wenn diese in KnowingButtons vorhanden sind.
        /// </summary>
        /// <returns></returns>
        public static async Task<Boolean> ReadButtons()
        {
            try
            {
                if (cmd == null) await Open();
                rcmd = new SQLiteCommand(conn)
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
                        if(DateTime.TryParse(date, out DateTime dtout))
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
        }

    }
}