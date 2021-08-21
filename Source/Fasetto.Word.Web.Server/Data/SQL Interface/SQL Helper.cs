using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;  
using System.Data.SqlClient;  
using System.Threading.Tasks;

namespace Fasetto.Word.Web.Server
{
    internal static class SqlHelper
    {
        /// <summary>
        /// All helper methods related to persistance of objects on SQL databases
        /// Where result sets are perceived to be managaeable in size, GetDataTableFromDb
        /// returns the result of a query in a data table
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        // Set the connection, command, and then execute the command with non query.  
        public static int ExecuteNonQuery(string connectionString, string commandText,
            CommandType commandType, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(commandText, conn))
                {
                    // There're three command types: StoredProcedure, Text, TableDirect. The TableDirect   
                    // type is only for OLE DB.    
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Set the connection, command, and then execute the command and only return one value.  
        public static object ExecuteScalar(string connectionString, string commandText,
            CommandType commandType, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        // Set the connection, command, and then execute the command with query and return the reader.  
        public static SqlDataReader ExecuteReader(string connectionString, string commandText,
            CommandType commandType, params SqlParameter[] parameters)
        {
            var conn = new SqlConnection(connectionString);

            using (var cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                // When using CommandBehavior.CloseConnection, the connection will be closed when the   
                // IDataReader is closed.  
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                return reader;
            }
        }
        #region <<ExecuteCommand_WithSharedConnection>>

        //public void ExecuteCommand_WithSharedConnection_ShouldPerformAsyncByDefault()
        //{
        //    var executedProcessList = new List<string>();

        //    //for shared connection we need to add MARS capabilities
        //    using (var conn = new SqlConnection(DataTestClass.SQL2005_Northwind + "MultipleActiveResultSets=true;"))
        //    {
        //        conn.Open();
        //        var task1 = ExecuteCommandWithSharedConnectionAsync(conn, "C", "SELECT top 10 * FROM Orders", executedProcessList);
        //        var task2 = ExecuteCommandWithSharedConnectionAsync(conn, "D", "SELECT top 10 * FROM Products", executedProcessList);
        //        //wait all before verifing the results
        //        Task.WaitAll(task1, task2);
        //    }

        //    //verify whether it executed async
        //    Assert.True(DoesProcessExecutedAsync(executedProcessList));
        //}

        internal static async Task<ICollection<string>> ExecuteCommandWithSharedConnectionAsync(SqlConnection conn, string processName, string cmdText, ICollection<string> executedProcessList)
        {
            conn.Open();
            var cmd = new SqlCommand(cmdText, conn);
           
            //var reader1 = cmd.ExecuteReader();
            //var cols = reader1.GetColumnSchema();
            //reader1.Close();
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
  
            {
                while (await reader.ReadAsync())
  
                {
                    executedProcessList.Add(processName + reader.GetString(1));
                  
                }
            }
            conn.Close();
            return executedProcessList;
        }
        #endregion
        #region KeyMethods
        public static  DataTable GetDataTableFromDb(SqlConnection conn,  string cmdText)
        {
            //var con = new SqlConnection(conn);
            conn.Open();
            var Adpt = new SqlDataAdapter(cmdText, conn);
            var dt = new DataTable();
            try
            {
                Adpt.Fill(dt);
            }
            catch (SqlException ex)
            {
                dt = null;
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            finally
            {
                if (conn != null)
                    if (conn.State == ConnectionState.Open) conn.Close();
                Adpt.Dispose();
            }
            return dt;
        }
        #endregion
    }
}