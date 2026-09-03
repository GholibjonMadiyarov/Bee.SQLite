using Bee.SQLite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Reflection;

namespace Bee.SQLite
{
    public class SQLite
    {
        //Connection string
        public static string connectionString = "data source=Test.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000;";
        public static string logPath = null;

        public void version(string path = null)
        {
            if (path != null)
            {
                Log.info(path, "Version:" + Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString());
            }
        }

        public void version()
        {
            if (logPath != null)
            {
                Log.info(logPath, "Version:" + Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString());
            }
        }

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
                var rows = new List<Dictionary<string, object>>();

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
                                {
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>();

                                for (int i = 0; i <= reader.FieldCount - 1; i++)
                                {
                                    if (reader.IsDBNull(i) || reader.GetValue(i) == null)
                                    {
                                        row[reader.GetName(i)] = null;
                                    }
                                    else
                                    {
                                        row[reader.GetName(i)] = reader.GetValue(i);
                                    }
                                }
                                
                                rows.Add(row);
                            }
                        }
                    }
                }

                return new Select { execute = true, message = "Request completed successfully", queryText = queryText, data = rows };
            }
            catch(Exception e)
            {
                return new Select { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace, queryText = queryText, data = new List<Dictionary<string, object>>() };
            }
        }

        /// <summary>
        /// Used to retrieve data from a database.
        /// </summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns> Select model. The response is returned as a list of dictionary type.</returns>
        public static SelectString selectString(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                var rows = new List<Dictionary<string, string>>();

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
                                {
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, string>();

                                for (int i = 0; i <= reader.FieldCount - 1; i++)
                                {
                                    if (reader.IsDBNull(i) || reader.GetValue(i) == null)
                                    {
                                        row[reader.GetName(i)] = null;
                                    }
                                    else
                                    {
                                        row[reader.GetName(i)] = reader.GetValue(i).ToString();
                                    }
                                }

                                rows.Add(row);
                            }
                        }
                    }
                }

                return new SelectString { execute = true, message = "Request completed successfully", data = rows };
            }
            catch (Exception e)
            {
                return new SelectString { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace, data = new List<Dictionary<string, string>>() };
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
                                {
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                for (int i = 0; i <= reader.FieldCount - 1; i++)
                                {
                                    if (reader.IsDBNull(i) || reader.GetValue(i) == null)
                                    {
                                        row[reader.GetName(i)] = null;
                                    }
                                    else
                                    {
                                        row[reader.GetName(i)] = reader.GetValue(i);
                                    }
                                }

                                return new SelectRow { execute = true, message = "Request completed successfully", data = row, read = true };
                            }
                        }
                    }
                }

                return new SelectRow { execute = true, message = "Request completed successfully", data = row, read = false };
            }
            catch(Exception e)
            {
                return new SelectRow { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace, data = new Dictionary<string, object>(), read = false, exception = true };
            }
        }

        /// <summary>
        /// Used to retrieve data from a database.
        /// </summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns> SelectValue model. The first column of the first row is returned.</returns>
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
                                {
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SelectValue { execute = true, message = "Request completed successfully", value = (reader.IsDBNull(0) || reader.GetValue(0) == null) ? null : reader.GetValue(0), read = true };
                            }
                        }
                    }
                }
                return new SelectValue { execute = true, message = "The request was successful, but no result was returned", value = null, read = false };
            }
            catch(Exception e)
            {
                return new SelectValue { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace, value = null, exception = true};
            }
        }

        /// <summary>
        /// Used to retrieve data from a database.
        /// </summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns> SelectValueString model. The first column of the first row is returned.</returns>
        public static SelectValueString selectValueString(string queryText, Dictionary<string, object> parameters = null)
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
                                {
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SelectValueString { execute = true, message = "Request completed successfully", value = (reader.IsDBNull(0) || reader.GetValue(0) == null) ? null : reader.GetValue(0).ToString(), read = true };
                            }
                        }
                    }
                }
                return new SelectValueString { execute = true, message = "The request was successful, but no result was returned", value = null, read = false };
            }
            catch (Exception e)
            {
                return new SelectValueString { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace, value = null, exception = true };
            }
        }

        /// <summary>
        /// Used to retrieve data from a database.
        /// </summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <returns> SelectValueInteger model. The first column of the first row is returned.</returns>
        public static SelectValueInteger selectValueInteger(string queryText, Dictionary<string, object> parameters = null)
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
                                {
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.IsDBNull(0))
                                {
                                    return new SelectValueInteger { execute = true, message = "Request completed successfully", value = null, read = true };
                                }

                                return new SelectValueInteger { execute = true, message = "Request completed successfully", value = Convert.ToInt32(reader.GetValue(0)), read = true };
                            }
                        }
                    }
                }

                return new SelectValueInteger { execute = true, message = "The request was successful, but no result was returned", value = null, read = false };
            }
            catch (Exception e)
            {
                return new SelectValueInteger { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace, value = null, exception = true };
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
                                command.Transaction = transaction;
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
                                            foreach (var parameter in parameters[index])
                                            {
                                                command.Parameters.AddWithValue(parameter.Key, parameter.Value != null ? parameter.Value : DBNull.Value);
                                            }
                                        }
                                    }

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
                            return new Insert { execute = false, message = "Transaction canceled. " + e.Message, stackTrace = e.StackTrace, duplicate = (e.ErrorCode == (int)SQLiteErrorCode.Constraint) ? true : false };
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return new Insert { execute = false, message = "Transaction canceled. " + e.Message, stackTrace = e.StackTrace };
                        }
                    }
                }
            }
            catch(Exception e)
            {
                return new Insert { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace};
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
                                command.Transaction = transaction;

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

                                transaction.Commit();

                                return new Insert { execute = true, message = "Request completed successfully", insertedId = connection.LastInsertRowId};
                            }
                        }
                        catch (SQLiteException e)
                        {
                            transaction.Rollback();
                            return new Insert { execute = false, message = "Transaction canceled. " + e.Message, stackTrace = e.StackTrace, duplicate = (e.ErrorCode == (int)SQLiteErrorCode.Constraint) ? true : false };
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return new Insert { execute = false, message = "Transaction canceled. " + e.Message, stackTrace = e.StackTrace};
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new Insert { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace };
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
                                {
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                        }

                        int affectedRowCount = command.ExecuteNonQuery();

                        return new Update { execute = true, message = "Request completed successfully!", affectedRowCount = affectedRowCount };
                    }
                }
            }
            catch (Exception e)
            {
                return new Update { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace};
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
                                {
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                        }

                        int affectedRowCount = command.ExecuteNonQuery();

                        return new Delete { execute = true, message = "Request completed successfully!", affectedRowCount = affectedRowCount };
                    }
                }
            }
            catch (Exception e)
            {
                return new Delete { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace };
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
                                {
                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                }
                            }
                        }

                        command.ExecuteNonQuery();
                        return new Query { execute = true, message = "Request completed successfully!" };
                    }
                }
            }
            catch (SQLiteException e)
            {
                return new Query { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace, duplicate = (e.ErrorCode == (int)SQLiteErrorCode.Constraint) ? true : false };
            }
            catch (Exception e)
            {
                return new Query { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace };
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
                                command.Transaction = transaction;

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
                                                {
                                                    command.Parameters.AddWithValue(parameter.Key, DBNull.Value);
                                                }
                                                else
                                                {
                                                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                                                }
                                            }
                                        }
                                    }

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

                            return new Query { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace, duplicate = (e.ErrorCode == (int)SQLiteErrorCode.Constraint) ? true : false };
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return new Query { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace };
                        }
                    }
                }
            }
            catch(Exception e)
            {
                return new Query { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace };
            }
        }

        /// <summary>
        /// Set password
        /// </summary>
        /// <param name="passwordText">Password Text</param>
        /// <returns>Boolean</returns>
        public static bool setPassword(string passwordText)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.SetPassword(passwordText);
                    connection.Open();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="passwordText">Password Text</param>
        /// <returns>Boolean</returns>
        public static bool changePassword(string passwordText)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.ChangePassword(passwordText);
                    connection.Open();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
