using ConsoleTableExt;
using Flash_Cards.FlashCardData.DTO;
using Flash_Cards.FlashCardData.Models;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Flash_Cards.FlashCardData.Stacks
{
    internal class ManageStacks
    {
        public static List<Languages> list { get; private set; } = new();
        public static List<DTOLanguages> dtolist { get; private set; } = new();
        public static bool hasRows { get; private set; } = false;
        
        public static void ManageStacksMenu()
        {
            Console.Clear();
            Console.Write($@"--------MANAGE STACKS--------
0. Back
1. Add a Stack
2. Edit a Stack
3. Delete a Stack
4. List Stacks
-----------------------------
Input: ");
            var choice = Console.ReadLine();
            RouteChoice(choice);
        }

        static private void RouteChoice(string s)
        {
            string choice = "";
            switch (s)
            {
                case "0":
                    Program.ShowMenu();
                    break;
                case "1":
                    Console.Clear();
                    Console.Write("Enter the name of the language: ");
                    InsertStack();
                    Console.ReadKey();
                    ManageStacksMenu();
                    break;
                case "2":
                    Console.Clear();
                    DisplayStacks();
                    if (!hasRows)
                    {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        ManageStacksMenu();
                    }
                    Console.Write("\n\nEnter the name of the language to update: ");
                    choice = Console.ReadLine().Trim().ToLower();
                    Console.Write("\nEnter new name for the stack: ");
                    string newName = Console.ReadLine().Trim().ToLower();
                    UpdateStack(choice, newName);
                    Console.ReadKey();
                    ManageStacksMenu();
                    break;
                case "3":
                    Console.Clear();
                    DisplayStacks();
                    if (!hasRows)
                    {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        ManageStacksMenu();
                    }
                    Console.Write("\n\nEnter the language to delete: ");
                    choice = Console.ReadLine().Trim().ToLower();
                    DeleteStack(choice);
                    Console.ReadKey();
                    ManageStacksMenu();
                    break;
                case "4":
                    Console.Clear();
                    DisplayStacks();
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    ManageStacksMenu();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Sorry, wrong input. Try again...\nPress any key to continue...");
                    Console.ReadKey();
                    ManageStacksMenu();
                    break;
            }
        }

        private static void UpdateStack(string name, string new_name)
        {
            if (Program.sql.SQLExecute($"UPDATE languages SET name='{new_name}' WHERE name='{name}'") > 0)
                Console.WriteLine("\n\nSuccessfully updated!\nPress any key to continue...");
            else
                Console.WriteLine("\n\nFailed to update!\nPress any key to continue...");
        }

        private static void DeleteStack(string name)
        {
            if (Program.sql.SQLExecute($"DELETE FROM languages WHERE name='{name}'") > 0)
                Console.WriteLine("\n\nSuccessfully deleted!\nPress any key to continue...");
            else
                Console.WriteLine("\n\nFailed to delete!\nPress any key to continue...");
        }

        private static void InsertStack()
        {
            var language_name = Console.ReadLine();
            if (Program.sql.SQLExecute($"INSERT INTO languages VALUES ('{language_name}')") > 0)
                Console.WriteLine("\n\nSuccessfully added!");
            else
                Console.WriteLine("\n\nFailed to add!");
            Console.WriteLine("\nPress any key to continue...");
        }

        public static void DisplayStacks()
        {
            list = new();
            dtolist = new();
            var cmd = Program.connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM languages";
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //improve this part
                // remove creating double lists
                while (reader.Read())
                {
                    list.Add(new Languages
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                    });
                    dtolist.Add(new DTOLanguages
                    {
                        Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(list.Last().Name.Trim().ToLower()),
                    });
                }
                ConsoleTableBuilder.From(dtolist).ExportAndWriteLine();
                hasRows = true;
            }
            else
            {
                Console.WriteLine("\n\nNo records!");
                hasRows = false;
            }
            reader.Close();
        }
    }
}