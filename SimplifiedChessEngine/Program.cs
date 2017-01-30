using System;
using System.Collections.Generic;

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
                Console.WriteLine(solver.Winner == ChessColor.White ? "YES" : "NO");
            }

            Console.ReadLine();
        }
    }
}