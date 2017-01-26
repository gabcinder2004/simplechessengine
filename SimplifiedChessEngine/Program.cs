﻿using System;
using System.Collections.Generic;
using System.Linq;

    class Solution
    {
        // args[0] = "1"
        // args[1] = "2 1 1"
        // args[2] = "N B 2"
        // args[3] = "Q B 1"
        // args[4] = "Q A 4"

        static void Main(string[] args)
        {
            var listArgs = args.ToList();
            listArgs.Add(Console.ReadLine());
            listArgs.Add(Console.ReadLine());

            var splitArg = listArgs.ElementAt(1).Split(' ').ToList();
            var totalPieces = Convert.ToInt32(splitArg[0]) + Convert.ToInt32(splitArg[1]);

            for (int i = 0; i < totalPieces; i++)
            {
                listArgs.Add(Console.ReadLine());
            }

            args = listArgs.ToArray();

            var game = new ChessGame()
            {
                ChessBoard = new ChessBoard(),
                CurrentMoveCount = 0,
            };

            game.Initialize(args);

            var solver = new GameSolver(game);

            solver.NextPlay(game.ChessBoard, ChessColor.White, new List<ChessMove>());
            Console.WriteLine(solver.GameWon ? "YES" : "NO");

            Console.ReadLine();
        }
    }

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
            var board = new ChessBoard();
            board.Cells = Cells.Select(item => (Cell)item.Clone()).ToList();
            return board;
        }
    }

    public class GameSolver
    {
        public bool GameWon = false;
        public ChessGame Game { get; set; }
        public List<ChessMove> WinningMoves { get; set; }
        public ChessMove BestMove { get; set; }

        public GameSolver(ChessGame game)
        {
            Game = game;
            WinningMoves = new List<ChessMove>();
        }

        public List<ChessMove> NextPlay(ChessBoard board, ChessColor colorTurn, List<ChessMove> allMoves)
        {
            var moves = new List<ChessMove>();

            if (GameWon)
            {
                return moves;
            }

            if (allMoves.Count(x => x.Turn == ChessColor.White) >= Game.TotalMovesAllowed)
            {
                return new List<ChessMove>();
            }

            var cells = board.Cells.Where(cell => cell.Piece != null && cell.Piece.Color == colorTurn).ToList();
            foreach (var cell in cells)
            {
                var availableMoves = cell.Piece.AvailableMoves(cell, board);
                foreach (var move in availableMoves)
                {
                    var currentMoves = new List<ChessMove>();
                    currentMoves.AddRange(allMoves);
                    var newBoard = (ChessBoard)board.Clone();

                    newBoard.MakeMove(move);
                    currentMoves.Add(move);

                    var blackQueenAlive = false;
                    var whiteQueenAlive = false;

                    foreach (var c in newBoard.Cells)
                    {
                        if (c.Piece == null)
                        {
                            continue;
                        }

                        if (c.Piece.Color == ChessColor.Black && c.Piece.GetType() == typeof(Queen))
                        {
                            blackQueenAlive = true;
                        }

                        if (c.Piece.Color == ChessColor.White && c.Piece.GetType() == typeof(Queen))
                        {
                            whiteQueenAlive = true;
                        }
                    }

                    if (!blackQueenAlive)
                    {
                        GameWon = true;
                        WinningMoves = currentMoves;
                        return WinningMoves;
                    }

                    if (!whiteQueenAlive)
                    {
                        return new List<ChessMove>();
                    }

                    var nextColor = colorTurn == ChessColor.White ? ChessColor.Black : ChessColor.White;
                    var m = NextPlay(newBoard, nextColor, currentMoves);

                    if (GameWon)
                    {
                        return m;
                    }
                }
            }

            return new List<ChessMove>();
        }
    }

    public class ChessMove
    {
        public Cell From { get; set; }
        public Cell To { get; set; }

        public ChessAction Action { get; set; }
        public ChessColor Turn { get; set; }

        public ChessMove(Cell @from, Cell to, ChessAction action, ChessColor turn)
        {
            From = @from;
            To = to;
            Action = action;
            Turn = turn;
        }

        public override string ToString()
        {
            return string.Format("{0} | {1}:{2} | {3}", Turn, From, To, Action);
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
                hashCode = (hashCode*397) ^ Y;
                hashCode = (hashCode*397) ^ (Piece != null ? Piece.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            return new Cell(Piece, X, Y);
        }
    }

    public class Knight : ChessPiece
    {
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

        public override List<ChessMove> AvailableMoves(Cell currentCell, ChessBoard board)
        {
            var availableMoves = new List<ChessMove>();

            foreach (var pattern in MovePattern)
            {
                var directionX = pattern.Item1;
                var directionY = pattern.Item2;

                var newCell =
                    board.Cells.SingleOrDefault(
                        cell => cell.X == currentCell.X + directionX && cell.Y == currentCell.Y + directionY);

                if (newCell == null)
                {
                    continue;
                }

                if (newCell.Piece == null)
                {
                    availableMoves.Add(new ChessMove(currentCell, newCell, ChessAction.MOVE, currentCell.Piece.Color));

                    continue;
                }

                if (newCell.Piece.Color == currentCell.Piece.Color)
                {
                    continue;
                }

                if (newCell.Piece.Color != currentCell.Piece.Color)
                {
                    availableMoves.Add(new ChessMove(currentCell, newCell, ChessAction.KILL, currentCell.Piece.Color));
                }
            }

            return availableMoves;
        }
    }

    public class Bishop : ChessPiece
    {
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

        public override List<ChessMove> AvailableMoves(Cell currentCell, ChessBoard board)
        {
            return base.AvailableMoves(MovePattern, currentCell, board);
        }
    }

    public class Rook : ChessPiece
    {
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

        public override List<ChessMove> AvailableMoves(Cell currentCell, ChessBoard board)
        {
            return base.AvailableMoves(MovePattern, currentCell, board);
        }
    }

    public class Queen : ChessPiece
    {
        public override List<Tuple<int, int>> MovePattern
        {
            get
            {
                return new List<Tuple<int, int>>
                {
                    new Tuple<int, int>(1, 1),
                    new Tuple<int, int>(1, -1),
                    new Tuple<int, int>(-1, 1),
                    new Tuple<int, int>(-1, -1),
                    new Tuple<int, int>(0, 1),
                    new Tuple<int, int>(0, -1),
                    new Tuple<int, int>(1, 0),
                    new Tuple<int, int>(-1, 0),
                };

            }
        }

        public override List<ChessMove> AvailableMoves(Cell currentCell, ChessBoard board)
        {
            return base.AvailableMoves(MovePattern, currentCell, board);
        }
    }

    public abstract class ChessPiece
    {
        public ChessColor Color { get; set; }

        public abstract List<Tuple<int, int>> MovePattern { get; }

        public abstract List<ChessMove> AvailableMoves(Cell currentCell, ChessBoard board);

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

        protected List<ChessMove> AvailableMoves(List<Tuple<int, int>> movePattern, Cell currentCell, ChessBoard board)
        {
            var availableMoves = new List<ChessMove>();

            foreach (var pattern in movePattern)
            {
                var directionX = pattern.Item1;
                var directionY = pattern.Item2;

                while (true)
                {
                    var newCell =
                        board.Cells.SingleOrDefault(
                            cell => cell.X == currentCell.X + directionX && cell.Y == currentCell.Y + directionY);

                    if (newCell == null)
                    {
                        break;
                    }

                    if (newCell.Piece == null)
                    {
                        availableMoves.Add(new ChessMove(currentCell, newCell, ChessAction.MOVE, currentCell.Piece.Color));

                        if (directionX != 0)
                        {
                            directionX = (directionX > 0) ? directionX + 1 : directionX - 1;
                        }

                        if (directionY != 0)
                        {
                            directionY = (directionY > 0) ? directionY + 1 : directionY - 1;
                        }

                        continue;
                    }

                    if (newCell.Piece.Color == currentCell.Piece.Color)
                    {
                        break;
                    }

                    if (newCell.Piece.Color != currentCell.Piece.Color)
                    {
                        availableMoves.Add(new ChessMove(currentCell, newCell, ChessAction.KILL, currentCell.Piece.Color));

                        directionX = (directionX > 0) ? directionX + 1 : directionX - 1;
                        directionY = (directionY > 0) ? directionY + 1 : directionY - 1;
                    }
                }
            }

            return availableMoves;
        }
    }

    public enum ChessColor
    {
        None,
        White,
        Black
    }

    public enum ChessAction
    {
        MOVE,
        KILL
    }
