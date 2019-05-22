using System;
using System.Configuration;
using System.Collections.Specialized;

namespace JunctionController
{
    /// <summary>
    /// For reading configuration from the app.config file
    /// </summary>
    public static class ConfigReader
    {
        private static NameValueCollection _appSettings = ConfigurationManager.AppSettings;

        /// <summary>
        /// Retrieves connection string from app.config
        /// </summary>
        /// <param name="connection">name of connection</param>
        /// <returns>connection string</returns>
        public static string GetConnectionString(string connection)
        {
            try
            {
                return (ConfigurationManager.ConnectionStrings[connection].ConnectionString);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error retrieving connction string from config", ex);
            }
        }

        /// <summary>
        /// Retrieves application setting
        /// </summary>
        /// <param name="key">name of setting</param>
        /// <returns>appliction setting as string</returns>
        public static string GetAppSettingString(string key)
        {
            try
            {
                return _appSettings[key];
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error retrieving application setting from config", ex);
            }
        }

        /// <summary>
        /// Retrieves boolean application setting
        /// </summary>
        /// <param name="key">name of setting</param>
        /// <returns>boolean application setting</returns>
        public static bool GetAppSettingBool(string key)
        {
            try
            {
                if (_appSettings[key].ToUpper() == "TRUE")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error retrieving boolean application setting from config", ex);
            }
        }

        /// <summary>
        /// Retrieves application setting and converts to an integer
        /// </summary>
        /// <param name="key">name of setting</param>
        /// <returns>application setting converted to integer</returns>
        public static int GetIntAppSetting(string key)
        {
            try
            {
                return Int32.Parse(GetAppSettingString(key));
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error getting integer application setting from config", ex);
            }
        }

        /// <summary>
        /// Retrieves application setting and converts to a double
        /// </summary>
        /// <param name="key">name of setting</param>
        /// <returns></returns>
        public static double GetDoubleAppSetting(string key)
        {
            try
            {
                return Double.Parse(GetAppSettingString(key));
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error getting double application setting from config", ex);
            }
        }
    }
}


