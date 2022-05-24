using System;
using Telegram.Bot;

namespace TranslaterConsole
{
    internal class Program
    {
        static TelegramBotClient Bot;
        static Tutor Tutor = new Tutor();
        static Dictionary<long, string> LastWord = new Dictionary<long, string>();
        const string COMMAND_LIST =
@"Список команд:
/add <eng> <rus> - добавление английского слова и его перевод в словарь
/get - получаем случайное английское слово из словаря
/check - <eng> <rus> - проверяем правильность перевода английского слова
";

        private static void Main(string[] args)
        {
            //@LanguageTutorBrainXBot

            Bot = new TelegramBotClient("5307170375:AAEbJQB_5ivplaPAGiePp5DKglVLGyhhpZI");

            Bot.OnMessage += Bot_OnMessage;
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();

            //var me = Bot.GetMeAsync().Result;
            //Console.WriteLine(me.FirstName);

            Console.ReadKey();

            //var tutor = new Tutor();
            //tutor.AddWord("rat", "крыса");
            //tutor.AddWord("dog", "собака");
            //tutor.AddWord("cat", "кошка");
            //tutor.AddWord("rabbit", "кролик");

            //while (true)
            //{
            //    var word = tutor.GetRandomEngWord();
            //    Console.WriteLine($"Как переводится слово: {word}?");
            //    var userAnswer = Console.ReadLine();
            //    if (tutor.CheckWord(word, userAnswer))
            //        Console.WriteLine("Правильно!\n");
            //    else
            //    {
            //        var correctAnswer = tutor.Translate(word);
            //        Console.WriteLine($"Неверно. Правильный ответ: {correctAnswer}\n");
            //    }
            //}
        }

        private static async void Bot_OnMessage(object? sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e == null || e.Message == null || e.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return;

            Console.WriteLine($"{e.Message.From.Username}[{e.Message.From.Id}]: {e.Message.Text}");
            var userId = e.Message.From.Id;
            var msgArgs = e.Message.Text.Split(' ');
            String text;
            switch (msgArgs[0])
            {
                case "/start":
                    text = COMMAND_LIST;
                    break;
                case "/add":
                    text = AddWords(msgArgs);
                    break;
                case "/get":
                    text = GetRandomEngWord(userId);
                    break;
                case "/check":
                    text = CheckWord(msgArgs);
                    var newWord = GetRandomEngWord(userId);
                    text = $"{text}\r\nСледующее слово: {newWord}";
                    break;
                default:
                    if (LastWord.ContainsKey(userId))
                    {
                        text = CheckWord(LastWord[userId], msgArgs[0]);
                        newWord = GetRandomEngWord(userId);
                        text = $"{text}\r\nСледующее слово: {newWord}";
                    }
                    else
                        text = COMMAND_LIST;
                    break;
            }
            await Bot.SendTextMessageAsync(e.Message.From.Id, text);
        }

        private static string GetRandomEngWord(long userId)
        {
            var text = Tutor.GetRandomEngWord();
            if (LastWord.ContainsKey(userId))
                LastWord[userId] = text;
            else
                LastWord.Add(userId, text);

            return text;
        }

        private static string CheckWord(string[] msgArgs)
        {
            if (msgArgs.Length != 3)
                return "Неправильное количество аргументов. Их должно быть 2";
            else
            {
                return CheckWord(msgArgs[1], msgArgs[2]);
            }
        }

        private static string CheckWord(string eng, string rus)
        {
            if (Tutor.CheckWord(eng, rus))
                return "Правильно!";
            else
            {
                var correctAnswer = Tutor.Translate(eng);
                return $"Неверно. Правильный ответ: \"{correctAnswer}\".";
            }
        }

        private static string AddWords(String[] msgArgs)
        {
            if (msgArgs.Length != 3)
                return "Неправильное количество аргументов. Их должно быть 2";
            else
            {
                Tutor.AddWord(msgArgs[1], msgArgs[2]);
                return "Новое слово добавлено в словарь";
            }
        }
    }
}