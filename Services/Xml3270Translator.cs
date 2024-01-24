using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services
{
    public class Xml3270Translator : I3270Translator<XElement>
    {
        public XElement Translate(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}
