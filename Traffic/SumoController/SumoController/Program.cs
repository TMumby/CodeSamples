using System;
using SumoController.Traci;

namespace SumoController
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Connection sumoConnect = new Connection(); //connect to sumo  
                Simulation simulation = new Simulation(sumoConnect);
                simulation.Simulate(); //start simulation  
            }
            catch(SumoControllerException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Exception encountered, press any key to exit");
                Console.ReadLine();
            }
          
        }
    }
}
