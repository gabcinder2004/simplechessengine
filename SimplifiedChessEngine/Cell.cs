using System;
using System.Runtime.Remoting.Messaging;

namespace SimplifiedChessEngine
{
    public class Cell : ICloneable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ChessPiece Piece { get; set; }

        public string Position
        {
            get { return ChessUtility.NumberToLetter[X] + Y; }
        }

        public Cell(ChessPiece piece, int x, int y)
        {
            Piece = piece;
            X = x;
            Y = y;
        }

        public bool IsEmpty()
        {
            return Piece == null;
        }

        public override string ToString()
        {
            return Position;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Cell;

            if (other != null && (X == other.X && Y == other.Y))
            {
                return true;
            }

            return false;
        }

        protected bool Equals(Cell other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ (Piece != null ? Piece.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            return new Cell(Piece == null ? null : Piece.Clone(), X, Y);
        }
    }
}