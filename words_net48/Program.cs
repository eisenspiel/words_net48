using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using WordsCountingLib;
using System.Reflection;

namespace words_net48
{
    class Program
    {
        static void Main(string[] args)
        {
            //Запрашиваем путь к файлу, если путь не введён, по умолчанию путь к файлу в корне диска D:
            Console.Title = "Words Counting -- Test Task for Digital Design";
            Console.WriteLine("Please enter the path to your txt file\nor\nJust press Enter if you want to use the default path \"D:\\Vojnaimir.Tom1.utf8.txt\"");
            string path = Console.ReadLine();
            if (path == "") path = @"D:\Vojnaimir.Tom1.UTF8.txt";


            //Читаем файл
            Console.WriteLine($"Trying to open {path}...");
            string whole_text = "";
            try
            {
                whole_text = System.IO.File.ReadAllText(path);
            }
            catch (Exception)
            {
                Console.WriteLine("Something wrong with the path you entered. Please start again and enter a valid vath");
                Environment.Exit(0);
            }
            Console.WriteLine($"Done! There are {whole_text.Length} symbols.\nNow please wait, the book is big, so it's gonna take a while...");
            
            //Создаём словарь, куда будем записывать результат подсчёта слов
            Dictionary<string, int> wordCount = new Dictionary<string, int>();

            //Предлагаем выбрать, как считать слова: 1 - приватным методом в одном потоке, 2 - публичным методом многопоточно, 3 - на сервере многопоточно
            Console.WriteLine("\nPlease enter 1 if you want to use private metod with one thread\nor" +
                "\nEnter 2 for public method with multiple threads\nor" +
                "\nEnter anything other for counting words on the server side");
            string ChosenMethod = Console.ReadLine();            
            if (ChosenMethod == "1")
            {
                //Вызываем из библиотеки WordsCountLib приватный метод подсчёта слов из текста
                var WordsCountIstance = new WordsCountingClass();
                var magicMethod = WordsCountIstance.GetType().GetMethod("CountWords", BindingFlags.NonPublic | BindingFlags.Instance);
                wordCount = (Dictionary<string, int>)magicMethod.Invoke(WordsCountIstance, new object[] { whole_text });
            }
            else if (ChosenMethod == "2")
            {
                //Вызываем из библиотеки WordsCountLib публичный метод подсчёта слов из текста с многопоточностью
                var WordsCountIstance = new WordsCountingClass();
                wordCount = WordsCountIstance.CountWordsMultithread(whole_text);
            }
            else
            {
                //Отправляем текст для подсчёта на веб-сервер
                wordCount = CountWordsWCF(whole_text);
            }


            // Выводим топ-10 слов
            printTop(wordCount, 7);


            //Записываем результат в файл
            Console.WriteLine("Now please enter the path to the folder where you want to save the results.\nor\nJust press Enter if you want to use the default path \"D:\\\"");
            path = Console.ReadLine();
            if (path == "") path = @"D:\";
            Console.WriteLine($"Full results are available in a txt file here: {path}\\results.txt");
            File.WriteAllLines($"{path}\\results.txt", wordCount.Select(x => x.Key + " " + x.Value));

            // Ждём реакции пользователя
            Console.WriteLine("Press any key to close the console app");
            Console.ReadKey();

        }

       /* 
        private static Dictionary<string, int> CountWordsWCF(string text)
        {
            ServiceReference1.CompositeType ret = null;
            Dictionary<string, int> dict;
            using (var client = new ServiceReference1.Service1Client())
            {
                ret = client.GetDataUsingDataContract(new ServiceReference1.CompositeType
                {
                    BoolValue = true,
                    StringValue = text
                });
            }
            dict = ret.Dict;
            return dict;
        }*/
        
        //метод отправляет текст и принимает словарь
        private static Dictionary<string, int> CountWordsWCF(string text)
        {
            Dictionary<string, int> dict;
            using (var client = new ServiceReference1.Service1Client())
            {
                dict = client.GetData(text);
            }
            return dict;
        }
        
        //метод выводит первые n пар словаря на консоль
        private static void printTop(Dictionary<string, int> dict, int n = 5)
        {
            Console.WriteLine($"Here's top {n} most frequent words from the book:");
            int count = 0;
            foreach (var item in dict)
            {
                Console.WriteLine("{0}   {1}", item.Key, item.Value);
                count++;
                if (count == n) break;
            }
        }
    }
}
