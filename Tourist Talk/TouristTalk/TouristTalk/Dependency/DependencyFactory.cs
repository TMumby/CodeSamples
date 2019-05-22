using System;
using Unity;
using TouristTalk.Exceptions;

namespace TouristTalk.Dependency
{
    public static class DependencyFactory
    {
        private static UnityContainer _container;

        /// <summary>
        /// Constructor, calls to build dependencies
        /// </summary>
        static DependencyFactory()
        {
            try
            {
                BuildDependancies();
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error initialising dependency factory", e);
            }            
        }

        /// <summary>
        /// Registers different dependencies into unity container
        /// </summary>       
        private static void BuildDependancies()
        {
            try
            {
                _container = new UnityContainer();
                _container.RegisterType<IDataConnect, DataConnect.DataConnect>();

            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error building unity container", e);
            }
        }

        /// <summary>
        /// Resolves IDBConnect with class in unity container
        /// </summary>
        /// <returns>database class</returns>
        public static IDataConnect ResolveDataConnect()
        {
            try
            {
                IDataConnect dataConnect = _container.Resolve<DataConnect.DataConnect>();
                return dataConnect;
            }
            catch (Exception e)
            {
                throw new TouristTalkException("Error resolving IDBConnect dependancy", e);
            }
        }

    }
}