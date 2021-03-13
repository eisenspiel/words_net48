using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WordsCountingLib
{
    public class WordsCountingClass
    {
        private Dictionary<string, int> CountWords(string TextInput)
        {
            //Засекаем время обработки в одном потоке
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Заменяем все символы, которые не буквы, на пробелы и переводим текст в нижний регистр
            TextInput = new Regex("[^a-zA-Zа-яёА-ЯЁ]").Replace(TextInput, " ").ToLower();

            // Создаём массив полученных слов
            var words = TextInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //Создаём словарь для пары "слово - количество"
            Dictionary<string, int> wordCountUnsorted = new Dictionary<string, int>();

            // Добавляем слово из массива в словарь, если его ещё нет. Если оно уже есть, увеличиваем количество на единицу
            for (int i = 0; i < words.Length; i++)
            {
                if (!wordCountUnsorted.ContainsKey(words[i]))
                {
                    wordCountUnsorted.Add(words[i], 1);
                }
                else
                {
                    wordCountUnsorted[words[i]] += 1;
                }
            }

            //Сортируем по убыванию вхождений
            var sortedDict = (from entry in wordCountUnsorted orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);

            //выводим время обработки
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds, ts.Milliseconds);
            Console.WriteLine($"The text processing by one thread took {elapsedTime}");


            return (Dictionary<string, int>)sortedDict;
        }

        public Dictionary<string, int> CountWordsMultithread(string TextInput)
        {
            //Засекаем время обработки в одном потоке
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Заменяем все символы, которые не буквы, на пробелы и переводим текст в нижний регистр
            TextInput = new Regex("[^a-zA-Zа-яёА-ЯЁ]").Replace(TextInput, " ").ToLower();

            // Создаём массив полученных слов
            var words = TextInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //Создаём словарь для пары "слово - количество", который может работать с несколькими потоками
            ConcurrentDictionary<string, int> cd = new ConcurrentDictionary<string, int>();

            // Добавляем слово из массива в словарь, и единицу в значение, если его ещё нет. Если слово уже есть, увеличиваем количество на единицу
            Parallel.For(0, words.Length, i => {
                cd.AddOrUpdate(words[i], 1, (key, oldValue) => oldValue + 1);
            });

            //Сортируем по убыванию вхождений
            //var sortedDict = (from entry in cd.AsParallel() orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            var sortedDict = (from entry in cd orderby entry.Value descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);

            //выводим время обработки
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:000}",
                ts.Minutes, ts.Seconds, ts.Milliseconds);
            Console.WriteLine($"The text processing by multiple threads in a public metod took {elapsedTime}");

            return (Dictionary<string, int>)sortedDict;
        }
    }
}
