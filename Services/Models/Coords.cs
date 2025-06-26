using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Util;

namespace Services.Models
{
    public readonly record struct Coords : IComparable<Coords>
    {
        public int Row { get; }
        public int Col { get; }

        public Coords() : this(0, 0)
        {
        }

        public Coords((int Row, int Col) tuple) : this(tuple.Row, tuple.Col)
        {
        }

        public Coords(int bufferAddress) : this(BinaryUtil.AddressCoordinates(bufferAddress))
        {
        }

        public Coords(int row, int col)
        {
            if (row < 0 || row >= Telnet.SCREEN_HEIGHT)
            {
                throw new ArgumentOutOfRangeException($"Row cannot be {row}");
            }

            if (col < 0 || col >= Telnet.SCREEN_WIDTH)
            {
                throw new ArgumentOutOfRangeException($"Col cannot be {col}");
            }

            Row = row;
            Col = col;
        }

        public Coords Increment()
        {
            var nextCol = (this.Col + 1) % Telnet.SCREEN_WIDTH;
            var nextRow = (nextCol == 0)
                ? (this.Row + 1) % Telnet.SCREEN_HEIGHT
                : this.Row;

            return new Coords(nextRow, nextCol);
        }

        public int CompareTo(Coords other)
        {
            var rowResult = this.Row.CompareTo(other.Row);
            return rowResult != 0 ? rowResult : this.Col.CompareTo(other.Col);
        }

        public static bool operator <(Coords left, Coords right) => left.CompareTo(right) < 0;
        public static bool operator <=(Coords left, Coords right) => left.CompareTo(right) <= 0;
        public static bool operator >(Coords left, Coords right) => left.CompareTo(right) > 0;
        public static bool operator >=(Coords left, Coords right) => left.CompareTo(right) >= 0;
    }
}
