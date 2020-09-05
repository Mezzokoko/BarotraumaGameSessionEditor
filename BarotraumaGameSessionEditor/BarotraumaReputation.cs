using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace BarotraumaGameSessionEditor
{
    public enum BarotraumaFaction
    {
        location,
        coalition,
        separatists,
        huskcult,
        clowns,
    }

    public class BarotraumaReputation : BarotraumaXmlObject
    {
        private XmlAttributeProperty KeyAttribute;
        private XmlAttributeProperty ReputationValueAttribute;

        public BarotraumaReputation(BarotraumaGameSession ParentSession, XmlNode ObjectNode) : base(ParentSession, ObjectNode)
        {
            KeyAttribute = new XmlAttributeProperty(ObjectNode, "key");
            ReputationValueAttribute = new XmlAttributeProperty(ObjectNode, "value");
        }

        public BarotraumaFaction Faction
        {
            get
            {
                string[] SplitKeys = KeyAttribute.StringValue.Split('.');

                if (SplitKeys[1] == "location")
                {
                    return BarotraumaFaction.location;
                }

                return (BarotraumaFaction)Enum.Parse(typeof(BarotraumaFaction), SplitKeys[2]);
            }
        }

        public int LocationIndex
        {
            get
            {
                if (Faction != BarotraumaFaction.location)
                {
                    return -1;
                }

                return Int32.Parse(KeyAttribute.StringValue.Split('.')[2]);
            }
        }

        public int ReputationValue
        {
            get => ReputationValueAttribute.IntegerValue;
            set => ReputationValueAttribute.IntegerValue = value;
        }
    }
}
