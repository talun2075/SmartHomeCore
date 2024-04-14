using System;
using System.Linq;
using System.Data.SQLite;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Domain;
using SmartHome.Classes.SmartHome.Util;
using SmartHome.Classes.SmartHome.Data;
using SmartHome.Classes.Receipt;
using System.Collections.Generic;

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
                if (conReceipt == null)
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

        public async Task<List<IngredientDTOBase>> ReadIngrediensData()
        {
            List<IngredientDTOBase> ret = new();
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "select * from talun_rezepte_zutaten order by zutat"
                };
                var rdr = await rcmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    IngredientDTOBase ing = new();
                    ing.ID = rdr.GetInt32(0);
                    ing.Ingredient = rdr.GetString(1);
                    ret.Add(ing);
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadIngrediensData",ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<List<CategoryDTO>> ReadCategoriesData()
        {
            List<CategoryDTO> ret = new();
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "select * from talun_rezepte_kategorie order by kategorie"
                };
                var rdr = await rcmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    CategoryDTO ing = new();
                    ing.ID = rdr.GetInt32(0);
                    ing.Category = rdr.GetString(1);
                    ret.Add(ing);
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadCategoriessData", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<List<ReceiptDTO>> ReadReceiptList()
        {
            List<ReceiptDTO> ret = new();
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "select * from talun_rezepte order by rezept_titel"
                };
                var rdr = await rcmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    ReceiptDTO receipt = new ReceiptDTO();
                    try
                    {
                        var resttimeunit = rdr.GetString(6);
                        receipt.ID = rdr.GetInt32(0);
                        receipt.Title = rdr.GetString(1);
                        receipt.Duration = rdr.GetString(4);
                        receipt.Decription = rdr.GetString(2);
                        receipt.RestTime.RestTime = rdr.GetString(5);
                        receipt.RestTime.Unit = string.IsNullOrEmpty(resttimeunit) ? 0 : Convert.ToInt32(resttimeunit);

                        var rkcmd = new SQLiteCommand(conReceipt)
                        {
                            CommandText = "SELECT zuei.menge,zu.zutat_id, zu.zutat, e.einheit,zuei.zuei_id FROM talun_rezepte_zuei as zuei left join talun_rezepte_zutaten as zu on zuei.zutat_id=zu.zutat_id LEFT JOIN talun_rezepte_einheit AS e ON zuei.einheit_id=e.einheit_id WHERE zuei.rezept_id=" + receipt.ID
                        };
                        var rkdr = await rkcmd.ExecuteReaderAsync();
                        List<IngredientPerReceipt> ingredients = new List<IngredientPerReceipt>();
                        while (rkdr.Read())
                        {
                            IngredientPerReceipt ing = new();
                            ing.Amount = rkdr.GetString(0);
                            ing.ID = rkdr.GetInt32(1);
                            ing.Ingredient = rkdr.GetString(2);
                            ing.Unit = rkdr.GetString(3);
                            ing.IngredientUnitID = rkdr.GetInt32(4);
                            ingredients.Add(ing);
                        }
                        receipt.Ingredients = ingredients;
                        var rkatcmd = new SQLiteCommand(conReceipt)
                        {
                            CommandText = "SELECT kr.kategorie_id, ka.kategorie FROM talun_rezepte_kare AS kr LEFT JOIN talun_rezepte_kategorie AS ka ON kr.kategorie_id=ka.kategorie_id WHERE kr.rezept_id=" + receipt.ID
                        };
                        var rkatdr = await rkatcmd.ExecuteReaderAsync();
                        List<CategoryDTO> categories = new();
                        while (rkatdr.Read())
                        {
                            CategoryDTO cat = new();
                            cat.ID = rkatdr.GetInt32(0);
                            if (cat.ID > 0)
                                cat.Category = rkatdr.GetString(1);

                            categories.Add(cat);
                        }
                        receipt.Categories = categories;

                        var rbatcmd = new SQLiteCommand(conReceipt)
                        {
                            CommandText = "SELECT * FROM talun_rezepte_bilder WHERE rezept_id='" + receipt.ID + "' order by bild_ordnen"
                        };
                        var rbatdr = await rbatcmd.ExecuteReaderAsync();
                        List<Picture> pictures = new();
                        while (rbatdr.Read())
                        {
                            try
                            {
                                Picture pic = new();
                                pic.ID = rbatdr.GetInt32(0);
                                pic.Image = rbatdr.GetString(2);
                                pic.SortOrder = rbatdr.GetString(3);
                                pictures.Add(pic);
                            }
                            catch (Exception ex)
                            {
                                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadReceiptList:Pictures:RezeptID:" + receipt.ID, ex);
                            }
                        }
                        receipt.Pictures = pictures;

                        ret.Add(receipt);
                    }
                    catch (Exception ex)
                    {
                        SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadReceiptList:RezeptID:" + receipt.ID, ex);
                    }
                }//While Ende

                return ret;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadReceiptList", ex);
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
                if (conbuttons == null)
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