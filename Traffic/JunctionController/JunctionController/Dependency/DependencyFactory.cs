using System;
using Unity;

namespace JunctionController
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
                throw new JunctionException("Error initialising dependency factory", e);
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
                _container.RegisterType<IDataConnect, DBConnect>();
            }
            catch (Exception e)
            {
                throw new JunctionException("Error building unity container", e);
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
                IDataConnect dataConnect = _container.Resolve<DBConnect>();
                return dataConnect;
            }
            catch (Exception e)
            {
                throw new JunctionException("Error resolving IDBConnect dependancy", e);
            }
        }
    }
}
