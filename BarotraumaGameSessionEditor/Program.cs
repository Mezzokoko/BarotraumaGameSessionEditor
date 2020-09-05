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

            foreach (BarotraumaReputation Reputation in Session.Reputations)
            {
                Console.WriteLine(Reputation.Faction + " " + Reputation.LocationIndex + " " + Reputation.ReputationValue);
                Reputation.ReputationValue = 100;
            }

            Session.SaveToFile("newxml.xml");

            Console.ReadLine();
        }
    }
}
