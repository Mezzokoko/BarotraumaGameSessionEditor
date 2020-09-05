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

            foreach (BarotraumaLocationConnection C in Session.Connections)
            {
                float NewStartingDifficulty = 70.0f;
                float NewEndDifficulty = 120.0f;

                float DifficultyScale = (NewEndDifficulty - NewStartingDifficulty) / 100.0f;

                C.Difficulty *= (1 - DifficultyScale);
                C.Difficulty += NewStartingDifficulty;
            }

            Session.SaveToFile("newxml.xml");

            Console.ReadLine();
        }
    }
}
