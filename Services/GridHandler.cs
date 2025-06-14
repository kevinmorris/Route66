using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Models;
using Constants = Util.Constants;

namespace Services
{
    public class GridHandler<T> : IGridHandler<T>
    {
        private byte[] _buffer = new byte[Constants.SCREEN_WIDTH * Constants.SCREEN_HEIGHT];

        /// <summary>
        /// Fired when the row's buffer has been translated to <code>T</code>
        /// </summary>
        public event EventHandler<GridUpdateEventArgs<T>>? GridUpdated;


        /// <summary>
        /// Whether this grid data is "dirty" in need of retranslation.
        /// </summary>
        public bool Dirty { get; private set; } = false;

        private readonly IDictionary<byte, byte>[] _extendedFieldAttr =
            new IDictionary<byte, byte>[Constants.SCREEN_WIDTH * Constants.SCREEN_HEIGHT];

        private readonly IDictionary<string, object>[] _route66AttributeSet =
            new IDictionary<string, object>[Constants.SCREEN_WIDTH * Constants.SCREEN_HEIGHT];

        private int _cursor = -1;

        public int Cursor { get; private set; }

        private readonly I3270Translator<T> _translator;

        public GridHandler(I3270Translator<T> translator)
        {
            this._translator = translator;
            Reset();
        }

        public void Reset()
        {
            _buffer = new byte[Constants.SCREEN_WIDTH * Constants.SCREEN_HEIGHT];
            for (var i = 0; i < Constants.SCREEN_WIDTH * Constants.SCREEN_HEIGHT; i++)
            {
                _extendedFieldAttr[i] = new Dictionary<byte, byte>();
                _route66AttributeSet[i] = new Dictionary<string, object>();
            }
        }

        public void Update(bool force = false)
        {
            if (Dirty || force)
            {
                GridUpdated?.Invoke(this, new GridUpdateEventArgs<T>()
                {
                    Data = _translator.Translate(_buffer, _extendedFieldAttr, _route66AttributeSet, _cursor)
                });

                Dirty = false;
            }
        }

        public void SetCharacter(int row, int col, byte d)
        {
            var index = row * Constants.SCREEN_WIDTH + col;
            if (index < _buffer.Length)
            {
                Dirty = true;
                _buffer[index] = d;
                _extendedFieldAttr[index] = new Dictionary<byte, byte>();
                _route66AttributeSet[index] = new Dictionary<string, object>();
            }

        }

        public void ResetCursor()
        {
            _cursor = -1;
        }

        public void SetCursor(int row, int col)
        {
            _cursor = row * Constants.SCREEN_WIDTH + col;
        }

        /// <summary>
        /// Sets an extended attribute on this row.  The extended field attribute provides additional field definition beyond that
        /// provided by the field attribute.The extended field attribute defines field
        /// characteristics such as color, character set, field validation, field outlining, and
        /// extended highlighting.  This is also where the <code>TN3270Service</code> will set the
        /// main 3270 field attribute that defines a field.
        /// </summary>
        /// <param name="row">the row (y coordinate) of the attribute</param>
        /// <param name="col">the column (x coordinate) of the attribute</param>
        /// <param name="attrKey">the key of the attribute</param>
        /// <param name="attrValue">the value of the attribute</param>
        public void SetExtendedAttribute(int row, int col, byte attrKey, byte attrValue)
        {
            Dirty = true;
            var index = row * Constants.SCREEN_WIDTH + col;
            _extendedFieldAttr[index][attrKey] = attrValue;
        }

        /// <summary>
        /// Sets a custom attribute on this row.  These attributes are not part of the
        /// 3270 spec.  Instead, these attributes are domain specific to Route66.
        /// </summary>
        /// <param name="row">the row (y coordinate) of the attribute</param>
        /// <param name="col">the column (x coordinate) of the attribute</param>
        /// <param name="attrKey">the key of the attribute</param>
        /// <param name="attrValue">the value of the attribute</param>
        public void SetRoute66Attribute(int row, int col, string attrKey, object attrValue)
        {
            Dirty = true;
            var index = row * Constants.SCREEN_WIDTH + col;
            _route66AttributeSet[index][attrKey] = attrValue;
        }
    }
}
