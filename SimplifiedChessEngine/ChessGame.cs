using System;
using System.Linq;

namespace SimplifiedChessEngine
{
    public class ChessGame
    {
        public ChessBoard ChessBoard { get; set; }
        public int TotalMovesAllowed { get; set; }
        public int CurrentMoveCount { get; set; }
        public int WhitePieceCount { get; set; }
        public int BlackPieceCount { get; set; }

        public void Initialize(string[] args)
        {
            var splitArgs = args[1].Split(' ');
            TotalMovesAllowed = Convert.ToInt32(splitArgs[2]);
            WhitePieceCount = Convert.ToInt32(splitArgs[0]);
            BlackPieceCount = Convert.ToInt32(splitArgs[1]);

            InitializePiecePositions(args);
            FillRemainingBoard();
        }

        private void InitializePiecePositions(string[] args)
        {
            for (int i = 2; i < args.Length; i++)
            {
                var splitArgs = args[i].Split(' ');
                var piece = ChessPiece.Create(splitArgs[0]);
                piece.Color = (i - 2) < WhitePieceCount ? ChessColor.White : ChessColor.Black;

                var x = ChessUtility.LetterToNumber[splitArgs[1]];
                var y = Convert.ToInt32(splitArgs[2]);

                ChessBoard.Cells.Add(new Cell(piece, x, y));
            }
        }

        private void FillRemainingBoard()
        {
            for (int x = 1; x <= ChessBoard.MaxX; x++)
            {
                for (int y = 1; y <= ChessBoard.MaxY; y++)
                {
                    if (!ChessBoard.Cells.Any(cell => cell.X == x && cell.Y == y))
                    {
                        ChessBoard.Cells.Add(new Cell(null, x, y));
                    }
                }
            }
        }
    }
}