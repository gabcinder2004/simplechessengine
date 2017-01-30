using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimplifiedChessEngine
{
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
            cells.Where(x => !x.IsEmpty()).ToList().ForEach(c => c.Piece.GetAvailableMoves(c, board, allMoves.Count + 1));

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
}