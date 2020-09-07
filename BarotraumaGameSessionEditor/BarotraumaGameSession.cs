using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.IO;
using System.Collections;

using System.Drawing;

using BarotraumaGameSessionEditor;

namespace BarotraumaGameSessionEditor
{
    public class BarotraumaGameSession
    {
        private string FileName;

        private XmlDocument GameSessionDocument;
        private XmlNode GameSession;

        private XmlNode MultiplayerCampaign;
        public XmlNode MetaData;
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

            R = new BarotraumaReputation(this, LocationIndex, Value);
            Reputations.Add(R);
        }

        public void SetReputationWithFaction(BarotraumaFaction Faction, int Value)
        {
            BarotraumaReputation R = GetReputationWithFaction(Faction);

            if (R != null)
            {
                R.ReputationValue = Value;
                return;
            }

            R = new BarotraumaReputation(this, Faction, Value);
            Reputations.Add(R);
        }

        public void ResetAllDifficulties()
        {
            foreach (BarotraumaLocationConnection C in Connections)
            {
                C.ResetDifficulty();
            }
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

        public void SaveToImage(float CanvasScale = 1, float RenderScale = 8, int Margin = 100, string FileName = null)
        {
            if (FileName == null)
            {
                FileName = this.FileName + "_Image.png";
            }

            //Determine Map Dimensions
            Vector2D InitLocation = Locations[0].MapLocation;

            Vector2D MinPoint = InitLocation;
            Vector2D MaxPoint = InitLocation;

            foreach (BarotraumaLocation L in Locations)
            {
                Vector2D CurrentMapLocation = L.MapLocation;

                MinPoint.X = Math.Min(MinPoint.X, CurrentMapLocation.X);
                MinPoint.Y = Math.Min(MinPoint.Y, CurrentMapLocation.Y);
                MaxPoint.X = Math.Max(MaxPoint.X, CurrentMapLocation.X);
                MaxPoint.Y = Math.Max(MaxPoint.Y, CurrentMapLocation.Y);
            }

            //Determine Canvas Size and Margin
            Vector2D MapDimensions = MaxPoint - MinPoint;

            int CanvasWidth = (int)Math.Ceiling((CanvasScale * ((2 * Margin) + MapDimensions.X)));
            int CanvasHeight = (int)Math.Ceiling((CanvasScale * ((2 * Margin) + MapDimensions.Y)));

            int RenderWidth = (int)Math.Ceiling(CanvasWidth * RenderScale);
            int RenderHeight = (int)Math.Ceiling(CanvasHeight * RenderScale);

            //Setting up Render canvas and brushes
            Bitmap RenderCanvas = new Bitmap(RenderWidth, RenderHeight);
            Graphics Renderer = Graphics.FromImage(RenderCanvas);

            Renderer.Clear(Color.White);

            Func<Vector2D, Point> MapLocationToRenderCanvas = MapLocation =>
            {
                Vector2D TransformedLocation = MapLocation;
                TransformedLocation -= MinPoint;
                TransformedLocation += new Vector2D(Margin, Margin);
                TransformedLocation *= CanvasScale * RenderScale;

                return new Point((int)Math.Round(TransformedLocation.X), (int)Math.Round(TransformedLocation.Y));
            };

            Func<Point, int, Rectangle> GetSquare = (P, Width) =>
            {
                int HalfWidth = Width / 2;

                P.X -= HalfWidth;
                P.Y -= HalfWidth;

                return new Rectangle(P, new Size(Width, Width));
            };

            Func<Color, Color, float, Color> LerpColor = (C1, C2, Alpha) =>
            {
                int R = Lerp<int>(C1.R, C2.R, Alpha);
                int G = Lerp<int>(C1.G, C2.G, Alpha);
                int B = Lerp<int>(C1.B, C2.B, Alpha);

                return Color.FromArgb(R, G, B);
            };

            Pen RenderPen = new Pen(Color.Black, 5 * CanvasScale * RenderScale);
            Brush RenderBrush = new SolidBrush(Color.Black);

            int CircleWidth = (int)(40 * CanvasScale * RenderScale);

            //Drawing Elements
            foreach (BarotraumaLocationConnection C in Connections)
            {
                Color A, B;

                bool LowDifficulty = C.Difficulty < 50;

                A = LowDifficulty ? Color.Green : Color.Yellow;
                B = LowDifficulty ? Color.Yellow : Color.Red;

                RenderPen.Color = LerpColor(A, B, (C.Difficulty - (LowDifficulty ? 0 : 50)) / 50);

                Point P1 = MapLocationToRenderCanvas(Locations[C.ConnectedLocations.A].MapLocation);
                Point P2 = MapLocationToRenderCanvas(Locations[C.ConnectedLocations.B].MapLocation);

                Renderer.DrawLine(RenderPen, P1, P2);
            }

            foreach (BarotraumaLocation L in Locations)
            {
                Point P_Circle = MapLocationToRenderCanvas(L.MapLocation);
                Point P_Label = MapLocationToRenderCanvas(L.MapLocation + new Vector2D(0, 20));

                Font StringFont = new Font(FontFamily.GenericSerif, 10 * CanvasScale * RenderScale);
                string LocationString = L.LocationIndex.ToString() + ": " + L.LocationName + "\n";
                LocationString += "Type: " + L.LocationType + "\n";

                P_Label.X -= (int)(Renderer.MeasureString(LocationString, StringFont).Width / 2);

                Renderer.FillEllipse(RenderBrush, GetSquare(P_Circle, CircleWidth));
                Renderer.DrawString(LocationString, StringFont, RenderBrush, P_Label);

            }

            //Scaling down to canvas size and saving
            Bitmap FinalCanvas = new Bitmap(CanvasWidth, CanvasHeight);
            Graphics FinalRenderer = Graphics.FromImage(FinalCanvas);

            FinalRenderer.Clear(Color.White);
            FinalRenderer.DrawImage(RenderCanvas, 0, 0, CanvasWidth, CanvasHeight);

            FinalCanvas.Save(FileName, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
