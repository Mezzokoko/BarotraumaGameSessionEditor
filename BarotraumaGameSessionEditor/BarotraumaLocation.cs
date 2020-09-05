using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace BarotraumaGameSessionEditor
{
    public enum BarotraumaLocationType
    {
        None,
        Outpost,
        Mine,
        Research,
        City,
        Military,
        Lair,
        EndLocation,
        Random,
        RandomButNotEndLocation
    }

    public class BarotraumaLocation
    {
        static Random RNG = new Random(Convert.ToInt32(DateTime.UtcNow.Ticks % Int32.MaxValue));

        XmlNode LocationNode;

        private XmlAttributeProperty IndexAttribute;
        private XmlAttributeProperty LocationTypeAttribute;
        private XmlAttributeProperty MapLocationAttribute;
        private XmlAttributeProperty DepthAttribute;
        private XmlAttributeProperty TradePriceMultiplierAttribute;
        private XmlAttributeProperty RepairPriceMultiplierAttribute;

        public BarotraumaLocation(XmlNode LocationNode)
        {
            this.LocationNode = LocationNode;

            IndexAttribute = new XmlAttributeProperty(LocationNode, "i");
            LocationTypeAttribute = new XmlAttributeProperty(LocationNode, "type");
            MapLocationAttribute = new XmlAttributeProperty(LocationNode, "position");
            DepthAttribute = new XmlAttributeProperty(LocationNode, "normalizeddepth");
            TradePriceMultiplierAttribute = new XmlAttributeProperty(LocationNode, "pricemultiplier");
            RepairPriceMultiplierAttribute = new XmlAttributeProperty(LocationNode, "mechanicalpricemultipler");
        }

        public int Index
        {
            set => IndexAttribute.IntegerValue = value;
            get => IndexAttribute.IntegerValue;
        }

        public BarotraumaLocationType LocationType
        {
            set => LocationTypeAttribute.LocationTypeValue = value;
            get => LocationTypeAttribute.LocationTypeValue;
        }

        public Vector2D MapLocation
        {
            get => MapLocationAttribute.VectorValue;
            set => MapLocationAttribute.VectorValue = value;
        }

        public float Depth
        {
            get => DepthAttribute.FloatValue;
            set => DepthAttribute.FloatValue = value;
        }

        public float TradePriceMultiplier
        {
            get => TradePriceMultiplierAttribute.FloatValue;
            set => TradePriceMultiplierAttribute.FloatValue = value;
        }

        public float RepairPriceMultiplier
        {
            get => RepairPriceMultiplierAttribute.FloatValue;
            set => RepairPriceMultiplierAttribute.FloatValue = value;
        }

        public static string GetLocationTypeString(BarotraumaLocationType Type)
        {
            Func<BarotraumaLocationType, BarotraumaLocationType> RandomLocation = (BarotraumaLocationType UpperBound) =>
            {
                return (BarotraumaLocationType)(RNG.Next() % (int)UpperBound);
            };

            if (Type == BarotraumaLocationType.Random)
            {
                return GetLocationTypeString(RandomLocation(BarotraumaLocationType.Random));
            }

            if (Type == BarotraumaLocationType.RandomButNotEndLocation)
            {
                return GetLocationTypeString(RandomLocation(BarotraumaLocationType.EndLocation));
            }

            return Type.ToString();
        }
    }
}
