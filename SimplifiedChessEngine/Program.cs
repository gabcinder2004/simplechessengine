using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                solver.NextPlay(game.ChessBoard, ChessColor.White, new List<ChessMove>());
                stopwatch.Stop();
                solver.Stopwatch = stopwatch;
                gameSolutions.Add(solver);
                var str = string.Format("{0} : {1}", solver.GameWon ? "YES" : "NO", solver.Stopwatch.Elapsed);
                Console.WriteLine(str);
            }

            Console.ReadLine();
        }
    }
}