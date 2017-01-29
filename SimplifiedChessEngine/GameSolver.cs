using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SimplifiedChessEngine
{
    public class GameSolver
    {
        public bool GameWon;
        public ChessGame Game { get; set; }
        public List<ChessMove> WinningMoves { get; set; }
        public Stopwatch Stopwatch { get; set; }

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

            if ((allMoves.Count() >= Game.TotalMovesAllowed) || (colorTurn == ChessColor.Black && allMoves.Count == Game.TotalMovesAllowed - 1))
            {
                return new List<ChessMove>();
            }

            var cells = board.Cells.Where(cell => cell.Piece != null && cell.Piece.Color == colorTurn).OrderBy(x => x.Piece.GetType() != typeof(Queen)).ToList();
            foreach (var cell in cells)
            {
                var availableMoves = cell.Piece.GetAvailableMoves(cell, board);

                // if it is possible to kill the opposing queen, do it.
                if (availableMoves.Any(move => move.To.Piece != null && move.To.Piece.GetType() == typeof(Queen) && move.Action == ChessAction.KILL))
                {
                    var move = availableMoves.First(m => m.To.Piece != null && m.To.Piece.GetType() == typeof(Queen) && m.Action == ChessAction.KILL);
                    
                    // If our queen is going to die, abandon this move path
                    if (colorTurn != ChessColor.White) return new List<ChessMove>();

                    // If we can kill the black queen, do it and win.
                    allMoves.Add(move);
                    GameWon = true;
                    WinningMoves = allMoves;
                    return WinningMoves;
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

            // If it's blacks turn and they don't have any valid moves.. checkmate.
            if (colorTurn == ChessColor.Black && !cells.Any(cell => cell.Piece.AvailableMoves.Count > 0))
            {
                WinningMoves = allMoves;
                GameWon = true;
                return WinningMoves;
            }

            return new List<ChessMove>();
        }
    }
}