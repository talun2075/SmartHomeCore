using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Domain;
using SmartHome.Classes.SmartHome.Util;
using SmartHome.Classes.Receipt;
using System.Collections.Generic;

namespace SmartHome.Classes.Database
{
    public class DatabaseWrapper : IDatabaseWrapper
    {
        private SqliteConnection conReceipt;
        private readonly string dbReceipt = @"Data Source=E:\talun\receipts.db";

        public DatabaseWrapper(IOptionsMonitor<DatabaseOptions> options)
        {
            if (!string.IsNullOrEmpty(options.CurrentValue.PathReceipt))
            {
                dbReceipt = $"Data Source={options.CurrentValue.PathReceipt}";
            }
            conReceipt = new SqliteConnection(dbReceipt);
        }

        #region private
        private async Task<bool> OpenReceipt()
        {
            try
            {
                if (conReceipt == null)
                {
                    conReceipt = new SqliteConnection(dbReceipt);
                }
                if (conReceipt.State == System.Data.ConnectionState.Open)
                    return true;
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

        private async Task<List<IngredientPerReceipt>> GetIngredientsForReceipt(long receiptId)
        {
            List<IngredientPerReceipt> ingredients = new();
            using var rkcmd = conReceipt.CreateCommand();
            rkcmd.CommandText = @"SELECT zuei.menge, zu.zutat_id, zu.zutat, e.einheit, zuei.zuei_id 
                                  FROM talun_rezepte_zuei AS zuei 
                                  LEFT JOIN talun_rezepte_zutaten AS zu ON zuei.zutat_id = zu.zutat_id 
                                  LEFT JOIN talun_rezepte_einheit AS e ON zuei.einheit_id = e.einheit_id 
                                  WHERE zuei.rezept_id = @receiptId";
            rkcmd.Parameters.AddWithValue("@receiptId", receiptId);
            using var rkdr = await rkcmd.ExecuteReaderAsync();
            while (rkdr.Read())
            {
                try
                {
                    IngredientPerReceipt ing = new()
                    {
                        Amount = rkdr.GetString(0),
                        ID = rkdr.GetInt64(1),
                        Ingredient = rkdr.GetString(2),
                        Unit = rkdr.GetString(3),
                        IngredientUnitID = rkdr.GetInt32(4)
                    };
                    ingredients.Add(ing);
                }
                catch (Exception ex)
                {
                    SmartHomeConstants.log.ServerErrorsAdd($"databaseWrapper.GetIngredientsForReceipt:RezeptID:{receiptId}", ex);
                }
            }
            return ingredients;
        }

        private async Task<List<CategoryDTO>> GetCategoriesForReceipt(long receiptId)
        {
            List<CategoryDTO> categories = new();
            using var rkatcmd = conReceipt.CreateCommand();
            rkatcmd.CommandText = @"SELECT kr.kategorie_id, ka.kategorie 
                                    FROM talun_rezepte_kare AS kr 
                                    LEFT JOIN talun_rezepte_kategorie AS ka ON kr.kategorie_id = ka.kategorie_id 
                                    WHERE kr.rezept_id = @receiptId";
            rkatcmd.Parameters.AddWithValue("@receiptId", receiptId);
            using var rkatdr = await rkatcmd.ExecuteReaderAsync();
            while (rkatdr.Read())
            {
                try
                {
                    CategoryDTO cat = new()
                    {
                        ID = rkatdr.GetInt64(0),
                        Category = rkatdr.GetInt64(0) > 0 ? rkatdr.GetString(1) : null
                    };
                    categories.Add(cat);
                }
                catch (Exception ex)
                {
                    SmartHomeConstants.log.ServerErrorsAdd($"databaseWrapper.GetCategoriesForReceipt:RezeptID:{receiptId}", ex);
                }
            }
            return categories;
        }

        private async Task<List<Picture>> GetPicturesForReceipt(long receiptId)
        {
            List<Picture> pictures = new();
            using var rbatcmd = conReceipt.CreateCommand();
            rbatcmd.CommandText = "SELECT * FROM talun_rezepte_bilder WHERE rezept_id = @receiptId ORDER BY bild_ordnen";
            rbatcmd.Parameters.AddWithValue("@receiptId", receiptId);
            using var rbatdr = await rbatcmd.ExecuteReaderAsync();
            while (await rbatdr.ReadAsync())
            {
                try
                {
                    Picture pic = new()
                    {
                        ID = rbatdr.GetInt64(0),
                        Image = rbatdr.GetString(2),
                        SortOrder = rbatdr.GetString(3),
                        ReceiptID = receiptId
                    };
                    pictures.Add(pic);
                }
                catch (Exception ex)
                {
                    SmartHomeConstants.log.ServerErrorsAdd($"databaseWrapper.GetPicturesForReceipt:RezeptID:{receiptId}", ex);
                }
            }
            return pictures;
        }

        #endregion private


        public async Task<bool> AddCategory(string categoryName)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "INSERT INTO talun_rezepte_kategorie(kategorie) VALUES(@categoryName)";
                rcmd.Parameters.AddWithValue("@categoryName", categoryName);
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

        public async Task<bool> AddIngredient(string ingredient)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "INSERT INTO talun_rezepte_zutaten(zutat) VALUES(@ingredient)";
                rcmd.Parameters.AddWithValue("@ingredient", ingredient);
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

        public async Task<long> AddReceipt(string receiptName)
        {
            long newId = -1;
            try
            {
                await OpenReceipt();
                using var transaction = conReceipt.BeginTransaction();
                using var insertCmd = conReceipt.CreateCommand();
                insertCmd.CommandText = "INSERT INTO talun_rezepte(rezept_titel) VALUES(@receiptName)";
                insertCmd.Parameters.AddWithValue("@receiptName", receiptName);
                await insertCmd.ExecuteNonQueryAsync();

                using var selectCmd = conReceipt.CreateCommand();
                selectCmd.CommandText = "SELECT last_insert_rowid()";
                newId = (long)await selectCmd.ExecuteScalarAsync();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.AddReceipt", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return newId;
        }


        public async Task<bool> AddUnit(string unit)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "INSERT INTO talun_rezepte_einheit(einheit) VALUES(@unit)";
                rcmd.Parameters.AddWithValue("@unit", unit);
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

        public async Task<Picture> PictureAdd(Picture pic)
        {
            try
            {
                await OpenReceipt();
                using var command = conReceipt.CreateCommand();
                command.CommandText = "INSERT INTO talun_rezepte_bilder(rezept_id, rezept_bild, bild_ordnen) VALUES (@receiptId, @image, '1')";
                command.Parameters.AddWithValue("@receiptId", pic.ReceiptID);
                command.Parameters.AddWithValue("@image", pic.Image);
                await command.ExecuteNonQueryAsync();

                using var selectCommand = conReceipt.CreateCommand();
                selectCommand.CommandText = "SELECT last_insert_rowid()";
                pic.ID = (long)await selectCommand.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.PictureAdd", ex);
            }
            finally
            {
                CloseReceipt();
            }
            return pic;
        }


        public async Task<List<CategoryDTO>> ReadCategoriesData()
        {
            List<CategoryDTO> ret = new();
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "SELECT * FROM talun_rezepte_kategorie ORDER BY kategorie";
                using var rdr = await rcmd.ExecuteReaderAsync();
                while (rdr.Read())
                {
                    CategoryDTO ing = new()
                    {
                        ID = rdr.GetInt32(0),
                        Category = rdr.GetString(1)
                    };
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
            ret = ret.OrderBy(x => x.Category).ToList();
            return ret;
        }

        public async Task<List<IngredientDTOBase>> ReadIngrediensData()
        {
            List<IngredientDTOBase> ret = new();
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "SELECT * FROM talun_rezepte_zutaten ORDER BY zutat";
                using var rdr = await rcmd.ExecuteReaderAsync();
                while (rdr.Read())
                {
                    IngredientDTOBase ing = new()
                    {
                        ID = rdr.GetInt32(0),
                        Ingredient = rdr.GetString(1)
                    };
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
            ret = ret.OrderBy(x => x.Ingredient).ToList();
            return ret;
        }

        public async Task<List<ReceiptDTO>> ReadReceiptList()
        {
            List<ReceiptDTO> ret = new();
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "SELECT * FROM talun_rezepte ORDER BY rezept_titel COLLATE NOCASE ASC";
                using var rdr = await rcmd.ExecuteReaderAsync();
                while (rdr.Read())
                {
                    ReceiptDTO receipt = new();
                    try
                    {
                        receipt.ID = rdr.GetInt64(0);
                        receipt.Title = rdr.GetString(1);
                        receipt.Duration = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
                        receipt.Decription = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                        receipt.RestTime.RestTime = rdr.IsDBNull(5) ? "" : rdr.GetString(5);
                        receipt.RestTime.Unit = rdr.IsDBNull(6) ? "" : rdr.GetString(6);

                        receipt.Ingredients = await GetIngredientsForReceipt(receipt.ID);
                        receipt.Categories = await GetCategoriesForReceipt(receipt.ID);
                        receipt.Pictures = await GetPicturesForReceipt(receipt.ID);

                        ret.Add(receipt);
                    }
                    catch (Exception ex)
                    {
                        SmartHomeConstants.log.ServerErrorsAdd($"databaseWrapper.ReadReceiptList:RezeptID:{receipt.ID}", ex);
                    }
                }
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

        public async Task<List<UnitDTO>> ReadUnitsData()
        {
            List<UnitDTO> ret = new();
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "SELECT * FROM talun_rezepte_einheit ORDER BY einheit";
                using var rdr = await rcmd.ExecuteReaderAsync();
                while (rdr.Read())
                {
                    UnitDTO ing = new()
                    {
                        ID = rdr.GetInt32(0),
                        Unit = rdr.GetString(1)
                    };
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
            ret = ret.OrderBy(x => x.Unit).ToList();
            return ret;
        }

        public async Task<bool> UpdateCategory(CategoryDTO category)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "UPDATE talun_rezepte_kategorie SET kategorie = @category WHERE kategorie_id = @id";
                rcmd.Parameters.AddWithValue("@category", category.Category);
                rcmd.Parameters.AddWithValue("@id", category.ID);
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

        public async Task<bool> UpdateIngredient(IngredientDTOBase ingredient)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "UPDATE talun_rezepte_zutaten SET zutat = @ingredient WHERE zutat_id = @id";
                rcmd.Parameters.AddWithValue("@ingredient", ingredient.Ingredient);
                rcmd.Parameters.AddWithValue("@id", ingredient.ID);
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

        public async Task<Boolean> UpdateReceipt(long receiptId, ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                using var command = conReceipt.CreateCommand();

                string field = ru.Type switch
                {
                    ReceiptUpdateType.Title => "rezept_titel",
                    ReceiptUpdateType.Description => "rezept_besch",
                    ReceiptUpdateType.Duration => "rezept_dauer",
                    ReceiptUpdateType.RestTime => "rezept_ruhezeit",
                    ReceiptUpdateType.ResTimeUnit => "rezept_ruhezeit_d",
                    _ => string.Empty
                };

                if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(ru.Value))
                    return false;

                command.CommandText = $"UPDATE talun_rezepte SET {field} = @value WHERE rezept_id = @receiptId";
                command.Parameters.AddWithValue("@value", ru.Value);
                command.Parameters.AddWithValue("@receiptId", receiptId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceipt", ex);
                return false;
            }
            finally
            {
                CloseReceipt();
            }
        }


        public async Task<bool> UpdateReceiptCategoryAdd(long receiptId, ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                using var command = conReceipt.CreateCommand();
                command.CommandText = "INSERT INTO talun_rezepte_kare(rezept_id, kategorie_id) VALUES (@receiptId, @categoryId)";
                command.Parameters.AddWithValue("@receiptId", receiptId);
                command.Parameters.AddWithValue("@categoryId", ru.UnitID);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptCategoryAdd", ex);
                return false;
            }
            finally
            {
                CloseReceipt();
            }
        }


        public async Task<bool> UpdateReceiptCategoryDelete(long receiptId, ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                using var command = conReceipt.CreateCommand();
                command.CommandText = "DELETE FROM talun_rezepte_kare WHERE rezept_id = @receiptId AND kategorie_id = @categoryId";
                command.Parameters.AddWithValue("@receiptId", receiptId);
                command.Parameters.AddWithValue("@categoryId", ru.UnitID);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptCategoryDelete", ex);
                return false;
            }
            finally
            {
                CloseReceipt();
            }
        }


        public async Task<bool> UpdateReceiptImageDelete(ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                using var command = conReceipt.CreateCommand();
                command.CommandText = "DELETE FROM talun_rezepte_bilder WHERE bild_id = @imageId";
                command.Parameters.AddWithValue("@imageId", ru.UnitID);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptImageDelete", ex);
                return false;
            }
            finally
            {
                CloseReceipt();
            }
        }


        public async Task<bool> UpdateReceiptImageSortOrder(ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                using var command = conReceipt.CreateCommand();
                command.CommandText = "UPDATE talun_rezepte_bilder SET bild_ordnen = @sortOrder WHERE bild_id = @imageId";
                command.Parameters.AddWithValue("@sortOrder", ru.Value);
                command.Parameters.AddWithValue("@imageId", ru.UnitID);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptImageSortOrder", ex);
                return false;
            }
            finally
            {
                CloseReceipt();
            }
        }


        public async Task<long> UpdateReceiptIngridientUnitAdd(long receiptId, ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                using var command = conReceipt.CreateCommand();
                command.CommandText = "INSERT INTO talun_rezepte_zuei(rezept_id, einheit_id, zutat_id, menge) VALUES (@receiptId, @unitId, @unitId2, @value)";
                command.Parameters.AddWithValue("@receiptId", receiptId);
                command.Parameters.AddWithValue("@unitId", ru.UnitID);
                command.Parameters.AddWithValue("@unitId2", ru.UnitID2);
                command.Parameters.AddWithValue("@value", ru.Value);
                await command.ExecuteNonQueryAsync();

                using var selectCommand = conReceipt.CreateCommand();
                selectCommand.CommandText = "SELECT last_insert_rowid()";
                return (long)await selectCommand.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptIngridientUnitAdd", ex);
                return 0;
            }
            finally
            {
                CloseReceipt();
            }
        }


        public async Task<bool> UpdateReceiptIngridientUnitDelete(ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                using var command = conReceipt.CreateCommand();
                command.CommandText = "DELETE FROM talun_rezepte_zuei WHERE zuei_id = @unitId";
                command.Parameters.AddWithValue("@unitId", ru.UnitID);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptIngridientUnitDelete", ex);
                return false;
            }
            finally
            {
                CloseReceipt();
            }
        }


        public async Task<bool> UpdateReceiptIngridientUnitUpdate(ReceiptUpdateDTO ru)
        {
            try
            {
                await OpenReceipt();
                using var command = conReceipt.CreateCommand();
                command.CommandText = "UPDATE talun_rezepte_zuei SET menge = @value WHERE zuei_id = @unitId";
                command.Parameters.AddWithValue("@value", ru.Value);
                command.Parameters.AddWithValue("@unitId", ru.UnitID);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("databaseWrapper.UpdateReceiptIngridientUnitUpdate", ex);
                return false;
            }
            finally
            {
                CloseReceipt();
            }
        }


        public async Task<bool> UpdateUnit(int id, string unit)
        {
            Boolean ret = false;
            try
            {
                await OpenReceipt();
                using var rcmd = conReceipt.CreateCommand();
                rcmd.CommandText = "UPDATE talun_rezepte_einheit SET einheit = @unit WHERE einheit_id = @id";
                rcmd.Parameters.AddWithValue("@unit", unit);
                rcmd.Parameters.AddWithValue("@id", id);
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
    }
}
