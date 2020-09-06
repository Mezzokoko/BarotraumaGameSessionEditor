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

        private XmlNode MultiplayerCampaign;
        private XmlNode MetaData;
        private XmlNode CampaignMap;

        private XmlAttributeProperty MoneyAttribute;

        public List<BarotraumaReputation> Reputations = new List<BarotraumaReputation>();

        public List<BarotraumaLocation> Locations = new List<BarotraumaLocation>();
        public List<BarotraumaLocationConnection> Connections = new List<BarotraumaLocationConnection>();

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

            MoneyAttribute = new XmlAttributeProperty(MultiplayerCampaign, "money");

            //Sort out Metadata
            MetaData = XmlHelpers.GetSingleNodeFromName(MultiplayerCampaign, "Metadata");

            foreach (XmlNode Child in MetaData.ChildNodes)
            {
                XmlAttribute KeyAttribute = XmlHelpers.GetAttributeFromName(Child, "key");

                if (KeyAttribute.Value.Split('.')[0] == "reputation")
                {
                    Reputations.Add(new BarotraumaReputation(this, Child));
                }
            }

            //Sort out Map Objects
            CampaignMap = XmlHelpers.GetSingleNodeFromName(MultiplayerCampaign, "map");

            foreach (XmlNode Child in CampaignMap.ChildNodes)
            {
                if (Child.Name == "location")
                {
                    BarotraumaLocation NewLocation = new BarotraumaLocation(this, Child);

                    int Index = NewLocation.LocationIndex;

                    int HighestIndex = Locations.Count;

                    for (int i = HighestIndex; i < Index + 1; i++)
                    {
                        Locations.Add(null);
                    }

                    Locations[Index] = NewLocation;
                }

                if (Child.Name == "connection")
                {
                    Connections.Add(new BarotraumaLocationConnection(this, Child));
                }
            }
        }

        public static T Lerp<T>(T A, T B, float Alpha)
        {
            dynamic _A = A;
            dynamic _B = B;

            return (T)(Alpha * (_B - _A)) + _A;
        }

        private BarotraumaReputation GetReputationWithFaction(BarotraumaFaction Faction)
        {
            if (Faction == BarotraumaFaction.location)
            {
                return null;
            }

            foreach (BarotraumaReputation R in Reputations)
            {
                if (R.Faction == Faction)
                {
                    return R;
                }
            }

            return null;
        }

        private BarotraumaReputation GetReputationWithLocation(int LocationIndex)
        {
            foreach (BarotraumaReputation R in Reputations)
            {
                if (R.Faction == BarotraumaFaction.location && R.LocationIndex == LocationIndex)
                {
                    return R;
                }
            }

            return null;
        }

        public int GetReputationValueWithFaction(BarotraumaFaction Faction)
        {
            BarotraumaReputation R = GetReputationWithFaction(Faction);

            if (R == null)
            {
                return 0;
            }

            return R.ReputationValue;
        }

        public int GetReputationValueWithLocation(int LocationIndex)
        {
            BarotraumaReputation R = GetReputationWithLocation(LocationIndex);

            if (R == null)
            {
                return 0;
            }

            return R.ReputationValue;
        }

        public void SetReputationWithLocation(int LocationIndex, int Value)
        {
            BarotraumaReputation R = GetReputationWithLocation(LocationIndex);

            if (R != null)
            {
                R.ReputationValue = Value;
                return;
            }

            //TODO: make new reputation object
        }

        public void SetReputationWithFaction(BarotraumaFaction Faction, int Value)
        {
            BarotraumaReputation R = GetReputationWithFaction(Faction);

            if (R != null)
            {
                R.ReputationValue = Value;
                return;
            }

            //TODO: make new reputation object
        }

        public void SetDifficultyBounds(float LowerBound, float HigherBound)
        {
            float DifficultyScale = (HigherBound - LowerBound) / 100.0f;

            foreach (BarotraumaLocationConnection C in Connections)
            {
                C.Difficulty = LowerBound + (C.Difficulty * DifficultyScale);
            }
        }

        public void ScalePricesWithDifficulty(float PriceScaleAtFullDifficulty, float PriceScaleAtZeroDifficulty = 1)
        {
            foreach (BarotraumaLocation L in Locations)
            {
                float PriceScale = Lerp<float>(PriceScaleAtZeroDifficulty, PriceScaleAtFullDifficulty, L.GetAverageLocationDifficulty());

                L.TradePriceMultiplier = PriceScale;
                L.RepairPriceMultiplier = PriceScale;
            }
        }

        public void ScaleLevelSizeWithDifficulty(Vector2D ScaleAtFullDifficulty)
        {
            foreach (BarotraumaLocationConnection C in Connections)
            {
                Vector2D CurrentLevelSize = new Vector2D(C.LevelSize.A, C.LevelSize.B);

                CurrentLevelSize = Lerp<Vector2D>(CurrentLevelSize, CurrentLevelSize * ScaleAtFullDifficulty, C.Difficulty);

                C.LevelSize = new IntTuple((int)CurrentLevelSize.X, (int)CurrentLevelSize.Y);
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
