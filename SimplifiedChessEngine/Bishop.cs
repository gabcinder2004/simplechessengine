using System;
using System.Collections.Generic;

namespace SimplifiedChessEngine
{
    public class Bishop : ChessPiece
    {
        public Bishop()
        {
            
        }

        public Bishop(Bishop orig)
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
                    new Tuple<int, int>(1, 1),
                    new Tuple<int, int>(1, -1),
                    new Tuple<int, int>(-1, 1),
                    new Tuple<int, int>(-1,-1),
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
            return new Bishop(this);
        }
    }
}