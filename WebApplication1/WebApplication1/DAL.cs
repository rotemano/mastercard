using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Text;

namespace WebApplication1
{


    public class DAL
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["Connection_str"].ToString();

        #region Fill Data Table

        public static DataTable FillDataTableFromQuery(string query, Dictionary<string, string> queryParams)
        {
            DataTable dt = new DataTable();
            SqlTransaction transaction = null;

            try
            {

                if (String.IsNullOrEmpty(query) == false)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            if (queryParams != null)
                            {
                                foreach (KeyValuePair<string, string> kvp in queryParams)
                                {
                                    command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? "");
                                }
                            }

                            connection.Open();
                            transaction = connection.BeginTransaction();
                            command.Transaction = transaction;
                            using (SqlDataReader reader = command.ExecuteReader())
                                dt.Load(reader, LoadOption.Upsert);
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> exQueryParams = null;
                if (queryParams == null)
                    exQueryParams = new Dictionary<string, string>();
                else
                    exQueryParams = new Dictionary<string, string>(queryParams);
                exQueryParams.Add("Query", query);
                ExceptionsLogger.LogException(ex, exQueryParams);
            }

            return dt;
        }

        public static DataTable FillDataTableFromSp(string sp)
        {
            DataTable dt = new DataTable();

            try
            {
                if (String.IsNullOrEmpty(sp) == false)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = sp;
                            command.CommandTimeout = 120;

                            connection.Open();

                            using (SqlDataReader reader = command.ExecuteReader())
                                dt.Load(reader, LoadOption.Upsert);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionsLogger.LogException(ex, new Dictionary<string, string>() { { "SP", sp } });
            }

            return dt;
        }

        public static DataTable FillDataTableFromSp(string sp, Dictionary<string, string> queryParams)
        {
            return FillDataTableFromSp(sp, queryParams, null);
        }

        public static DataTable FillDataTableFromSp(string sp, Dictionary<string, string> queryParams, string connStringAppKey)
        {
            bool succeeded = true;
            return FillDataTableFromSp(sp, queryParams, connStringAppKey, out succeeded);
        }

        public static DataTable FillDataTableFromSp(string sp, Dictionary<string, string> queryParams, string connStringAppKey, out bool succeeded)
        {
            DataTable dt = new DataTable();
            succeeded = true;

            try
            {
                if (String.IsNullOrEmpty(sp) == false)
                {
                    string connString = connectionString;
                    if (String.IsNullOrEmpty(connStringAppKey) == false)
                        connString = ConfigurationManager.ConnectionStrings[connStringAppKey].ToString();

                    using (SqlConnection connection = new SqlConnection(connString))
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = sp;
                            command.CommandTimeout = 120;

                            if (queryParams != null)
                            {
                                foreach (KeyValuePair<string, string> kvp in queryParams)
                                {
                                    command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? "");
                                }
                            }

                            connection.Open();

                            using (SqlDataReader reader = command.ExecuteReader())
                                dt.Load(reader, LoadOption.Upsert);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                succeeded = false;
                if (queryParams == null)
                    queryParams = new Dictionary<string, string>();
                queryParams.Add("SP", sp);
                ExceptionsLogger.LogException(ex, queryParams);
                throw ex;
            }

            return dt;
        }

        #endregion


        //#region Fill Data Set

        //public static DataSet FillDataSet(string query, bool isSp, Dictionary<string, string> queryParams)
        //{
        //    DataSet ds = new DataSet();

        //    try
        //    {
        //        if (String.IsNullOrEmpty(query) == false)
        //        {
        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                using (SqlCommand command = new SqlCommand())
        //                {
        //                    command.Connection = connection;
        //                    command.CommandText = query;

        //                    if (isSp)
        //                        command.CommandType = CommandType.StoredProcedure;

        //                    if (queryParams != null)
        //                    {
        //                        foreach (KeyValuePair<string, string> kvp in queryParams)
        //                        {
        //                            command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? "");
        //                        }
        //                    }

        //                    connection.Open();

        //                    using (SqlDataAdapter da = new SqlDataAdapter(command))
        //                    {
        //                        da.Fill(ds);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionsLogger.LogException(ex, new Dictionary<string, string>() { { "Query", query } });
        //    }

        //    return ds;
        //}

        //#endregion


        #region Execute Non Query

        public static int ExecuteNonQuery(string query, Dictionary<string, string> queryParams)
        {
            int numberOfRowsAffected = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        foreach (KeyValuePair<string, string> kvp in queryParams)
                        {
                            command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? "");
                        }
                        connection.Open();
                        numberOfRowsAffected = (int)command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> exQueryParams = null;
                if (queryParams == null)
                    exQueryParams = new Dictionary<string, string>();
                else
                    exQueryParams = new Dictionary<string, string>(queryParams);
                exQueryParams.Add("Query", query);
                ExceptionsLogger.LogException(ex, exQueryParams);
            }

            return numberOfRowsAffected;
        }

        //public static int ExecuteNonQuery(string query, Dictionary<string, object> queryParams)
        //{
        //    int numberOfRowsAffected = -1;
        //    SqlTransaction transaction = null;

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            using (SqlCommand command = new SqlCommand(query, connection))
        //            {
        //                foreach (KeyValuePair<string, object> kvp in queryParams)
        //                {
        //                    command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? "");
        //                }

        //                connection.Open();
        //                transaction = connection.BeginTransaction();
        //                command.Transaction = transaction;
        //                numberOfRowsAffected = (int)command.ExecuteNonQuery();
        //                transaction.Commit();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionsLogger.LogException(ex, new Dictionary<string, string>() { { "Query", query } });
        //    }

        //    return numberOfRowsAffected;
        //}



        #endregion

        #region Execute Scalar

        public static object ExecuteScalar(string query, Dictionary<string, string> queryParams)
        {
            return ExecuteScalar(query, queryParams, false);
        }

        public static object ExecuteScalar(string query, Dictionary<string, string> queryParams, bool isSp, bool withTransaction = true)
        {
            object result = null;
            SqlTransaction transaction = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (queryParams != null)
                        {
                            foreach (KeyValuePair<string, string> kvp in queryParams)
                            {
                                command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? "");
                            }
                        }

                        if (isSp)
                            command.CommandType = CommandType.StoredProcedure;

                        connection.Open();

                        if (withTransaction)
                        {
                            transaction = connection.BeginTransaction();
                            command.Transaction = transaction;
                        }

                        result = command.ExecuteScalar();

                        if (withTransaction)
                            transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
#pragma warning disable CS0436 // Type conflicts with imported type
                ExceptionsLogger.LogException(ex, new Dictionary<string, string>() { { "Query", query } });
#pragma warning restore CS0436 // Type conflicts with imported type
            }

            return result;
        }

        #endregion

        #region Bulk Insert

        public static bool BulkInsert(DataTable dt, string destinationTableName)
        {
            return BulkInsert(dt, destinationTableName, new Dictionary<string, string>());
        }

        public static bool BulkInsert(DataTable dt, string destinationTableName, Dictionary<string, string> columnMappings)
        {
            bool succeeded = true;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = destinationTableName;

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        if (columnMappings != null)
                        {
                            foreach (KeyValuePair<string, string> kvp in columnMappings)
                            {
                                sqlBulkCopy.ColumnMappings.Add(kvp.Key, kvp.Value);
                            }
                        }

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionsLogger.LogException(ex);
                succeeded = false;
            }

            return succeeded;
        }

        public static bool BulkInsert(DataTable dt, string destinationTableName, List<KeyValuePair<string, string>> columnMappings)
        {
            bool succeeded = true;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = destinationTableName;

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        if (columnMappings != null)
                        {
                            foreach (KeyValuePair<string, string> kvp in columnMappings)
                            {
                                sqlBulkCopy.ColumnMappings.Add(kvp.Key, kvp.Value);
                            }
                        }

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionsLogger.LogException(ex);
                succeeded = false;
            }

            return succeeded;
        }

        #endregion

    }
}