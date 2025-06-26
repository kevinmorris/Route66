using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IGridHandler<T>
    {
        /// <summary>
        /// Fired when the grid's buffer has been translated to <code>T</code>
        /// </summary>
        public event EventHandler<GridUpdateEventArgs<T>> GridUpdated;

        /// <summary>
        /// Clears this grid's buffer.  All characters are set to zero.
        /// </summary>
        public void Reset();

        /// <summary>
        /// Starts the translation of this grid's buffer bytes to <code>T</code>
        /// if the Dirty flag is set.  The row then fires the RowUpdated event with
        /// the translation 
        /// </summary>
        public void Update(bool force = false);

        /// <summary>
        /// Sets a single character within the display buffer
        /// </summary>
        /// <param name="row">the row (y coordinate) of the character to be set</param>
        /// <param name="col">the column (x coordinate) of the character to be set</param>
        /// <param name="d">the character to be set</param>
        public void SetCharacter(int row, int col, byte d);

        public void ResetCursor();

        public int Cursor { get; }
        /// <summary>
        /// Sets the location of the display's "cursor".  This can mean different things
        /// depending on how the bytes are rendered.
        /// </summary>
        /// <param name="col">the index or column (x coordinate) of the cursor</param>
        /// 
        public void SetCursor(int row, int col);

        /// <summary>
        /// Sets an extended attribute on this grid.  The extended field attribute provides additional field definition beyond that
        /// provided by the field attribute.The extended field attribute defines field
        /// characteristics such as color, character set, field validation, field outlining, and
        /// extended highlighting.  This is also where the <code>TN3270Service</code> will set the
        /// main 3270 field attribute that defines a field.
        /// </summary>
        /// <param name="row">the row (y coordinate) of the attribute</param>
        /// <param name="col">the column (x coordinate) of the attribute</param>
        /// <param name="attrKey">the key of the attribute</param>
        /// <param name="attrValue">the value of the attribute</param>
        public void SetExtendedAttribute(int row, int col, byte attrKey, byte attrValue);

        /// <summary>
        /// Sets a custom attribute on this grid.  These attributes are not part of the
        /// 3270 spec.  Instead, these attributes are domain specific to Route66.
        /// </summary>
        /// <param name="row">the row (y coordinate) of the attribute</param>
        /// <param name="col">the column (x coordinate) of the attribute</param>
        /// <param name="attrKey">the key of the attribute</param>
        /// <param name="attrValue">the value of the attribute</param>
        public void SetRoute66Attribute(int row, int col, string attrKey, object attrValue);
    }
}
