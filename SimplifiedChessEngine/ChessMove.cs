namespace SimplifiedChessEngine
{
    public class ChessMove
    {
        public Cell From { get; set; }
        public Cell To { get; set; }

        public int TurnNumber { get; set; }
        public ChessAction Action { get; set; }
        public ChessColor Turn { get; set; }

        public ChessMove(Cell from, Cell to, ChessAction action, ChessColor turn, int turnNumber)
        {
            From = @from;
            To = to;
            Action = action;
            Turn = turn;
            TurnNumber = turnNumber;
        }

        public override string ToString()
        {
            var piece = From.Piece.GetType();

            return string.Format("Turn {5}: {0} {4} | {1}:{2} | {3}", Turn, From, To, Action, piece.Name, TurnNumber);
        }
    }
}