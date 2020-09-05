using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarotraumaGameSessionEditor
{
    class Program
    {

        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            //Read Map locations and print
            BarotraumaGameSession Session = new BarotraumaGameSession("gamesession.xml");

            BarotraumaLocation Location = Session.Locations[45];

            foreach (BarotraumaLocation OtherLocation in Location.GetConnectedLocations())
            {
                Console.WriteLine(OtherLocation.LocationIndex);
            }

            Console.ReadLine();
        }
    }
}
