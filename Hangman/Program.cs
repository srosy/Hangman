using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hangman
{
    class Program
    {
        static void Main(string[] args)
        {
            var selectedLetters = new List<char>();
            var word = GetWord();
            var numIncorrect = 0;
            var gameDone = false;

            while (!gameDone)
            {
                DrawHangman(numIncorrect);

                var displayWord = new List<char>(); // selected && correct characters or _ underscores to hide characters
                foreach (char c in word)
                {
                    if (!selectedLetters.Any() || !selectedLetters.Contains(c))
                        displayWord.Add('_');
                    else
                        displayWord.Add(c);
                }
                DisplayInfo(selectedLetters, numIncorrect, displayWord);

                Console.Write("Select a character: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    var letter = input.ToCharArray().First();
                    if (!word.ToCharArray().Contains(letter) && !selectedLetters.Contains(letter)) // only penalize once per letter
                        numIncorrect++;
                    if (!selectedLetters.Contains(letter))
                        selectedLetters.Add(letter);

                    gameDone = new string(displayWord.ToArray()).Replace('_', letter).Equals(word) || numIncorrect >= 6;
                    if (gameDone)
                    {
                        Console.WriteLine(numIncorrect >= 6 ? $"\nGame Over\nWord was {word}\nYou lose!" : $"Congrats\nWord was {word}\nYou win!");
                        Thread.Sleep(5000);
                    }
                }
                Console.Clear();
            }
        }

        private static void DisplayInfo(List<char> selectedLetters, int numIncorrect, List<char> displayWord)
        {
            displayWord.ForEach(c => Console.Write(c + " "));
            Console.WriteLine("\n");
            Console.WriteLine("Chosen Letters: " + string.Join(",", selectedLetters));
            Console.WriteLine($"Num incorrect: {numIncorrect}/6");
            //Console.WriteLine(word); // testing
            Console.WriteLine();
        }

        private static void DrawHangman(int numIncorrect)
        {
            if (numIncorrect >= 1)
            {
                Console.WriteLine(" ### ");
                Console.WriteLine("  |  ");
                Console.WriteLine("(^.^)");
            }
            if (numIncorrect == 2)
                Console.WriteLine("  |  ");
            else if (numIncorrect >= 2)
                Console.Write(" /");
            if (numIncorrect >= 3)
                Console.Write("|");
            if (numIncorrect >= 4)
                Console.WriteLine("\\");
            if (numIncorrect >= 5)
                Console.Write(" /");
            if (numIncorrect >= 6)
                Console.WriteLine("\\");
            Console.WriteLine("\n");
        }

        private static string GetWord()
        {
            var client = new RestClient("https://random-word-api.herokuapp.com/word?number=10");
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);
            return JArray.Parse(response.Content).ToObject<List<string>>().First(w => w.Length > 6);
        }
    }
}
