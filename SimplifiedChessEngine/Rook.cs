using System;
using System.Collections.Generic;

namespace SimplifiedChessEngine
{
    public class Rook : ChessPiece
    {
        public Rook()
        {
            
        }

        public Rook(Rook orig)
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
                    new Tuple<int, int>(1, 0),
                    new Tuple<int, int>(-1, 0),
                    new Tuple<int, int>(0, 1),
                    new Tuple<int, int>(0,-1),
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
            return new Rook(this);
        }
    }
}