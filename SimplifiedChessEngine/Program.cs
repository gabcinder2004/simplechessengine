using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimplifiedChessEngine
{
    internal class Solution
    {
        static void Main(string[] args)
        {
            var games = GameFactory.CreateAllGames();
            var gameSolutions = new List<GameSolver>();
            foreach (var game in games)
            {
                var solver = new GameSolver(game);
                solver.NextPlay(game.ChessBoard, ChessColor.White, new List<ChessMove>());
                gameSolutions.Add(solver);
                //var str = string.Format("{0} : {1}", solver.GameOver ? "YES" : "NO", solver.Stopwatch.Elapsed);
                Console.WriteLine(solver.Results.Any(x => x.Winner == ChessColor.White) ? "YES" : "NO");
            }

            Console.ReadLine();
        }
    }

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

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ (!IsEmpty() ? Piece.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            return new Cell(IsEmpty() ? null : Piece.Clone(), X, Y);
        }
    }

    public enum ChessAction
    {
        MOVE,
        KILL
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

    public enum ChessColor
    {
        None,
        White,
        Black
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

    public abstract class ChessPiece
    {
        public ChessColor Color { get; set; }

        public abstract List<Tuple<int, int>> MovePattern { get; }

        public abstract List<ChessMove> GetAvailableMoves(Cell currentCell, ChessBoard board, bool protectPiece = true);

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

        public abstract ChessPiece Clone();

        protected List<ChessMove> GetAvailableMoves(List<Tuple<int, int>> movePattern, Cell currentCell, ChessBoard board, bool protectQueen)
        {
            var availableMoves = new List<ChessMove>();

            foreach (var pattern in movePattern)
            {
                var directionX = pattern.Item1;
                var directionY = pattern.Item2;

                while (true)
                {
                    var newCell = board.Cells.SingleOrDefault(cell => cell.X == currentCell.X + directionX && cell.Y == currentCell.Y + directionY);

                    // If cell doesn't exist, or the piece on that cell is the same color as the piece trying to move
                    if (newCell == null || !newCell.IsEmpty() && newCell.Piece.Color == currentCell.Piece.Color)
                    {
                        break;
                    }

                    var action = newCell.IsEmpty() ? ChessAction.MOVE : ChessAction.KILL;

                    // Check if queen will be threatened if the current piece makes this move
                    if (protectQueen)
                    {
                        var moveInQuestion = new ChessMove(currentCell, newCell, action, currentCell.Piece.Color);

                        var newBoard = board.Clone();
                        newBoard.MakeMove(moveInQuestion);
                        var newQueenCell = newBoard.Cells.First(cell => !cell.IsEmpty() && cell.Piece.GetType() == typeof(Queen) && cell.Piece.Color == currentCell.Piece.Color);


                        //(!moveInQuestion.To.IsEmpty() && moveInQuestion.To.Piece.GetType() != typeof(Queen) && moveInQuestion.Action == ChessAction.KILL) && 
                        if (newBoard.QueenThreatened(newQueenCell) && (moveInQuestion.To.IsEmpty() || moveInQuestion.To.Piece.GetType() != typeof(Queen) && moveInQuestion.Action == ChessAction.KILL))
                        {
                            if (moveInQuestion.Action == ChessAction.MOVE && currentCell.Piece.GetType() != typeof(Knight))
                            {
                                directionX += pattern.Item1;
                                directionY += pattern.Item2;
                                continue;
                            }
                            break;
                        }
                    }

                    availableMoves.Add(new ChessMove(currentCell, newCell, action, currentCell.Piece.Color));

                    // If the piece moved, then try to move another cell in the same direction (except for Knights)
                    if (action == ChessAction.MOVE && currentCell.Piece.GetType() != typeof(Knight))
                    {
                        directionX += pattern.Item1;
                        directionY += pattern.Item2;
                        continue;
                    }

                    break;
                }
            }

            return availableMoves;
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

    public class GameResult
    {
        public List<ChessMove> AllMoves { get; set; }
        public ChessColor Winner { get; set; }

        public override string ToString()
        {
            return Winner.ToString();
        }
    }

    public class GameSolver
    {
        public ChessColor Winner = ChessColor.White;
        public ChessGame Game { get; set; }
        public List<ChessMove> WinningMoves { get; set; }
        public Stopwatch Stopwatch { get; set; }

        public List<GameResult> Results { get; set; }

        public GameSolver(ChessGame game)
        {
            Game = game;
            WinningMoves = new List<ChessMove>();
            Results = new List<GameResult>();
        }

        public void NextPlay(ChessBoard board, ChessColor colorTurn, List<ChessMove> allMoves)
        {
            if (Results.Any(x => x.Winner == ChessColor.White))
            {
                return;
            }

            if ((allMoves.Count() >= Game.TotalMovesAllowed) || (colorTurn == ChessColor.Black && allMoves.Count == Game.TotalMovesAllowed - 1))
            {
                Results.Add(new GameResult { AllMoves = allMoves, Winner = ChessColor.Black });
                return;
            }

            var nextColor = colorTurn == ChessColor.White ? ChessColor.Black : ChessColor.White;
            var cells = board.Cells.Where(cell => !cell.IsEmpty() && cell.Piece.Color == colorTurn).OrderBy(x => x.Piece.GetType() != typeof(Queen)).ToList();
            cells.Where(x => !x.IsEmpty()).ToList().ForEach(c => c.Piece.GetAvailableMoves(c, board));

            var allMovesThisTurn = cells.Where(x => !x.IsEmpty() && x.Piece.Color == colorTurn).SelectMany(x => x.Piece.AvailableMoves).ToList();

            if (!allMovesThisTurn.Any())
            {
                Results.Add(new GameResult() { AllMoves = allMoves, Winner = nextColor });
                return;
            }

            if (allMovesThisTurn.Any(x => !x.To.IsEmpty() && x.To.Piece.GetType() == typeof(Queen) && x.Action == ChessAction.KILL))
            {
                var move = allMovesThisTurn.First(x => !x.To.IsEmpty() && x.To.Piece.GetType() == typeof(Queen) && x.Action == ChessAction.KILL);
                allMoves.Add(move);
                Results.Add(new GameResult() { AllMoves = allMoves, Winner = colorTurn });
                return;
            }

            foreach (var chessMove in allMovesThisTurn)
            {
                var currentMoves = new List<ChessMove>();
                currentMoves.AddRange(allMoves);
                var newBoard = (ChessBoard)board.Clone();

                newBoard.MakeMove(chessMove);
                currentMoves.Add(chessMove);

                NextPlay(newBoard, nextColor, currentMoves);

                if (Results.Any(x => x.Winner == ChessColor.White))
                {
                    return;
                }
            }

        }
    }

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

    public class Queen : ChessPiece
    {
        public Queen()
        {

        }

        public Queen(Queen queen)
        {
            AvailableMoves = new List<ChessMove>();
            Color = queen.Color;
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
                    new Tuple<int, int>(-1, -1),
                    new Tuple<int, int>(0, 1),
                    new Tuple<int, int>(0, -1),
                    new Tuple<int, int>(1, 0),
                    new Tuple<int, int>(-1, 0),
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
            return new Queen(this);
        }
    }

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