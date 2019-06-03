using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoryPerformanceTester
{
    public static class ODBCTools
    {
        /// <summary>
        /// Returns the correct ODBC connection for the given service
        /// </summary>
        /// <param name="strServiceName"></param>
        /// <returns>OdbcConnection</returns>
        public static OdbcConnection GetOdbcConnection(string strServiceName)
        {
            string strOdbcConnectionString;
            CXSCRIPTLib.GlobalFunctions globalFunctions = new CXSCRIPTLib.GlobalFunctions();
            strOdbcConnectionString = globalFunctions.GetODBCConnectionString(strServiceName);

            OdbcConnection connection = new OdbcConnection(strOdbcConnectionString);

            return connection;
        }

        /// <summary>
        /// Returns the correct ADO ODBC connection for the given service
        /// </summary>
        /// <param name="strServiceName"></param>
        /// <returns>ADODB.Connection</returns>
        public static ADODB.Connection GetAdoOdbcConnection(string strServiceName)
        {
            string strOdbcConnectionString;
            CXSCRIPTLib.GlobalFunctions globalFunctions = new CXSCRIPTLib.GlobalFunctions();
            strOdbcConnectionString = globalFunctions.GetODBCConnectionString(strServiceName);

            ADODB.Connection connection = new ADODB.Connection();
            connection.Open(strOdbcConnectionString, null, null, 0);

            return connection;
        }

        /// <summary>
        /// Returns the service in the format "SITE_SERVICE", which is how ODBC wants it
        /// </summary>
        /// <returns></returns>
        public static string GetServiceNameAsOdbcTable(string strService)
        {
            return String.Format("\"{0}\"", strService.Replace('.', '_'));
        }
    }
}
