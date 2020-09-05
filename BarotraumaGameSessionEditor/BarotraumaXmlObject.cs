using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace BarotraumaGameSessionEditor
{
    public abstract class BarotraumaXmlObject
    {
        protected BarotraumaGameSession ParentSession;
        protected XmlNode ObjectNode;

        public BarotraumaXmlObject(BarotraumaGameSession ParentSession, XmlNode ObjectNode)
        {
            this.ParentSession = ParentSession;
            this.ObjectNode = ObjectNode;
        }
    }
}
