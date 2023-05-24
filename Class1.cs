using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace FileReaderLib
{
    public class WordCounter
    {
        private static readonly char[] delimiterChars = { ' ', ',', '.', ':', '!', '?', '-', '\t' };
        private static readonly char newLine = '\n';
        private static readonly string nullString = "";
        private const int CommonValue = 1;
        private const int Incriment = 1;
        private static Dictionary<string, int> wordCountDictionary = new Dictionary<string, int>();

        private Dictionary<string, int> CountWords(string text)
        {
            string[] lines = text.Split(newLine);
            foreach(var line in lines)
            {
                string[] words = line.Split(delimiterChars);
                foreach (string word in words)
                {
                    var wordToLower = word.ToLower().Trim();
                    if (wordToLower == nullString)
                        continue;
                    if (wordCountDictionary.ContainsKey(wordToLower))
                        wordCountDictionary[wordToLower] += Incriment;
                    else
                        wordCountDictionary.Add(wordToLower, CommonValue);
                }
            }

            return wordCountDictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public Dictionary<string, int> CountWordsThread(string text)
        {
            string[] lines = text.Split(newLine);
            foreach (string line in lines)
            {
                new Thread(() =>
                {
                    System.Action action = () =>
                    {
                        string[] words = line.Split(delimiterChars);
                        foreach(string word in words)
                        {
                            var wordToLower = word.ToLower().Trim();
                            if (wordToLower == nullString)
                                continue;
                            if (wordCountDictionary.ContainsKey(wordToLower))
                                wordCountDictionary[wordToLower] += Incriment;
                            else
                                wordCountDictionary.Add(wordToLower, CommonValue);
                        }
                    };

                }).Start();
            }

            return wordCountDictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

    }
}
