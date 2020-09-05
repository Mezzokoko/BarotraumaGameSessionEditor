using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace BarotraumaGameSessionEditor
{
    public enum BarotraumaBiome
    {
        theaphoticplateau,
        europanridge,
        coldcaverns,
        thegreatvoid,
        hydrothermalwastes,
        endzone
    }

    public enum BarotraumaGenerationParams
    {
        outpostlevel,
        coldcavernsmaze,
        ridgebasic,
        voidbasic,
        coldcavernsbasic,
        plateaubasic,
        wastesbasic,
        endlocationbasic
    }

    public class BarotraumaLocationConnection
    {
        XmlNode ConnectionNode;

        private XmlAttributeProperty DifficultyAttribute;
        private XmlAttributeProperty DifficultyAttributeLevel;
        private XmlAttributeProperty BiomeAttribute;
        private XmlAttributeProperty BiomeAttributeLevel;
        private XmlAttributeProperty ConnectedLocationsAttribute;
        private XmlAttributeProperty LevelSizeAttribute;
        private XmlAttributeProperty GenerationParamsAttribute;

        public BarotraumaLocationConnection(XmlNode ConnectionNode)
        {
            this.ConnectionNode = ConnectionNode;

            XmlNode LevelNode = XmlHelpers.GetSingleNodeFromName(ConnectionNode, "Level");

            DifficultyAttribute = new XmlAttributeProperty(ConnectionNode, "difficulty");
            DifficultyAttributeLevel = new XmlAttributeProperty(LevelNode, "difficulty");
            BiomeAttribute = new XmlAttributeProperty(ConnectionNode, "biome");
            BiomeAttributeLevel = new XmlAttributeProperty(LevelNode, "biome");
            ConnectedLocationsAttribute = new XmlAttributeProperty(ConnectionNode, "locations");
            LevelSizeAttribute = new XmlAttributeProperty(LevelNode, "size");
            GenerationParamsAttribute = new XmlAttributeProperty(LevelNode, "generationparams");
        }

        public float Difficulty
        {
            get => DifficultyAttribute.FloatValue;
            set
            {
                DifficultyAttribute.FloatValue = value;
                DifficultyAttributeLevel.FloatValue = value;
            }
        }

        public BarotraumaBiome Biome
        {
            get => BiomeAttribute.BiomeValue;
            set
            {
                BiomeAttribute.BiomeValue = value;
                BiomeAttributeLevel.BiomeValue = value;
            }
        }

        public Tuple<int, int> ConnectedLocations
        {
            get => ConnectedLocationsAttribute.TupleValue;
            set => ConnectedLocationsAttribute.TupleValue = value;
        }

        public Tuple<int, int> LevelSize
        {
            get => LevelSizeAttribute.TupleValue;
            set => LevelSizeAttribute.TupleValue = value;
        }

        public BarotraumaGenerationParams GenerationParams
        {
            get => GenerationParamsAttribute.GenerationParamsValue;
            set => GenerationParamsAttribute.GenerationParamsValue = value;
        }
    }
}
