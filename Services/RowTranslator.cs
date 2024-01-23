using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services
{
    public class RowTranslator
    {
        public event EventHandler<RowUpdateEventArgs<XElement>> RowUpdated;

        private int _i = 0;

        public RowTranslator()
        {
            var timer = new Timer((state) =>
            {
                Update([]);
            }, null, 0, 15000);
        }

        internal void Update(byte[] bytes)
        {
            var rowUpdated = RowUpdated;
            if (rowUpdated != null)
            {
                rowUpdated.Invoke(this, new RowUpdateEventArgs<XElement>()
                {
                    Data = new XElement($"alpha{_i}")
                });

                _i += 1;
            }
        }
    }
}
