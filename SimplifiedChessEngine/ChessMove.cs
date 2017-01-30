namespace SimplifiedChessEngine
{
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
}