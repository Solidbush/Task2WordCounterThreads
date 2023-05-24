using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileReaderLib;
using System.Reflection;
using System.Diagnostics;
namespace FileReader
{
    class Program
    {
        private FileInfo fileForRead;
        private FileInfo fileForWrite;

        public string ReadText(string pathFileForRead, string pathFileForWrite)
        {
            string text = null;
            fileForRead = new FileInfo(pathFileForRead);
            fileForWrite = new FileInfo(pathFileForWrite);
            if (!fileForRead.Exists)
                throw new Exception($"File with path: {fileForRead.FullName} doesn't exists!");
            if (!fileForWrite.Exists)
            {
                var forRead = File.Create(fileForRead.FullName);
                forRead.Close();
            }

            foreach (var line in File.ReadLines(fileForRead.FullName, System.Text.Encoding.Default))
            {
                text += line;
            }

            Console.WriteLine($"File from {fileForRead.FullName} read successfully!");

            return text;
        }

        public void WriteToFile(Dictionary<string, int> wordCountDictionary)
        {
            StreamWriter fileWriter = new StreamWriter(fileForWrite.FullName, false, System.Text.Encoding.Default);
            foreach (var wordCountPair in wordCountDictionary)
            {
                fileWriter.WriteLine($"{wordCountPair.Key} {wordCountPair.Value}");
            }

            fileWriter.Close();
            Console.WriteLine($"Words have been successfully written to file: {fileForWrite.FullName}");
            Console.WriteLine($"Total: {wordCountDictionary.Count()}");
        }

        static void Main(string[] args)
        {
            var commonCounterTimer = new Stopwatch();
            var threadsCounterTimer = new Stopwatch();
            var program = new Program();
            Console.WriteLine("Input file's path for read: ");
            string pathForRead = Console.ReadLine();
            Console.WriteLine("Input file's path for write: ");

            string pathForWrite = Console.ReadLine();
            object readedText = program.ReadText(pathForRead, pathForWrite);
            MethodInfo countWords = typeof(FileReaderLib.WordCounter).GetMethod("CountWords", BindingFlags.Instance | BindingFlags.NonPublic);
            commonCounterTimer.Start();
            Dictionary<string, int> commonWordsCount = countWords.Invoke(new WordCounter(), new object[] { readedText }) as Dictionary<string, int>;
            commonCounterTimer.Stop();
            program.WriteToFile(commonWordsCount);

            WordCounter libMeths = new WordCounter();
            threadsCounterTimer.Start();
            Dictionary<string, int> asyncWordsCount = libMeths.CountWordsThread(readedText as string);
            threadsCounterTimer.Stop();
            program.WriteToFile(asyncWordsCount);

            Console.WriteLine("Common counter runtime: " + commonCounterTimer.Elapsed.ToString());
            Console.WriteLine("Threads counter runtime: " + threadsCounterTimer.Elapsed.ToString());
            Console.ReadKey();
        }
    }
}
