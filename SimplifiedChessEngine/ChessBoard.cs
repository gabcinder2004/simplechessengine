using System.Collections.Generic;
using System.Linq;

namespace SimplifiedChessEngine
{
    public class ChessBoard
    {
        public const int MaxX = 4;
        public const int MaxY = 4;
        public List<Cell> Cells { get; set; }

        public ChessBoard()
        {
            Cells = new List<Cell>();
        }

        public void MakeMove(ChessMove move)
        {
            var from = Cells.Find(x => x.Equals(move.From));
            var to = Cells.Find(x => x.Equals(move.To));

            to.Piece = from.Piece;
            from.Piece = null;
        }

        public ChessBoard Clone()
        {
            var board = new ChessBoard { Cells = Cells.Select(item => (Cell)item.Clone()).ToList() };
            return board;
        }

        public bool QueenThreatened(Cell currentCell)
        {
            Cells.Where(cell => !cell.IsEmpty()
                                && cell.Piece.Color != currentCell.Piece.Color)
                .ToList().ForEach(x => x.Piece.GetAvailableMoves(x, this, false));

            return Cells.Where(cell => !cell.IsEmpty() && cell.Piece.Color != currentCell.Piece.Color)
                .Any(cell => cell.Piece.AvailableMoves != null && cell.Piece.AvailableMoves
                    .Any(move => move.Action == ChessAction.KILL && move.To.X == currentCell.X && move.To.Y == currentCell.Y));
        }
    }
}