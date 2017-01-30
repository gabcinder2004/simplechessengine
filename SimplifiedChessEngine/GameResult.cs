using System.Collections.Generic;

namespace SimplifiedChessEngine
{
    public class GameResult
    {
        public List<ChessMove> AllMoves { get; set; }
        public ChessColor Winner { get; set; }

        public override string ToString()
        {
            return Winner.ToString();
        }
    }
}