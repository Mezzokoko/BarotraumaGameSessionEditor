﻿using System;
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

    public class BarotraumaLocation : BarotraumaMapObject
    {
        static Random RNG = new Random(Convert.ToInt32(DateTime.UtcNow.Ticks % Int32.MaxValue));

        private XmlAttributeProperty LocationIndexAttribute;
        private XmlAttributeProperty LocationTypeAttribute;
        private XmlAttributeProperty MapLocationAttribute;
        private XmlAttributeProperty DepthAttribute;
        private XmlAttributeProperty TradePriceMultiplierAttribute;
        private XmlAttributeProperty RepairPriceMultiplierAttribute;

        public BarotraumaLocation(BarotraumaGameSession ParentSession, XmlNode ObjectNode) : base(ParentSession, ObjectNode)
        {
            LocationIndexAttribute = new XmlAttributeProperty(ObjectNode, "i");
            LocationTypeAttribute = new XmlAttributeProperty(ObjectNode, "type");
            MapLocationAttribute = new XmlAttributeProperty(ObjectNode, "position");
            DepthAttribute = new XmlAttributeProperty(ObjectNode, "normalizeddepth");
            TradePriceMultiplierAttribute = new XmlAttributeProperty(ObjectNode, "pricemultiplier");
            RepairPriceMultiplierAttribute = new XmlAttributeProperty(ObjectNode, "mechanicalpricemultipler");
        }

        public int LocationIndex
        {
            set => LocationIndexAttribute.IntegerValue = value;
            get => LocationIndexAttribute.IntegerValue;
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

        public List<BarotraumaLocation> GetConnectedLocations()
        {
            List<BarotraumaLocation> Locations = new List<BarotraumaLocation>();

            foreach (BarotraumaLocationConnection Connection in ParentSession.Connections)
            {
                IntTuple ConnectionTuple = Connection.ConnectedLocations;

                if (!ConnectionTuple.Contains(LocationIndex))
                {
                    continue;
                }

                int OtherLocationIndex = ConnectionTuple.GetOther(LocationIndex);

                BarotraumaLocation OtherLocation = ParentSession.Locations[OtherLocationIndex];

                if (OtherLocation != null)
                {
                    Locations.Add(OtherLocation);
                }
            }

            return Locations;
        }
    }
}
