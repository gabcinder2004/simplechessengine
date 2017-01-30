using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplifiedChessEngine
{
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
}