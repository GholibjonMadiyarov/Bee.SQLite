using Bee.SQLite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Bee.SQLite
{
    public class SQLite
    {
        //Connection string
        public static string connectionString = "data source=Test.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000";
        
        /// <summary>
        /// Used to retrieve data from a database.
        /// </summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns> Select model. The response is returned as a list of dictionary type.</returns>
        public static Select select(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null)
                        {
                            foreach (KeyValuePair<string, object> parameter in parameters)
                            {
                                if (parameter.Value == null)
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                else
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }

                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>();

                                for (int i = 0; i <= reader.FieldCount - 1; i++)
                                {
                                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader[reader.GetName(i)];
                                }
                                
                                rows.Add(row);
                            }
                        }
                    }
                }

                return new Select { execute = true, message = "Request completed successfully", data = rows };
            }
            catch(Exception e)
            {
                return new Select { execute = false, message = "Request failed. " + e.Message, data = new List<Dictionary<string, object>>() };
            }
        }

        /// <summary>
        /// Used to retrieve data from a database.
        /// </summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns> SelectItem model. The first line is returned as a dictionary.</returns>
        public static SelectRow selectRow(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                var row = new Dictionary<string, object>();

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                if (parameter.Value == null)
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                else
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }

                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                for (int i = 0; i <= reader.FieldCount - 1; i++)
                                {
                                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader[reader.GetName(i)];
                                }
                            }
                        }
                    }
                }

                return new SelectRow { execute = true, message = "Request completed successfully", data = row };
            }
            catch(Exception e)
            {
                return new SelectRow { execute = false, message = "Request failed. " + e.Message , data = new Dictionary<string, object>() };
            }
        }

        /// <summary>
        /// Used to retrieve data from a database.
        /// </summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns> SelectOne model. The first column of the first row is returned.</returns>
        public static SelectValue selectValue(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                if (parameter.Value == null)
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                else
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SelectValue { execute = true, message = "Request completed successfully", value = reader.IsDBNull(0) ? null : reader[0] };
                            }
                        }
                    }
                }
                return new SelectValue { execute = false, message = "The request was successful, but no result was returned", value = null };
            }
            catch(Exception e)
            {
                return new SelectValue { execute = false, message = "Request failed. " + e.Message, value = null};
            }
        }

        /// <summary>
        /// Requests insert for multiple queres.
        /// </summary>
        /// <param name="queryTexts">The SQL querys is represented as a list.</param>
        /// <param name="parameters">Parameters are given accordingly for each request.</param>
        /// <returns>Query model</returns>
        public static Insert insert(List<string> queryTexts, List<Dictionary<string, object>> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.Connection = connection;

                                int index = 0;

                                while (index <= queryTexts.Count - 1)
                                {
                                    command.CommandType =  CommandType.Text;
                                    command.CommandText = queryTexts[index];

                                    command.Parameters.Clear();

                                    if (parameters != null)
                                    {
                                        if (parameters[index] != null)
                                        {
                                            foreach (var parameter in parameters[index])
                                            {
                                                command.Parameters.AddWithValue(parameter.Key, parameter.Value != null ? parameter.Value : DBNull.Value);
                                            }
                                        }
                                    }

                                    command.Transaction = transaction;

                                    var r = command.ExecuteNonQuery();

                                    index++;
                                }

                                transaction.Commit();

                                return new Insert { execute = true, message = "Request completed successfully", insertedId = connection.LastInsertRowId };
                            }
                        }
                        catch (SQLiteException e)
                        {
                            transaction.Rollback();
                            return new Insert { execute = false, message = "Transaction canceled. " + e.Message, duplicate = (e.ErrorCode == (int)SQLiteErrorCode.Constraint) ? true : false };
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return new Insert { execute = false, message = "Transaction canceled. " + e.Message };
                        }
                    }
                }
            }
            catch(Exception e)
            {
                return new Insert { execute = false, message = "Request failed. " + e.Message };
            }
        }

        /// <summary>
        /// Requests insert
        /// </summary>
        /// <param name="queryText">The SQL query is represented as a text.</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Query model</returns>
        public static Insert insert(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.Connection = connection;
                                command.CommandType = CommandType.Text;
                                command.CommandText = queryText ;

                                if (parameters != null)
                                {
                                    foreach (var parameter in parameters)
                                    {
                                        if (parameter.Value == null)
                                            command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                        else
                                            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                    }
                                }

                                command.Transaction = transaction;

                                command.ExecuteNonQuery();

                                transaction.Commit();

                                return new Insert { execute = true, message = "Request completed successfully", insertedId = connection.LastInsertRowId};
                            }
                        }
                        catch (SQLiteException e)
                        {
                            transaction.Rollback();
                            return new Insert { execute = false, message = "Transaction canceled. " + e.Message, duplicate = (e.ErrorCode == (int)SQLiteErrorCode.Constraint) ? true : false };
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return new Insert { execute = false, message = "Transaction canceled. " + e.Message };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new Insert { execute = false, message = "Request failed. " + e.Message };
            }
        }

        /// <summary>
        /// Executes update requests.
        /// </summary>
        /// <param name="queryText">The SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Query model</returns>
        public static Update update(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                if (parameter.Value == null)
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                else
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }
                        }

                        int affectedRowCount = command.ExecuteNonQuery();
                        return new Update { execute = true, message = "Request completed successfully!", affectedRowCount = affectedRowCount };
                    }
                }
            }
            catch (Exception e)
            {
                return new Update { execute = false, message = "Request failed. " + e.Message };
            }
        }

        /// <summary>
        /// Executes delete requests.
        /// </summary>
        /// <param name="queryText">The SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Delete model</returns>
        public static Delete delete(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                if (parameter.Value == null)
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                else
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }
                        }

                        int affectedRowCount = command.ExecuteNonQuery();
                        return new Delete { execute = true, message = "Request completed successfully!", affectedRowCount = affectedRowCount };
                    }
                }
            }
            catch (Exception e)
            {
                return new Delete { execute = false, message = "Request failed. " + e.Message };
            }
        }

        /// <summary>
        /// Executes any query with out select requests.
        /// </summary>
        /// <param name="queryText">The SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Query model</returns>
        public static Query query(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                if (parameter.Value == null)
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                else
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }
                        }

                        command.ExecuteNonQuery();
                        return new Query { execute = true, message = "Request completed successfully!" };
                    }
                }
            }
            catch (SQLiteException e)
            {
                return new Query { execute = false, message = "Request failed. " + e.Message, duplicate = (e.ErrorCode == (int)SQLiteErrorCode.Constraint) ? true : false };
            }
            catch (Exception e)
            {
                return new Query { execute = false, message = "Request failed. " + e.Message };
            }
        }

        /// <summary>
        /// Executes any query with out select requests.
        /// </summary>
        /// <param name="queryText">The SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Query model</returns>
        public static Query query(List<string> queryTexts, List<Dictionary<string, object>> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.Connection = connection;
                                command.CommandType = CommandType.Text;

                                int index = 0;
                                while (index <= queryTexts.Count - 1)
                                {
                                    command.CommandText = queryTexts[index];

                                    command.Parameters.Clear();

                                    if (parameters != null)
                                    {
                                        if (parameters[index] != null)
                                        {
                                            foreach (KeyValuePair<string, object> parameter in parameters[index])
                                            {
                                                if (parameter.Value == null)
                                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                                else
                                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                            }
                                        }
                                    }

                                    command.Transaction = transaction;
                                    command.ExecuteNonQuery();
                                    index++;
                                }

                                transaction.Commit();

                                return new Query { execute = true, message = "Request completed successfully!"};
                            }
                        }
                        catch (SQLiteException e)
                        {
                            transaction.Rollback();

                            return new Query { execute = false, message = "Request failed. " + e.Message, duplicate = (e.ErrorCode == (int)SQLiteErrorCode.Constraint) ? true : false };
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return new Query { execute = false, message = "Request failed. " + e.Message };
                        }
                    }
                }
            }
            catch(Exception e)
            {
                return new Query { execute = false, message = "Request failed. " + e.Message };
            }
        }
    }
}
