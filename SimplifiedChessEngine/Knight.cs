using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplifiedChessEngine
{
    public class Knight : ChessPiece
    {
        public Knight()
        {
            
        }

        public Knight(Knight orig)
        {
            AvailableMoves = new List<ChessMove>();
            Color = orig.Color;
        }

        public override List<Tuple<int, int>> MovePattern
        {
            get
            {
                return new List<Tuple<int, int>>
                {
                    new Tuple<int, int>(1, 2),
                    new Tuple<int, int>(1, -2),
                    new Tuple<int, int>(-1, 2),
                    new Tuple<int, int>(-1,-2),
                    new Tuple<int, int>(2, 1),
                    new Tuple<int, int>(2, -1),
                    new Tuple<int, int>(-2, 1),
                    new Tuple<int, int>(-2, -1)
                };
            }
        }

        public override List<ChessMove> GetAvailableMoves(Cell currentCell, ChessBoard board, bool protectQueen = true)
        {
            AvailableMoves = base.GetAvailableMoves(MovePattern, currentCell, board, protectQueen);
            return AvailableMoves;
        }

        public override ChessPiece Clone()
        {
            return new Knight(this);
        }
    }
}