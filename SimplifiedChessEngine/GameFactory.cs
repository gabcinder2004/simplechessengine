using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplifiedChessEngine
{
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
}