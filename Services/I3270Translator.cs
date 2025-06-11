using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Defines an interface for translation from raw 3270 bytes and attributes
    /// to <code>T</code> as defined by a concrete, implementing class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface I3270Translator<out T>
    {
        /// <summary>
        /// Performs the translation of 3270 bytes and attributes to the concrete type <code>T</code>
        /// </summary>
        /// <param name="buffer">the raw row bytes from the 3270 service</param>
        /// <param name="attributeSet">3270 attributes attached to the bytes of the row</param>
        /// <param name="route66AttributeSet">custom non-standard attributes attached to the bytes of the row</param>
        /// <returns></returns>
        public T Translate(
            byte[] buffer,
            IDictionary<byte, byte>[] attributeSet,
            IDictionary<string, object>[] route66AttributeSet,
            int cursor);
    }
}
