using System;
using System.Text;
using OfficeOpenXml;
using TournamentWeb.Models;
using TournamentWeb.Services;

namespace TournamentWeb.ExcelReaders
{
    public class PointCalculator
    {
        public static int AddScoreForWinner(ref int score, string winner)
        {
            FakeConsole.WriteLine($"+16 for korrekt finalevinner : {winner}");
            score += 16;
            return score;
        }

        public static int AddScoreForBronzeWinner(ref int score, string bronzeWinner)
        {
            FakeConsole.WriteLine($"+14 for korrekt bronsefinalevinner : {bronzeWinner}");
            score += 14;
            return score;
        }

        public static void AddScoreForCorrectResultInGroupMatch(ref int score)
        {
            score += 2;
            //FakeConsole.OutputEncoding = Encoding.UTF8;
            FakeConsole.WriteLine("+2 for gruppespillkamp : korrekt resultat");
        }

        public static void AddScoreForCorrectOutcomeInGroupMatch(ref int score)
        {
            score += 2;
            //Console.OutputEncoding = Encoding.UTF8;
            FakeConsole.WriteLine("+2 for gruppespillkamp : korrekt utfall");
        }

        public static void AddScoreForCorrectPlacementInGroup(ref int score, dynamic pos)
        {
            score += 2;
            //Console.OutputEncoding = Encoding.UTF8;
            FakeConsole.WriteLine($"+2 for {pos} på korrekt plass i gruppen");
        }

        public static void AddScoreForEightFinals(ref int score, string eightfinalists)
        {
            score += 4;
            //Console.OutputEncoding = Encoding.UTF8;
            FakeConsole.WriteLine($"+4 for {eightfinalists} videre til åttendelsfinale");
        }

        public static void AddScoreForQuarterfinals(ref int score, string quarterfinalist)
        {
            score += 6;
            FakeConsole.WriteLine($"+6 for {quarterfinalist} videre til kvartfinale");
        }

        public static void AddScoreForSemifinals(ref int score, string semifinalist)
        {
            score += 8;
            FakeConsole.WriteLine($"+8 for {semifinalist} videre til semifinale");
        }

        public static void AddScoreForTeamInFinals(ref int score, string finalist)
        {
            score += 12;
            FakeConsole.WriteLine($"+12 for {finalist} videre til finale");
        }

        public static void AddScoreForTeamInBronzeFinals(ref int score, string finalist)
        {
            score += 10;
            FakeConsole.WriteLine($"+10 for {finalist} videre til bronsefinale");
        }

        public static void AddScoreForWinner(ExcelWorksheet worksheet, Results results, ref int score)
        {
            var winner = TeamPlacementReader.GetWinner(worksheet);

            if (winner.Equals(results.Winner))
                score = AddScoreForWinner(ref score, winner);
        }

        public static void AddScoreForBronzeWinner(ExcelWorksheet worksheet, Results results, ref int score)
        {
            var bronzeWinner = TeamPlacementReader.GetBronzeWinner(worksheet);

            if (bronzeWinner.Equals(results.BronzeWinner))
                score = AddScoreForBronzeWinner(ref score, bronzeWinner);
        }
    }
}