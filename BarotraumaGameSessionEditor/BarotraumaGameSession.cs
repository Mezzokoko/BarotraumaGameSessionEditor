using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.IO;
using System.Collections;

using BarotraumaGameSessionEditor;

namespace BarotraumaGameSessionEditor
{
    public class BarotraumaGameSession
    {
        private string FileName;

        private XmlDocument GameSessionDocument;
        private XmlNode GameSession;

        private XmlAttributeProperty MoneyAttribute;

        public XmlNode MultiplayerCampaign;
        public XmlNode MetaData;
        public XmlNode CampaignMap;

        public ArrayList Locations = new ArrayList();

        public int Money
        {
            get => MoneyAttribute.IntegerValue;
            set => MoneyAttribute.IntegerValue = value;
        }

        public BarotraumaGameSession(string FileName)
        {
            //Initialize XML doc
            this.FileName = FileName;

            GameSessionDocument = new XmlDocument();

            System.IO.StreamReader DocStreamReader = new StreamReader(FileName);

            GameSessionDocument.Load(DocStreamReader);

            //Set up various pointers
            GameSession = GameSessionDocument.GetElementsByTagName("Gamesession")[0];

            MultiplayerCampaign = XmlHelpers.GetSingleNodeFromName(GameSession, "MultiPlayerCampaign");

            MetaData = XmlHelpers.GetSingleNodeFromName(MultiplayerCampaign, "Metadata");
            CampaignMap = XmlHelpers.GetSingleNodeFromName(MultiplayerCampaign, "map");

            MoneyAttribute = new XmlAttributeProperty(MultiplayerCampaign, "money");

            //Collect Locations in Array            
            XmlNodeList MapObjects = CampaignMap.ChildNodes;

            foreach (XmlNode Child in MapObjects)
            {
                if (Child.Name == "location")
                {
                    BarotraumaLocation NewLocation = new BarotraumaLocation(Child);

                    int Index = NewLocation.Index;

                    int HighestIndex = Locations.Count;

                    for (int i = HighestIndex; i < Index + 1; i++)
                    {
                        Locations.Add(null);
                    }

                    Locations[Index] = NewLocation;
                }
            }
        }

        public void SaveToFile(string FileName = null)
        {
            if (FileName == null)
            {
                FileName = this.FileName;
            }

            GameSessionDocument.Save(new StreamWriter(FileName));
        }
    }
}
