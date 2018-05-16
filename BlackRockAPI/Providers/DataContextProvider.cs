using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace BlackRockAPI.Providers
{
    public class DataContextProvider : IDisposable
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataAdapter dataAdapter;
        private SqlDataReader dataReader;
        private SqlTransaction transaction;
        private DataSet dataSet;

        public DataContextProvider()
        {
            try
            {
                connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CodePlexContext"].ToString());
            }
            catch(Exception ex)
            {

            }
        }

        public bool ExecuteNonQuery(string spName, Dictionary<string, string> parameters)
        {
            try
            {
                connection.Open();
                command = new SqlCommand(spName, connection);
                command.CommandType = CommandType.StoredProcedure;
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
                }

                return command.ExecuteNonQuery() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<object> ExecuteReader(string spName)
        {
            try
            {
                connection.Open();
                dataSet = new DataSet();
                dataAdapter = new SqlDataAdapter();
                command = new SqlCommand(spName, connection);
                command.CommandType = CommandType.StoredProcedure;

                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(dataSet);

                DataTable dt = dataSet.Tables[0];

                List<DataRow> lr = dt.AsEnumerable().ToList();

                List<object> resultList = new List<object>();

                foreach(var item in lr)
                {
                    resultList.Add(item.ItemArray);
                }
                return resultList;
            }
            catch(Exception ex)
            {
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Executes stored procedure and returns a datatable with result
        /// </summary>
        /// <param name="spName">stored procedure name</param>
        /// <param name="parameters">parameters needed to stored procedure</param>
        /// <returns></returns>
        public DataTable ExecuteReader(string spName, Dictionary<string,string> parameters)
        {
            try
            {
                dataSet = new DataSet();
                dataAdapter = new SqlDataAdapter();
                command = new SqlCommand(spName, connection);
                command.CommandType = CommandType.StoredProcedure;
                foreach(var parameter in parameters)
                {
                    command.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
                }

                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
            catch(Exception ex)
            {
                return null;
            }            
            finally
            {
                connection.Close();
            }
        }

        public DataTable ExecuteScalar()
        {
            return new DataTable();
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}