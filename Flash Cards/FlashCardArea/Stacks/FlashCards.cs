using ConsoleTableExt;
using Flash_Cards.FlashCardData.DTO;
using Flash_Cards.FlashCardData.Models;
using System.Globalization;

namespace Flash_Cards.FlashCardData.Stacks
{
    internal class FlashCards
    {
        public static bool hasRows { get; private set; } = false;
        public static List<FlashCard> list { get; private set; } = new();
        public static List<DTOFlashCard> dtolist { get; private set; } = new();
        private static void GoToChosenMethod(string? menuChoice)
        {
            string choice = "";
            switch (menuChoice)
            {
                case "0":
                    Program.ShowMenu();
                    break;
                case "1":
                    Console.Clear();
                    ManageStacks.DisplayStacks();
                    if (ManageStacks.hasRows == false)
                    {
                        DisplayError();
                    }
                    Console.Write("\n\nChoose a Stack (0 to go back): ");
                    choice = Console.ReadLine().Trim().ToLower();
                    Console.Clear();
                    if (ManageStacks.list.Exists(x => x.Name == choice))
                    {
                        AddFlashCard(choice);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        ManageFlashCardsMenu();
                    }
                    else
                    {
                        if (choice == "0")
                            ManageFlashCardsMenu();
                        DisplayError();
                    }
                    break;
                case "2":
                    Console.Clear();
                    EditFlashCard();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    ManageFlashCardsMenu();
                    break;
                case "3":
                    Console.Clear();
                    DeleteFlashCard();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    ManageFlashCardsMenu();
                    break;
                case "4":
                    Console.Clear();
                    DisplayFlashCards();
                    Console.ReadKey();
                    ManageFlashCardsMenu();
                    break;
                default:
                    DisplayError();
                    break;
            }
        }

        private static void DeleteFlashCard()
        {
            var stackChoice = DisplayFlashCards();
            if (stackChoice == "0")
                stackChoice = "all";
            Console.WriteLine($"------------------------\nCurrent stack: {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stackChoice.ToLower())}\nTo go back enter 0\n------------------------\n\n");
            Console.Write("\nEnter an ID to delete a flash card: ");
            int cardChoiceInput = int.Parse(Console.ReadLine().Trim());
            if (cardChoiceInput == 0)
                ManageFlashCardsMenu();
            int cardChoice = new int();
            try
            {
                cardChoice = list.Find(x => x.Id == cardChoiceInput).Id;
            }
            catch { DisplayError(); }
            if (Program.sql.SQLExecute($"DELETE FROM flash_card WHERE id = {cardChoice}") > 0)
                Console.WriteLine("\n\nSuccessfully deleted a Flash Card!");
            else
                Console.WriteLine("\n\nFailed to delete a Flash Card!");
        }

        private static void DisplayError()
        {
            Console.Clear();
            Console.WriteLine("Sorry, an error has happened. Try again...\nPress any key to continue...");
            Console.ReadKey();
            ManageFlashCardsMenu();
        }

        private static void EditFlashCard()
        {
            var stackChoice = DisplayFlashCards();
            if (stackChoice == "0")
                stackChoice = "all";
            Console.WriteLine($"------------------------\nCurrent stack: {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stackChoice.ToLower())}\nTo go back enter 0\n------------------------\n\n");
            Console.Write("\nEnter an ID to edit a flash card: ");
            int cardChoiceInput = int.Parse(Console.ReadLine().Trim());
            if (cardChoiceInput == 0)
                ManageFlashCardsMenu();
            int cardChoice = new int();
            try
            {
                cardChoice = list.Find(x => x.Id == cardChoiceInput).Id;
            }
            catch { DisplayError(); }
            Console.Write("\nFront of the Flash Card: ");
            string? front = Console.ReadLine();
            if (front == "0")
                ManageFlashCardsMenu();
            Console.Write("Back of the Flash Card: ");
            string? back = Console.ReadLine();
            if (back == "0")
                ManageFlashCardsMenu();
            if (Program.sql.SQLExecute($"UPDATE flash_card SET card_Front = '{front}', card_Back = '{back}' WHERE id = {cardChoice}") > 0)
                Console.WriteLine("\n\nSuccessfully edited a Flash Card!");
            else
                Console.WriteLine("\n\nFailed to edit a Flash Card!");
        }

        public static string DisplayFlashCards()
        {
            list = new();
            dtolist = new();
            var cmd = Program.connection.CreateCommand();
            ManageStacks.DisplayStacks();
            Console.Write("Choose from what language to display (0 for all): ");
            var choice = Console.ReadLine().Trim().ToLower();
            if (choice == "")
                choice = "0";
            if (choice == "0")
                cmd.CommandText = "SELECT * FROM flash_card";
            else
                cmd.CommandText = $"SELECT * FROM flash_card WHERE language_id={ManageStacks.list.Find(x => x.Name == choice).Id}";
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                int i = 1;
                while (reader.Read())
                {
                    list.Add(new FlashCard
                    {
                        Id = reader.GetInt32(0),
                        Front = reader.GetString(2),
                        Back = reader.GetString(3),
                    });
                    dtolist.Add(new DTOFlashCard
                    {
                        ID = i++,
                        Front = list.Last().Front,
                        Back = list.Last().Back,
                    });
                }
                Console.Clear();
                ConsoleTableBuilder.From(dtolist).ExportAndWriteLine();
                hasRows = true;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("No Flash Cards yet!\nPress anything to continue...");
                hasRows = false;
            }
            reader.Close();
            return choice;
        }

        private static void AddFlashCard(string choice)
        {
            Console.WriteLine($"------------------------\nCurrent stack: {choice}\nTo go back enter 0\n------------------------");
            Console.Write("\nFront of the Flash Card: ");
            string? front = Console.ReadLine();
            if (front == "0")
                ManageFlashCardsMenu();
            Console.Write("Back of the Flash Card: ");
            string? back = Console.ReadLine();
            if (back == "0")
                ManageFlashCardsMenu();
            if (Program.sql.SQLExecute($"INSERT INTO flash_card VALUES ({ManageStacks.list.Find(x => x.Name == choice).Id},'{front}','{back}')") > 0)
                Console.WriteLine("\n\nSuccessfully added a Flash Card!");
            else
                Console.WriteLine("\n\nFailed to add a Flash Card!");
        }

        public static void ManageFlashCardsMenu()
        {
            Console.Clear();
            Console.Write($@"--------MANAGE FLASH CARDS--------
0. Back
1. Add a Flash Card
2. Edit a Flash Card
3. Delete a Flash Card
4. List Flash Cards
----------------------------------
Input: ");
            var menuChoice = Console.ReadLine();
            GoToChosenMethod(menuChoice);
        }
    }
}