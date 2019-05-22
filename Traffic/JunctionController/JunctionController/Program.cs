namespace JunctionController
{
    using System;
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Junction.Start();
                HttpPostListener newCar = new HttpPostListener();
            }
            catch (JunctionException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Exception encountered, press any key to exit");
                Console.ReadLine();
            }
        }
    }
}
