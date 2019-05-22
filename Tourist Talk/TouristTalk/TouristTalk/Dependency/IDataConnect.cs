using System.Collections.Generic;

namespace TouristTalk.Dependency
{
    //interface to allow dependency injection of database
    public interface IDataConnect
    {
        /// <summary>
        /// Executes a stored procedure that returns a recordset
        /// </summary>
        /// <param name="spName">name of stored procedure to call</param>
        /// <param name="parameters">dictionary of parameters for stored proc</param>
        /// <returns>recordset</returns>
        List<Dictionary<string, object>> SendDataRequest(string spName, Dictionary<string, object> parameters);

    }
}
