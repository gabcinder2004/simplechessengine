using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplifiedChessEngine
{
    internal class Solution
    {
        /*
        5 3 6
        B B 1
        R C 2
        R C 4
        N A 4
        Q B 3
        R D 3
        N C 3
        Q D 1
        */

        /*
        2 1 1
        Q B 1
        N B 2
        Q A 4
        */

        static void Main(string[] args)
        {
            var games = GameFactory.CreateAllGames();

            //var cell = games.First().ChessBoard.Cells.First(x => x.Piece.GetType() == typeof(Queen) && x.Piece.Color == ChessColor.White);
            //var moves = cell.Piece.GetAvailableMoves(cell, games.First().ChessBoard);

            var gameSolutions = new List<GameSolver>();
            foreach (var game in games)
            {
                var solver = new GameSolver(game);
                var solution = solver.NextPlay(game.ChessBoard, ChessColor.White, new List<ChessMove>());
                gameSolutions.Add(solver);
                Console.WriteLine(solver.GameWon ? "YES" : "NO");
            }

            Console.ReadLine();
        }
    }

    public static class GameFactory
    {
        public static List<ChessGame> CreateAllGames()
        {
            var games = new List<ChessGame>();
            var totalGames = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < totalGames; i++)
            {
                var listArgs = new List<string>() { totalGames.ToString() };

                var gameInfo = Console.ReadLine();
                listArgs.Add(gameInfo);

                var splitArg = listArgs[1].Split(' ').ToList();
                var totalPieces = Convert.ToInt32(splitArg[0]) + Convert.ToInt32(splitArg[1]);

                for (int j = 0; j < totalPieces; j++)
                {
                    listArgs.Add(Console.ReadLine());
                }

                var args = listArgs.ToArray();

                var game = new ChessGame()
                {
                    ChessBoard = new ChessBoard(),
                    CurrentMoveCount = 0,
                };

                game.Initialize(args);
                games.Add(game);
            }

            return games;
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

            if (allMoves.Count() >= Game.TotalMovesAllowed)
            {
                return new List<ChessMove>();
            }

            var cells = board.Cells.Where(cell => cell.Piece != null && cell.Piece.Color == colorTurn).OrderBy(x => x.Piece.GetType() != typeof(Queen)).ToList();
            foreach (var cell in cells)
            {
                bool protectPiece = cell.Piece.GetType() == typeof(Queen) && cell.Piece.Color == colorTurn;
                var availableMoves = cell.Piece.GetAvailableMoves(cell, board, protectPiece);

                // if it is possible to kill the opposing queen, do it.
                if (availableMoves.Any(move => move.To.Piece != null && move.To.Piece.GetType() == typeof(Queen) && move.Action == ChessAction.KILL))
                {
                    var move = availableMoves.First(m => m.To.Piece != null && m.To.Piece.GetType() == typeof(Queen) && m.Action == ChessAction.KILL);
                    
                    if (colorTurn != ChessColor.White) return new List<ChessMove>();

                    allMoves.Add(move);
                    GameWon = true;
                    WinningMoves = allMoves;
                    return WinningMoves;
                }

                //if our queen is under threat, run.
                if (cell.Piece.GetType() == typeof(Queen) && cell.Piece.IsThreatened(cell, board))
                {
                    availableMoves = availableMoves.Where(move => move.From.Piece.GetType() == typeof(Queen)).ToList();
                }

                //Queen cannot be killed this turn, find next move.
                foreach (var move in availableMoves)
                {
                    var currentMoves = new List<ChessMove>();
                    currentMoves.AddRange(allMoves);
                    var newBoard = (ChessBoard)board.Clone();

                    newBoard.MakeMove(move);
                    currentMoves.Add(move);

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

    public abstract class ChessPiece
    {
        public ChessColor Color { get; set; }

        public abstract List<Tuple<int, int>> MovePattern { get; }

        public abstract List<ChessMove> GetAvailableMoves(Cell currentCell, ChessBoard board, bool protectPiece = false);

        public List<ChessMove> AvailableMoves { get; set; }

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

        protected List<ChessMove> GetAvailableMoves(List<Tuple<int, int>> movePattern, Cell currentCell, ChessBoard board, bool protectPiece)
        {
            var availableMoves = new List<ChessMove>();

            foreach (var pattern in movePattern)
            {
                var directionX = pattern.Item1;
                var directionY = pattern.Item2;

                while (true)
                {
                    var newCell = board.Cells.SingleOrDefault(cell => cell.X == currentCell.X + directionX && cell.Y == currentCell.Y + directionY);

                    if (newCell == null || newCell.Piece != null && newCell.Piece.Color == currentCell.Piece.Color)
                    {
                        break;
                    }

                    var action = newCell.Piece == null ? ChessAction.MOVE : ChessAction.KILL;

                    // Check if queen will be threatened if she makes this move
                    if (protectPiece)
                    {
                        var moveInQuestion = new ChessMove(currentCell, newCell, action, currentCell.Piece.Color);

                        var newBoard = board.Clone();
                        newBoard.MakeMove(moveInQuestion);
                        var newQueenCell = newBoard.Cells.First(cell => cell.Piece != null && cell.Piece.GetType() == typeof(Queen) && cell.Piece.Color == currentCell.Piece.Color);

                        if (newQueenCell.Piece.IsThreatened(newQueenCell, newBoard))
                        {
                            break;
                        }
                    }

                    availableMoves.Add(new ChessMove(currentCell, newCell, action, currentCell.Piece.Color));

                    if (action == ChessAction.MOVE)
                    {
                        directionX += directionX;
                        directionY += directionY;
                        continue;
                    }

                    break;
                }
            }

            return availableMoves;
        }

        public virtual bool IsThreatened(Cell currentCell, ChessBoard board)
        {
            board.Cells.Where(cell => cell.Piece != null && cell.X != currentCell.X && cell.Y != currentCell.Y).ToList().ForEach(x => x.Piece.GetAvailableMoves(x, board, false));

            return board.Cells.Where(cell =>cell.Piece != null && cell.Piece.Color != currentCell.Piece.Color)
                .Any(cell => cell.Piece.AvailableMoves != null && cell.Piece.AvailableMoves
                .Any(move => move.Action == ChessAction.KILL && move.To.X == currentCell.X && move.To.Y == currentCell.Y));
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
            var piece = From.Piece.GetType();

            return string.Format("{0} {4}| {1}:{2} | {3}", Turn, From, To, Action, piece.Name);
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
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ (Piece != null ? Piece.GetHashCode() : 0);
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

        public override List<ChessMove> GetAvailableMoves(Cell currentCell, ChessBoard board, bool protectPiece)
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

            AvailableMoves = availableMoves;
            return AvailableMoves;
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

        public override List<ChessMove> GetAvailableMoves(Cell currentCell, ChessBoard board, bool protectPiece = false)
        {
            AvailableMoves = base.GetAvailableMoves(MovePattern, currentCell, board, protectPiece);
            return AvailableMoves;
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

        public override List<ChessMove> GetAvailableMoves(Cell currentCell, ChessBoard board, bool protectPiece = false)
        {
            AvailableMoves = base.GetAvailableMoves(MovePattern, currentCell, board, protectPiece);
            return AvailableMoves;
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

        public override List<ChessMove> GetAvailableMoves(Cell currentCell, ChessBoard board, bool protectPiece = false)
        {
            AvailableMoves = base.GetAvailableMoves(MovePattern, currentCell, board, protectPiece);
            return AvailableMoves;
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
}