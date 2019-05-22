using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using TouristTalk.Exceptions;

namespace TouristTalk.DataConnect
{
    /// <summary>
    /// Communicates with database.
    /// </summary>
    public class DataConnect : Dependency.IDataConnect
    {
        private string _sqlConnectString;

        /// <summary>
        /// Initialisation of data source (connection with DB)
        /// </summary>
        public DataConnect()
        {
            try
            {
                ReadDBConnData();
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error initiliasing DataConnect", e);
            }
        }

        /// <summary>
        /// reads the connection string from config. Sets sqlConnectString.
        /// </summary>
        private void ReadDBConnData()
        {
            try
            {
                _sqlConnectString = ConfigurationManager.ConnectionStrings["TouristMessageDataBase"].ConnectionString;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error occured reading database details", e);
            }
        }

        /// <summary>
        /// Executes a stored procedure that returns a recordset
        /// </summary>
        /// <param name="spName">name of stored procedure to call</param>
        /// <param name="parameters">dictionary of parameters for stored proc</param>
        /// <returns>recordset</returns>
        public List<Dictionary<string, object>> SendDataRequest(string spName, Dictionary<string, object> parameters)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(_sqlConnectString))
                {
                    return GetRecordSet(spName, sqlCon, parameters);
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error retrieving record set, using " + spName, e);
            }
        }

        /// <summary>
        /// Gets the record set. - opens connection, calls reader
        /// </summary>
        /// <param name="spName">stored proc name</param>
        /// <param name="sqlCon">established connection</param>
        /// <param name="parameters">dictionary of parameters for stored procedure</param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetRecordSet(string spName, SqlConnection sqlCon, Dictionary<string, object> parameters)
        {
            try
            {
                using (SqlCommand sqlCmd = BuildSqlCommand(spName, sqlCon, parameters))
                {
                    sqlCon.Open(); // open connection
                    return SqlReader(sqlCmd); //read results
                }
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Getting Record Set", e);
            }            
        }

        /// <summary>
        /// Builds SqlClient Command for executing stored proc command
        /// </summary>
        /// <param name="spName">name of stored procedure to call</param>
        /// <param name="sqlCon">sql connection</param>
        /// <param name="parameters">dictionary of parameters for stored proc</param>
        /// <returns></returns>
        private SqlCommand BuildSqlCommand(string spName, SqlConnection sqlCon, Dictionary<string, object> parameters)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand(spName, sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure; //set command type to call stored procedure
                
                // add any parameters for stored procedure
                if (parameters != null && parameters.Count != 0)
                {
                    sqlCmd = AddAllParameters(sqlCmd, parameters);
                }

                return sqlCmd;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error building sql command", e);
            }
        }

        /// <summary>
        /// Adds parameters for stored procedure to sql command
        /// </summary>
        /// <param name="sqlCmd">sql command</param>
        /// <param name="parameters">dictionary of parameters for stored proc</param>
        /// <returns>SqlCommand with addded parameters</returns>
        private SqlCommand AddAllParameters(SqlCommand sqlCmd, Dictionary<string, object> parameters)
        {
            try
            {
                foreach (string parameterKey in parameters.Keys)
                {
                    sqlCmd = AddParameter(sqlCmd, parameterKey, parameters[parameterKey]);
                }

                return sqlCmd;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Adding stored procedure parameters", e);
            }

        }

        /// <summary>
        /// Adds a parameter to SQLCommand
        /// </summary>
        /// <param name="sqlCmd">sql command</param>
        /// <param name="parameterKey">name of parameter</param>
        /// <param name="parameterValue">value of parameter</param>
        /// <returns></returns>
        private SqlCommand AddParameter(SqlCommand sqlCmd, string parameterKey,  object parameterValue)
        {
            try
            {
                if (parameterValue == null)
                {
                    sqlCmd.Parameters.AddWithValue("@" + parameterKey, SqlBinary.Null); ;
                }
                else
                {
                    sqlCmd.Parameters.AddWithValue("@" + parameterKey, parameterValue);
                }
                return sqlCmd;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Adding stored procedure parameter", e);
            }
        }
        
        /// <summary>
        /// Reads an sql recordset. Places in a List of dictionarys.
        /// Where each dictionary is a row. 
        /// </summary>
        /// <param name="sqlCmd">sql result set</param>
        /// <returns>List of dictionarys.
        /// Where each dictionary is a row of recordset. Key is the column, value is the value.</returns>
        private List<Dictionary<string, object>> SqlReader(SqlCommand sqlCmd)
        {
            try
            {
                List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    //for every row
                    while (reader != null && reader.Read())
                    {                 
                        records.Add(GetRecord(reader));
                    }
                }
                return records;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error reading record set", e);
            }
        }

        /// <summary>
        /// Gets one individual record (row) from dataset
        /// </summary>
        /// <param name="reader">triggered SqlReader</param>
        /// <returns>Dictionary containing information for one record</returns>
        private Dictionary<string, object> GetRecord(SqlDataReader reader)
        {
            try
            {
                Dictionary<string, object> record = new Dictionary<string, object>();
                //for every column
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader[i] is DBNull)
                    {
                        record.Add(reader.GetName(i), null);
                    }
                    else
                    {
                        record.Add(reader.GetName(i), reader[i]);
                    }
                }

                return record;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error Getting record set", e);
            }
        }
    }
}