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
        public async Task<Boolean> AddCategory(string categoryName)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Insert INTO talun_rezepte_kategorie(kategorie) VALUES('" + categoryName + "')"
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.AddCategory", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<long> AddReceipt(string receiptName)
        {
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Insert INTO talun_rezepte(rezept_titel) VALUES('" + receiptName + "')"
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                return conReceipt.LastInsertRowId;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.AddCategory", ex);
                return -1;
            }
            finally
            {
                CloseReceipt();
            }
        }
        public async Task<Boolean> AddIngredient(string ingridient)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Insert INTO talun_rezepte_zutaten(zutat) VALUES('" + ingridient + "')"
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.AddIngredient", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<Boolean> AddUnit(string unit)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Insert INTO talun_rezepte_einheit(einheit) VALUES('" + unit + "')"
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.AddUnit", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<Boolean> UpdateCategory(CategoryDTO category)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Update talun_rezepte_kategorie set kategorie = '" + category.Category + "' where kategorie_id =" + category.ID
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateCategory", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<Boolean> UpdateIngredient(IngredientDTOBase ingredient)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Update talun_rezepte_zutaten set zutat = '" + ingredient.Ingredient + "' where zutat_id =" + ingredient.ID
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateIngredient", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<Boolean> UpdateUnit(int id, string unit)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Update talun_rezepte_einheit set einheit = '" + unit + "' where einheit_id =" + id
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateUnit", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<List<IngredientDTOBase>> ReadIngrediensData()
        {
            List<IngredientDTOBase> ret = new();
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "select * from talun_rezepte_zutaten order by zutat COLLATE NOCASE ASC"
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
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadIngrediensData", ex);
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
                    CommandText = "select * from talun_rezepte_kategorie order by kategorie COLLATE NOCASE ASC"
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
        public async Task<List<UnitDTO>> ReadUnitsData()
        {
            List<UnitDTO> ret = new();
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "select * from talun_rezepte_einheit order by einheit COLLATE NOCASE ASC"
                };
                var rdr = await rcmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    UnitDTO ing = new();
                    ing.ID = rdr.GetInt32(0);
                    ing.Unit = rdr.GetString(1);
                    ret.Add(ing);
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadUnitData", ex);
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
                    CommandText = "select * from talun_rezepte order by rezept_titel COLLATE NOCASE ASC"
                };
                var rdr = await rcmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    ReceiptDTO receipt = new ReceiptDTO();
                    try
                    {
                        try
                        {
                            receipt.ID = rdr.GetInt32(0);
                            receipt.Title = rdr.GetString(1);
                            receipt.Duration = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
                            receipt.Decription = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                            receipt.RestTime.RestTime = rdr.IsDBNull(5) ? "" : rdr.GetString(5);
                            receipt.RestTime.Unit = rdr.IsDBNull(6) ? "" : rdr.GetString(6);
                        }
                        catch (Exception ex)
                        {
                            SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadReceiptList:receipt:RezeptID:" + receipt.ID, ex);
                        }

                        var rkcmd = new SQLiteCommand(conReceipt)
                        {
                            CommandText = "SELECT zuei.menge,zu.zutat_id, zu.zutat, e.einheit,zuei.zuei_id FROM talun_rezepte_zuei as zuei left join talun_rezepte_zutaten as zu on zuei.zutat_id=zu.zutat_id LEFT JOIN talun_rezepte_einheit AS e ON zuei.einheit_id=e.einheit_id WHERE zuei.rezept_id=" + receipt.ID
                        };
                        var rkdr = await rkcmd.ExecuteReaderAsync();
                        List<IngredientPerReceipt> ingredients = new List<IngredientPerReceipt>();
                        while (rkdr.Read())
                        {
                            IngredientPerReceipt ing = new();
                            try
                            {
                                
                                ing.Amount = rkdr.GetString(0);
                                ing.ID = rkdr.GetInt32(1);
                                ing.Ingredient = rkdr.GetString(2);
                                ing.Unit = rkdr.GetString(3);
                                ing.IngredientUnitID = rkdr.GetInt32(4);
                                ingredients.Add(ing);
                            }
                            catch (Exception ex)
                            {
                                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadReceiptList:ingridiens:RezeptID:" + receipt.ID, ex);
                            }
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
                            try
                            {
                                CategoryDTO cat = new();
                                cat.ID = rkatdr.GetInt32(0);
                                if (cat.ID > 0)
                                    cat.Category = rkatdr.GetString(1);

                                categories.Add(cat);
                            }
                            catch (Exception ex)
                            {
                                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.ReadReceiptList:categories:RezeptID:" + receipt.ID, ex);
                            }
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
        public async Task<Boolean> UpdateReceipt(long receiptId, ReceiptUpdateDTO ru)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                string field = String.Empty;
                string val=String.Empty;
                switch (ru.Type)
                {
                    case ReceiptUpdateType.Title:
                        field = "rezept_titel";
                        val = ru.Value;
                        break;
                    case ReceiptUpdateType.Description:
                        field = "rezept_besch";
                        val = ru.Value;
                        break;
                    case ReceiptUpdateType.Duration:
                        field = "rezept_dauer";
                        val = ru.Value;
                        break;
                    case ReceiptUpdateType.RestTime:
                        field = "rezept_ruhezeit";
                        val = ru.Value;
                        break;
                    case ReceiptUpdateType.ResTimeUnit:
                        field = "rezept_ruhezeit_d";
                        val = ru.Value;
                        break;

                }
                if(string.IsNullOrEmpty(field) || string.IsNullOrEmpty(val)) return false;

                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Update talun_rezepte set "+field+" = '" + val + "' where rezept_id =" + receiptId
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateUnit", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<Boolean> UpdateReceiptCategoryAdd(long receiptId, ReceiptUpdateDTO ru)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "insert into talun_rezepte_kare(rezept_id,kategorie_id) VALUES (" + receiptId + "," + ru.UnitID + ")"
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptCategoryAdd", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<Boolean> UpdateReceiptCategoryDelete(long receiptId, ReceiptUpdateDTO ru)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Delete from talun_rezepte_kare where rezept_id="+receiptId+" AND kategorie_id =" + ru.UnitID
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptCategoryDelete", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<long> UpdateReceiptIngridientUnitAdd(long receiptId, ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "insert into talun_rezepte_zuei(rezept_id,einheit_id,zutat_id,menge) VALUES (" + receiptId + "," + ru.UnitID + "," + ru.UnitID2 + ",'" + ru.Value + "')"
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                return conReceipt.LastInsertRowId;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptCategoryAdd", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return 0;
        }
        public async Task<Boolean> UpdateReceiptIngridientUnitUpdate(ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Update talun_rezepte_zuei set menge ='"+ru.Value+"' where zuei_id="+ru.UnitID
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptCategoryAdd", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return false;
        }
        public async Task<Boolean> UpdateReceiptIngridientUnitDelete(ReceiptUpdateDTO ru)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Delete from talun_rezepte_zuei where zuei_id=" + ru.UnitID
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptCategoryDelete", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
        }
        public async Task<Boolean> UpdateReceiptImageSortOrder(ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Update talun_rezepte_bilder set bild_ordnen ='" + ru.Value + "' where bild_id=" + ru.UnitID
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptImageSortOrder", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return false;
        }
        public async Task<Boolean> UpdateReceiptImageDelete(ReceiptUpdateDTO ru)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                var rcmd = new SQLiteCommand(conReceipt)
                {
                    CommandText = "Delete from talun_rezepte_bilder where bild_id=" + ru.UnitID
                };
                _ = await rcmd.ExecuteNonQueryAsync();
                ret = true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptCategoryDelete", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return ret;
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