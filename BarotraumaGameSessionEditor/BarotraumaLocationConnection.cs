﻿using System;
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

    public class BarotraumaLocationConnection : BarotraumaXmlObject
    {
        private XmlAttributeProperty DifficultyAttribute;
        private XmlAttributeProperty DifficultyAttributeLevel;
        private XmlAttributeProperty BiomeAttribute;
        private XmlAttributeProperty BiomeAttributeLevel;
        private XmlAttributeProperty ConnectedLocationsAttribute;
        private XmlAttributeProperty LevelSizeAttribute;
        private XmlAttributeProperty GenerationParamsAttribute;

        public BarotraumaLocationConnection(BarotraumaGameSession ParentSession, XmlNode ObjectNode) : base(ParentSession, ObjectNode)
        {
            XmlNode LevelNode = XmlHelpers.GetSingleNodeFromName(ObjectNode, "Level");

            DifficultyAttribute = new XmlAttributeProperty(ObjectNode, "difficulty");
            DifficultyAttributeLevel = new XmlAttributeProperty(LevelNode, "difficulty");
            BiomeAttribute = new XmlAttributeProperty(ObjectNode, "biome");
            BiomeAttributeLevel = new XmlAttributeProperty(LevelNode, "biome");
            ConnectedLocationsAttribute = new XmlAttributeProperty(ObjectNode, "locations");
            LevelSizeAttribute = new XmlAttributeProperty(LevelNode, "size");
            GenerationParamsAttribute = new XmlAttributeProperty(LevelNode, "generationparams");
        }

        public float Difficulty
        {
            get => DifficultyAttribute.FloatValue;
            set
            {
                XmlAttribute DifficultyBackupAttribute;

                string DifficultyBackupKey = "difficulty-backup";

                DifficultyBackupAttribute = XmlHelpers.GetAttributeFromName(ObjectNode, DifficultyBackupKey);

                if (DifficultyBackupAttribute == null)
                {
                    DifficultyBackupAttribute = ObjectNode.OwnerDocument.CreateAttribute(DifficultyBackupKey);
                    DifficultyBackupAttribute.Value = DifficultyAttribute.StringValue;
                    ObjectNode.Attributes.Append(DifficultyBackupAttribute);
                }

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

        public IntTuple ConnectedLocations
        {
            get => ConnectedLocationsAttribute.TupleValue;
            set => ConnectedLocationsAttribute.TupleValue = value;
        }

        public IntTuple LevelSize
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
