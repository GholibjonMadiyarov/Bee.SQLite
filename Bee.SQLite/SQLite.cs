using Bee.SQLite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Reflection;

namespace Bee.SQLite
{
    /// <summary>Static methods for working with an SQLite database. Methods do not throw: errors are returned in the result object.</summary>
    public class SQLite
    {
        /// <summary>Connection string used by all methods.</summary>
        public static string connectionString = "data source=Test.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000;";

        /// <summary>Log file path used by the instance method version(). Null disables logging.</summary>
        public static string logPath = null;

        /// <summary>Writes the library version to a log file.</summary>
        /// <param name="path">Log file path. If null, nothing is written.</param>
        public static void version(string path = null)
        {
            if (path != null)
            {
                Log.info(path, "Version:" + Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString());
            }
        }

        /// <summary>Writes the library version to <c>logPath</c>. If <c>logPath</c> is null, nothing is written.</summary>
        public void version()
        {
            if (logPath != null)
            {
                Log.info(logPath, "Version:" + Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString());
            }
        }

        /// <summary>Runs a SELECT query and returns all rows.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>data</c>: rows as "column → value"; NULL becomes null.</returns>
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

                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> parameter in parameters)
                            {
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();

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

        /// <summary>Runs a SELECT query and returns all rows with values converted to strings.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>data</c>: rows as "column → string"; NULL becomes null.</returns>
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
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
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

        /// <summary>Runs a SELECT query and returns the first row.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>data</c>: first row as "column → value". <c>read</c> is false if there are no rows.</returns>
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
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
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

        /// <summary>Runs a query and returns the first column of the first row.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>value</c>: the value, or null. <c>read</c> is false if there are no rows.</returns>
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
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
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

        /// <summary>Runs a query and returns the first column of the first row as a string.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>value</c>: the value as string, or null. <c>read</c> is false if there are no rows.</returns>
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
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
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

        /// <summary>Runs a query and returns the first column of the first row as a long.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>value</c>: the value converted with Convert.ToInt64, or null. <c>read</c> is false if there are no rows.</returns>
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
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
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

                                return new SelectValueInteger { execute = true, message = "Request completed successfully", value = Convert.ToInt64(reader.GetValue(0)), read = true };
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

        /// <summary>Runs an INSERT query.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>insertedId</c>: rowid of the inserted row, or null if nothing was inserted. <c>duplicate</c> is true on a UNIQUE or PRIMARY KEY violation.</returns>
        public static Insert insert(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                long? insertedId = null;
                List<long?> insertedIds = new List<long?>();

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    connection.SetExtendedResultCodes(true);

                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                            }
                        }

                        int count = command.ExecuteNonQuery();

                        if (count > 0)
                        {
                            insertedId = connection.LastInsertRowId;
                            insertedIds.Add(insertedId);
                        }

                        return new Insert
                        {
                            execute = true,
                            message = "Request completed successfully",
                            insertedId = insertedId,
                            insertedIds = insertedIds
                        };
                    }
                }
            }
            catch (SQLiteException e)
            {
                return new Insert { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace, duplicate = e.ResultCode == SQLiteErrorCode.Constraint_Unique || e.ResultCode == SQLiteErrorCode.Constraint_PrimaryKey };
            }
            catch (Exception e)
            {
                return new Insert { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace };
            }
        }

        /// <summary>Runs several INSERT queries in one transaction. If one fails, all are rolled back.</summary>
        /// <param name="queryTexts">SQL queries, run in order.</param>
        /// <param name="parameters">Parameters for each query, in the same order. An item may be null.</param>
        /// <returns><c>insertedIds[i]</c>: rowid added by query i, or null. <c>insertedId</c>: the last added rowid. <c>duplicate</c> is true on a UNIQUE or PRIMARY KEY violation.</returns>
        public static Insert insert(List<string> queryTexts, List<Dictionary<string, object>> parameters = null)
        {
            try
            {
                if (queryTexts == null || queryTexts.Count == 0)
                {
                    return new Insert
                    {
                        execute = false,
                        message = "No queries provided"
                    };
                }

                if (parameters != null && parameters.Count != queryTexts.Count)
                {
                    return new Insert
                    {
                        execute = false,
                        message = "The number of parameters does not match the number of queries"
                    };
                }

                List<long?> insertedIds = new List<long?>();
                long? insertedId = null;
                int index = 0;

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    connection.SetExtendedResultCodes(true);

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.Connection = connection;
                                command.Transaction = transaction;
                                command.CommandType = CommandType.Text;

                                index = 0;

                                while (index <= queryTexts.Count - 1)
                                {
                                    command.CommandType = CommandType.Text;
                                    command.CommandText = queryTexts[index];

                                    command.Parameters.Clear();

                                    if (parameters != null)
                                    {
                                        if (parameters != null && parameters.Count > 0 && parameters[index] != null)
                                        {
                                            foreach (var parameter in parameters[index])
                                            {
                                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                                            }
                                        }
                                    }

                                    int count = command.ExecuteNonQuery();

                                    if (count > 0)
                                    {
                                        insertedId = connection.LastInsertRowId;
                                        insertedIds.Add(insertedId);
                                    }
                                    else
                                    {
                                        insertedIds.Add(null);
                                    }

                                    index++;
                                }

                                transaction.Commit();

                                return new Insert { 
                                    execute = true, 
                                    message = "Request completed successfully",
                                    insertedId = insertedId,
                                    insertedIds = insertedIds
                                };
                            }
                        }
                        catch (SQLiteException e)
                        {
                            transaction.Rollback();
                            return new Insert { execute = false, message = "Transaction canceled. " + e.Message, stackTrace = e.StackTrace, duplicate = e.ResultCode == SQLiteErrorCode.Constraint_Unique || e.ResultCode == SQLiteErrorCode.Constraint_PrimaryKey };
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

        /// <summary>Runs an UPDATE query.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>affectedRowCount</c>: number of changed rows.</returns>
        public static Update update(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    connection.SetExtendedResultCodes(true);

                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                            }
                        }

                        int affectedRowCount = command.ExecuteNonQuery();

                        return new Update { execute = true, message = "Request completed successfully", affectedRowCount = affectedRowCount };
                    }
                }
            }
            catch (Exception e)
            {
                return new Update { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace};
            }
        }

        /// <summary>Runs several UPDATE queries in one transaction. If one fails, all are rolled back.</summary>
        /// <param name="queryTexts">SQL queries, run in order.</param>
        /// <param name="parameters">Parameters for each query, in the same order. An item may be null.</param>
        /// <returns><c>affectedRowCount</c>: total number of changed rows.</returns>
        public static Update update(List<string> queryTexts, List<Dictionary<string, object>> parameters = null)
        {
            try
            {
                if (queryTexts == null || queryTexts.Count == 0)
                {
                    return new Update
                    {
                        execute = false,
                        message = "No queries provided"
                    };
                }

                if (parameters != null && parameters.Count != queryTexts.Count)
                {
                    return new Update { execute = false, message = "The number of parameters does not match the number of queries" };
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    connection.SetExtendedResultCodes(true);

                    int index = 0;
                    int affectedRowCount = 0;

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.Connection = connection;
                                command.Transaction = transaction;
                                command.CommandType = CommandType.Text;

                                while (index <= queryTexts.Count - 1)
                                {
                                    command.CommandText = queryTexts[index];

                                    command.Parameters.Clear();

                                    if (parameters != null && parameters.Count > 0)
                                    {
                                        if (parameters[index] != null)
                                        {
                                            foreach (var parameter in parameters[index])
                                            {
                                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                                            }
                                        }
                                    }

                                    int i = command.ExecuteNonQuery();

                                    index++;

                                    affectedRowCount += i;
                                }

                                transaction.Commit();

                                return new Update { 
                                    execute = true, 
                                    message = "Request completed successfully", 
                                    affectedRowCount = affectedRowCount 
                                };
                            }
                        }
                        catch (SQLiteException e)
                        {
                            transaction.Rollback();

                            return new Update { 
                                execute = false, 
                                message = "SQLiteException. Transaction canceled. " + e.Message, 
                                exception = true
                            };
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();

                            return new Update { 
                                execute = false, 
                                message = "Exception. Transaction canceled. " + e.Message, 
                                exception = true
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new Update { execute = false, message = "Exception. Request failed. " + e.Message, exception = true };
            }
        }

        /// <summary>Runs a DELETE query.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>affectedRowCount</c>: number of deleted rows.</returns>
        public static Delete delete(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    connection.SetExtendedResultCodes(true);

                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                            }
                        }

                        int affectedRowCount = command.ExecuteNonQuery();

                        return new Delete { execute = true, message = "Request completed successfully", affectedRowCount = affectedRowCount };
                    }
                }
            }
            catch (Exception e)
            {
                return new Delete { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace };
            }
        }

        /// <summary>Runs any SQL query: SELECT, INSERT, UPDATE, DELETE, CREATE and others.</summary>
        /// <param name="queryText">SQL query.</param>
        /// <param name="parameters">Parameters (name → value). A null value is sent as NULL.</param>
        /// <returns><c>data[0]</c>: returned rows (SELECT or RETURNING). <c>affectedRowCount</c>: number of changed rows. <c>insertedId</c>: rowid of the new row if the query starts with INSERT or REPLACE and added a row, otherwise null. <c>duplicate</c> is true on a UNIQUE or PRIMARY KEY violation.</returns>
        public static Query query(string queryText, Dictionary<string, object> parameters = null)
        {
            try
            {
                var rows = new List<Dictionary<string, object>>();

                int affectedRowCount = 0;
                long? insertedId = null;

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    connection.SetExtendedResultCodes(true);

                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryText;

                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> parameter in parameters)
                            {
                                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();

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

                            affectedRowCount = Math.Max(reader.RecordsAffected, 0);

                            string text = queryText.TrimStart().ToUpperInvariant();

                            if (affectedRowCount > 0 && (text.StartsWith("INSERT") || text.StartsWith("REPLACE")))
                            {
                                insertedId = connection.LastInsertRowId;
                            }
                        }
                    }
                }

                return new Query { 
                    execute = true, 
                    message = "Request completed successfully", 
                    queryText = queryText, 
                    data = new List<List<Dictionary<string, object>>> { rows }, 
                    affectedRowCount = affectedRowCount,
                    insertedId = insertedId
                };
            }
            catch (SQLiteException e)
            {
                return new Query { 
                    execute = false, 
                    exception = true,
                    message = "Request failed. " + e.Message,
                    exceptionType = e.GetType().Name,
                    exceptionNumber = e.ErrorCode,
                    duplicate = e.ResultCode == SQLiteErrorCode.Constraint_Unique || e.ResultCode == SQLiteErrorCode.Constraint_PrimaryKey,
                    stackTrace = e.StackTrace, 
                    queryText = queryText
                };
            }
            catch (Exception e)
            {
                return new Query {
                    execute = false, 
                    message = "Request failed. " + e.Message, 
                    stackTrace = e.StackTrace, 
                    queryText = queryText, 
                };
            }
        }

        /// <summary>Runs several SQL queries of any type in one transaction. If one fails, all are rolled back.</summary>
        /// <param name="queryTexts">SQL queries, run in order.</param>
        /// <param name="parameters">Parameters for each query, in the same order. An item may be null.</param>
        /// <returns><c>data[i]</c>: rows returned by query i. <c>affectedRowCount</c>: total number of changed rows. <c>insertedIds[i]</c>: rowid added by query i (INSERT or REPLACE), otherwise null. <c>insertedId</c>: the last added rowid. <c>duplicate</c> is true on a UNIQUE or PRIMARY KEY violation.</returns>
        public static Query query(List<string> queryTexts, List<Dictionary<string, object>> parameters = null)
        {
            try
            {
                var data = new List<List<Dictionary<string, object>>>();

                int affectedRowCount = 0;
                int index = 0;
                long? insertedId = null;
                var insertedIds = new List<long?>();

                if (queryTexts == null || queryTexts.Count == 0)
                {
                    return new Query
                    {
                        execute = false,
                        message = "No queries provided"
                    };
                }

                if (parameters != null && parameters.Count != queryTexts.Count)
                {
                    return new Query
                    {
                        execute = false,
                        message = "The number of parameters does not match the number of queries"
                    };
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    connection.SetExtendedResultCodes(true);

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.Connection = connection;
                                command.Transaction = transaction;
                                command.CommandType = CommandType.Text;

                                index = 0;

                                while (index <= queryTexts.Count - 1)
                                {
                                    var rows = new List<Dictionary<string, object>>();

                                    command.CommandType = CommandType.Text;
                                    command.CommandText = queryTexts[index];

                                    command.Parameters.Clear();

                                    if (parameters != null && parameters.Count > 0 && parameters[index] != null)
                                    {
                                        foreach (var parameter in parameters[index])
                                        {
                                            command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                                        }
                                    }

                                    using (SQLiteDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            var row = new Dictionary<string, object>();

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

                                        int count = Math.Max(reader.RecordsAffected, 0);

                                        affectedRowCount += count;

                                        data.Add(rows);

                                        string text = queryTexts[index].TrimStart().ToUpperInvariant();

                                        if (count > 0 && (text.StartsWith("INSERT") || text.StartsWith("REPLACE")))
                                        {
                                            insertedId = connection.LastInsertRowId;
                                            insertedIds.Add(insertedId);
                                        }
                                        else
                                        {
                                            insertedIds.Add(null);
                                        }
                                    }

                                    index++;
                                }

                                transaction.Commit();

                                return new Query { 
                                    execute = true, 
                                    message = "Request completed successfully", 
                                    data = data, 
                                    affectedRowCount = affectedRowCount,
                                    insertedId = insertedId,
                                    insertedIds = insertedIds
                                };
                            }
                        }
                        catch (SQLiteException e)
                        {
                            transaction.Rollback();

                            return new Query { 
                                execute = false, 
                                message = "Transaction canceled. " + e.Message, 
                                stackTrace = e.StackTrace, 
                                duplicate = e.ResultCode == SQLiteErrorCode.Constraint_Unique || e.ResultCode == SQLiteErrorCode.Constraint_PrimaryKey,
                                exception = true,
                                exceptionType = e.GetType().Name,
                                exceptionNumber = e.ErrorCode
                            };
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return new Query { 
                                execute = false, 
                                message = "Transaction canceled. " + e.Message, 
                                stackTrace = e.StackTrace,
                                exception = true,
                                exceptionType = e.GetType().Name
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new Query { execute = false, message = "Request failed. " + e.Message, stackTrace = e.StackTrace };
            }
        }

        /// <summary>Checks that the database opens with this password. The password is not stored; other methods use <c>connectionString</c>. Requires the SQLite Encryption Extension (SEE).</summary>
        /// <param name="passwordText">Password.</param>
        /// <returns>True if the database opened, false on error (also when SEE is not installed).</returns>
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

        /// <summary>Changes the database password. Requires the SQLite Encryption Extension (SEE).</summary>
        /// <param name="passwordText">New password.</param>
        /// <returns>True on success, false on error (also when SEE is not installed).</returns>
        public static bool changePassword(string passwordText)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    connection.ChangePassword(passwordText);
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
