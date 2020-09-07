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

        private static XmlNode GetNewXmlReputationNode(BarotraumaGameSession ParentSession, string KeyValue, int ReputationValue)
        {
            XmlNode NewNode = XmlHelpers.AddSubNode(ParentSession.MetaData, "Data");

            XmlHelpers.SetNodeAttribute(NewNode, "key", KeyValue);
            XmlHelpers.SetNodeAttribute(NewNode, "value", ReputationValue.ToString());
            XmlHelpers.SetNodeAttribute(NewNode, "type", "System.Single");

            return NewNode;
        }

        //I know this is awful to read, blame C# for being fucking stupid about constructor overloading
        public BarotraumaReputation(BarotraumaGameSession ParentSession, BarotraumaFaction Faction, int ReputationValue)
            : this (ParentSession, GetNewXmlReputationNode(ParentSession, "reputation.faction." + Faction.ToString(), ReputationValue))
        {
            System.Diagnostics.Debug.Assert(Faction != BarotraumaFaction.location);
        }

        public BarotraumaReputation(BarotraumaGameSession ParentSession, int LocationIndex, int ReputationValue)
            : this(ParentSession, GetNewXmlReputationNode(ParentSession, "reputation.location." + LocationIndex, ReputationValue)) {}

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
