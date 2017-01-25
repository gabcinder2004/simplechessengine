using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplifiedChessEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            // args[0] = "1"
            // args[1] = "2 1 1"
            // args[2] = "N B 2"
            // args[3] = "Q B 1"
            // args[4] = "Q A 4"

            var cell1 = new Cell(null, 2, 2);
            var cell2 = new Cell(null, 4, 4);

            var knight = new Knight();
            var result = knight.CanMove(cell1, cell2);

            //var game = new ChessGame()
            //{
            //    ChessBoard = new ChessBoard(),
            //    CurrentMoveCount = 0,
            //};

            //game.Initialize(args);
        }
    }

    public class ChessGame
    {
        public const int MaxX = 4;
        public const int MaxY = 4;
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
            for (int x = 1; x <= MaxX; x++)
            {
                for (int y = 1; y <= MaxY; y++)
                {
                    if (!ChessBoard.Cells.Any(cell => cell.X == x && cell.Y == y))
                    {
                        ChessBoard.Cells.Add(new Cell(null, x, y));
                    }
                }
            }
        }
    }

    public static class ChessUtility
    {
        public static Dictionary<string, int> LetterToNumber = new Dictionary<string, int>()
        {
            {"A", 1},
            {"B", 2},
            {"C", 3},
            {"D", 4}
        };

        public static Dictionary<int, string> NumberToLetter = new Dictionary<int, string>()
        {
            {1, "A"},
            {2, "B"},
            {3, "C"},
            {4, "D"}
        };
    }

    public class ChessBoard
    {
        public List<ChessPiece> ChessPieces { get; set; }
        public List<Cell> Cells { get; set; }

        public ChessBoard()
        {
            ChessPieces = new List<ChessPiece>();
            Cells = new List<Cell>();
        }

        public void MovePiece(Cell from, Cell to)
        {
            if (from.Piece == null)
            {
                throw new Exception("Cannot move piece");
            }

            if (to.Piece != null)
            {
                throw new NotImplementedException();
            }

            if (from.Piece.CanMove(from, to))
            {
                to.Piece = from.Piece;
                from.Piece = null;
            }
        }
    }

    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ChessPiece Piece { get; set; }
        public string Position { get { return ChessUtility.NumberToLetter[X] + Y; } }

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
    }

    public enum ChessColor
    {
        White,
        Black
    }

    public abstract class ChessPiece
    {
        public ChessColor Color { get; set; }
        public abstract bool CanMove(Cell from, Cell to);
        public static ChessPiece Create(string letter)
        {
            switch (letter)
            {
                case "Q":
                    return new Queen();
                case "N":
                    return new Knight();
                case "B":
                    return new Bishop();
                case "R":
                    return new Rook();
                default:
                    throw new Exception("Cannot create chess piece: Invalid Type");
            }
        }
    }

    public class Knight : ChessPiece
    {
        /*
         *  Knights can move either:
         *  - 2 LEFT/RIGHT 1 UP/DOWN
         *  - 1 LEFT/RIGHT 2 UP/DOWN
         */
        public override bool CanMove(Cell from, Cell to)
        {
            if (Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y) == 3)
            {
                return true;
            }

            return false;
        }
    }

    public class Queen : ChessPiece
    {
        /* Queens can move in any direction from their current position
         * 
         */
        public override bool CanMove(Cell from, Cell to)
        {
            throw new NotImplementedException();
        }
    }

    public class Bishop : ChessPiece
    {
        public override bool CanMove(Cell from, Cell to)
        {
            if (from.x)
        }
    }

    public class Rook : ChessPiece
    {
        public override bool CanMove(Cell from, Cell to)
        {
            throw new NotImplementedException();
        }
    }
}
