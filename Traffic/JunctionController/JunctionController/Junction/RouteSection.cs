using System;
using System.Collections.Generic;

namespace JunctionController
{
    /// <summary>
    /// defines section (lane) of a route
    /// </summary>
    public class RouteSection
    {
        private double _pollutionFactor;

        private double _pollution;

        private string _configName;

        /// <summary>
        /// returns Pollution for route adjusted by pollution factor
        /// </summary>
        public double Pollution
        {
            get
            {
                return _pollutionFactor * _pollution;
            }
        }

        /// <summary>
        /// intialises section. Retrieves the pollution factor from config file
        /// </summary>
        /// <param name="routeNo">route number that section is within</param>
        /// <param name="sectionNo">section number</param>
        public RouteSection(int routeNo, int sectionNo)
        {
            try
            {
                CreateConfigName(routeNo, sectionNo);
                _pollutionFactor = ReadSectionFactor();
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error initialising section", ex);
            }
        }

        /// <summary>
        /// Creates the string for the pollution factor name 
        /// </summary>
        /// <param name="routeNo">route number that section is within</param>
        /// <param name="sectionNo">section number</param>
        private void CreateConfigName(int routeNo, int sectionNo)
        {
            try
            {
                _configName = "Route" + routeNo + "Section" + sectionNo + "Factor";
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error creating pollution factor config name", ex);
            }            
        }

        /// <summary>
        /// Reads the section pollution factor from the config file
        /// </summary>
        /// <returns></returns>
        private double ReadSectionFactor()
        {
            try
            {
                return ConfigReader.GetDoubleAppSetting(_configName);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error reading section pollution factor from config file", ex);
            }            
        }

        /// <summary>
        /// Adjucts the pollution of a section
        /// </summary>
        /// <param name="pollution">value to change pollution by (vector)</param>
        public void AdjustPollution(double pollution)
        {
            try
            {
                _pollution += pollution;
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error adjusting pollution for section", ex);
            }            
        }

        /// <summary>
        /// Changes the pollution factor
        /// </summary>
        /// <param name="config">configuration dictionary containing new pollution factor</param>
        public void AdjustPollutionFactor(Dictionary<string, string> config)
        {
            try
            {
                _pollutionFactor = Convert.ToDouble(config[_configName]);
            }
            catch (Exception ex)
            {
                throw new JunctionException("Error Adjusting pollution factor", ex);
            }            
        }
    }
}
