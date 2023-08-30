using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CdrDecoder
{
    public class BcdBytes
    {
        public byte Value { get; set; }
        private readonly byte[] _valueAsBytes = new byte[1];
        public BcdBytes(byte value)
        {
            this.Value = value;
            this._valueAsBytes[0] = this.Value;
        }

        public override string ToString()
        {
            return BitConverter.ToString(this._valueAsBytes).Replace("-", "");
        }
    }
}
