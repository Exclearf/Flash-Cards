using ConsoleTableExt;
using Flash_Cards.FlashCardData.SQL;
using Flash_Cards.FlashCardData.Stacks;
using Flash_Cards.StudyArea;
using System.Configuration;
using System.Data.SqlClient;

namespace Flash_Cards
{
    internal class Program
    {
        private static string connectionString = ConfigurationManager.AppSettings["connectionString"].ToString();

        private static string?[] all_keys = ConfigurationManager.AppSettings.AllKeys;

        public static DBConnection sql { get; private set; } = new DBConnection();

        public static SqlConnection connection { get; private set; } = sql.SQLConnect(connectionString);
        static void Main(string[] args)
        {
            foreach (var key in all_keys)
            {
                if (key.Split("_").Last() == "db")
                {
                    sql.SQLExecute(ConfigurationManager.AppSettings[key].ToString());
                }
            }
            Console.Clear();
            ShowMenu();
            connection.Close();
        }

        private static void GoToChosenMethod(string? menuChoice)
        {
            Console.Clear();
            switch (menuChoice)
            {
                case "0":
                    Console.WriteLine("Was nice having you here!");
                    Environment.Exit(0);
                    break;
                case "1":
                    ManageStacks.ManageStacksMenu();
                    break;
                case "2":
                    FlashCards.ManageFlashCardsMenu();
                    break;
                case "3":
                    StudyAreaMain sarea = new();
                    break;
                default:
                    Console.WriteLine("Wrong input!\nPress any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }


        public static void ShowMenu()
        {
            Console.Clear();
            Console.Write($@"--------MENU--------
0. Exit
1. Manage Stacks
2. Manage Flash Cards
3. Study
------------------
Input: ");
            var menuChoice = Console.ReadLine();
            GoToChosenMethod(menuChoice);
        }
    }
}