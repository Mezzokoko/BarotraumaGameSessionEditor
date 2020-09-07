using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace BarotraumaGameSessionEditor
{
    class XmlHelpers
    {
        public static XmlNode GetSingleNodeFromName(XmlNode ParentNode, string Name)
        {
            XmlNodeList Children = ParentNode.ChildNodes;

            foreach (XmlNode Child in Children)
            {
                if (Child.Name == Name)
                {
                    return Child;
                }
            }

            System.Diagnostics.Debug.Assert(false);
            return null;
        }

        public static XmlAttribute GetAttributeFromName(XmlNode ParentNode, string Name)
        {
            XmlAttributeCollection Children = ParentNode.Attributes;

            foreach (XmlAttribute Child in Children)
            {
                if (Child.Name == Name)
                {
                    return Child;
                }
            }

            return null;
        }

        public static XmlNode AddSubNode(XmlNode ParentNode, string NodeName)
        {
            XmlNode NewNode = ParentNode.OwnerDocument.CreateElement(NodeName);
            ParentNode.AppendChild(NewNode);

            return NewNode;
        }

        public static void SetNodeAttribute(XmlNode ParentNode, string AttributeName, string AttributeValue)
        {
            XmlAttribute SelectedAttribtue = GetAttributeFromName(ParentNode, AttributeName);

            if (SelectedAttribtue != null)
            {
                SelectedAttribtue.Value = AttributeValue;
                return;
            }

            SelectedAttribtue = ParentNode.OwnerDocument.CreateAttribute(AttributeName);
            SelectedAttribtue.Value = AttributeValue;
            ParentNode.Attributes.Append(SelectedAttribtue);
        }
    }

    public class XmlAttributeProperty
    {
        XmlNode ParentNode;
        string Name;

        public XmlAttributeProperty(XmlNode ParentNode, string Name)
        {
            this.ParentNode = ParentNode;
            this.Name = Name;
        }

        public bool IsSet()
        {
            return XmlHelpers.GetAttributeFromName(ParentNode, Name) != null;
        }

        public void RemoveAttribute()
        {
            ParentNode.Attributes.Remove(XmlHelpers.GetAttributeFromName(ParentNode, Name));
        }

        public int IntegerValue
        {
            set => StringValue = value.ToString();
            get => Int32.Parse(StringValue);
        }

        public float FloatValue
        {
            set => StringValue = value.ToString();
            get => float.Parse(StringValue);
        }

        public string StringValue
        {
            set => XmlHelpers.SetNodeAttribute(ParentNode, Name, value);
            get => XmlHelpers.GetAttributeFromName(ParentNode, Name).Value;
        }

        public Vector2D VectorValue
        {
            set => StringValue = value.ToString();
            get => new Vector2D(StringValue);
        }

        public BarotraumaLocationType LocationTypeValue
        {
            set => StringValue = value.ToString();
            get => (BarotraumaLocationType)Enum.Parse(typeof(BarotraumaLocationType), StringValue);
        }

        public BarotraumaBiome BiomeValue
        {
            set => StringValue = value.ToString();
            get => (BarotraumaBiome)Enum.Parse(typeof(BarotraumaBiome), StringValue);
        }

        public BarotraumaGenerationParams GenerationParamsValue
        {
            set => StringValue = value.ToString();
            get => (BarotraumaGenerationParams)Enum.Parse(typeof(BarotraumaGenerationParams), StringValue);
        }

        public IntTuple TupleValue
        {
            set => StringValue = value.A.ToString() + "," + value.B.ToString();
            get
            {
                string[] SplitStrings = StringValue.Split(',');

                int A = Int32.Parse(SplitStrings[0]);
                int B = Int32.Parse(SplitStrings[1]);

                return new IntTuple(A, B);
            }
        }
    }
}
