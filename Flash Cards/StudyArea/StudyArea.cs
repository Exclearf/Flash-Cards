using ConsoleTableExt;
using Flash_Cards.FlashCardData.Models;
using Flash_Cards.FlashCardData.Stacks;
using Flash_Cards.StudyArea.DTO;
using Flash_Cards.StudyArea.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash_Cards.StudyArea
{
    public class StudyAreaMain
    {
        private List<FlashCard> list { get; set; } = new();
        private Random rand = new Random();
        private int CurrentLangID { get; set; }
        private string CurrentLangName = "None";

        public StudyAreaMain() {
            ShowMenu();
        }
        private void OpenChosenMenu(string c)
        {
            Console.Clear();
            switch(c)
            {
                case "0":
                    Program.ShowMenu();
                    break;
                case "1":
                    StartStudySession();
                    break;
                case "2":
                    ShowStudySessions();
                    break;
                default:
                    IncorrectInputMessage(ShowMenu);
                    break;
            }
        }

        private void ShowStudySessions()
        {
            Console.Clear();
            ManageStacks.DisplayStacks();
            if (!ManageStacks.hasRows)
            {
                Console.Write("Press any key to continue...");
                Console.ReadKey();
                ShowMenu();
            }
            Console.Write("\n\nChoose from what language to display (0 for all): ");
            var choice = Console.ReadLine().Trim().ToLower();
            var cmd = Program.connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM study_sessions WHERE language_name='{choice}'";
            var reader = cmd.ExecuteReader();
            var studySessionsList = new List<StudySession>();
            var studySessionsDTOList = new List<DTOStudySession>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    studySessionsList.Add(new StudySession
                    {
                        Id = reader.GetInt32(0),
                        LangName = reader.GetString(1),
                        StartTime = DateTime.Parse(reader.GetString(2)),
                        EndTime = DateTime.Parse(reader.GetString(3)),
                        Correct = reader.GetString(4),
                    });
                    studySessionsDTOList.Add(new DTOStudySession
                    {
                        Name = studySessionsList.Last().LangName,
                        Start = studySessionsList.Last().StartTime,
                        End = studySessionsList.Last().EndTime,
                        Correctness = studySessionsList.Last().Correct,
                    });
                }
            }
            reader.Close();
            Console.Clear();
            ConsoleTableBuilder.From(studySessionsDTOList).WithTitle(CurrentLangName).ExportAndWriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            ShowMenu();  
        }

        private void StartStudySession()
        {
            Console.Clear();
            ManageStacks.DisplayStacks();
            if (!ManageStacks.hasRows)
            {
                Console.Write("Press any key to continue...");
                Console.ReadKey();
                ShowMenu();
            }
            Console.Write("\n\nChoose from what language to display: ");
            var choice = Console.ReadLine().Trim().ToLower();
            var cmd = Program.connection.CreateCommand();
            cmd.CommandText = $"SELECT language_id FROM languages WHERE name='{choice}'";
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    CurrentLangID = reader.GetInt32(0);
                }
            }
            reader.Close();
            ShowRandom(choice);
        }
        private void ShowRandom(string s)
        {
            CurrentLangName = s;
            Console.Clear();
            Console.Write(@$"------------------------
Current stack: {s}  (ID: {CurrentLangID})
To go back enter 0
------------------------
1. View X Flash Cards
2. View All Flash Cards
Input: ");
            var choice = Console.ReadLine().Trim().ToLower();
            var cmd = Program.connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM flash_card WHERE language_id={CurrentLangID}";
            var reader = cmd.ExecuteReader();
            list = new();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    list.Add(new FlashCard
                    {
                        Id = reader.GetInt32(0),
                        Front = reader.GetString(2),
                        Back = reader.GetString(3), 
                    });
                }
            }
            reader.Close();
            var count = Program.connection.CreateCommand();
            count.CommandText = $"SELECT COUNT(*) FROM flash_card WHERE language_id={CurrentLangID}";
            var amountOfCards = (Int32)(count.ExecuteScalar());
            switch (choice)
            {
                case "0":
                    ShowMenu();
                    break;
                case "1":
                    amountOfCards = rand.Next(0, amountOfCards);
                    if(amountOfCards == 0 || amountOfCards < 1)
                    {
                        Console.Clear();
                        Console.WriteLine("Not enough Flash Cards found!\nPress any key to continue...");
                        Console.ReadKey();
                        ShowMenu();
                    }
                    ShowCards(amountOfCards);
                    break;
                case "2":
                    ShowCards(amountOfCards);
                    break;
                default:
                    IncorrectInputMessage(ShowMenu);
                    break;
            }
        }
        private void ShowCards(int end)
        {
            if (end < 0)
            {
                Console.WriteLine("Something went wrong!\nPress any key to continue...");
                Console.ReadKey();
                ShowMenu();
            }
            int correct = 0;
            var shuffledcards = list.OrderBy(a => rand.Next()).ToList();
            DateTime startDate = DateTime.Now;
            for (int i = 0; i < end; i++)
            {
                Console.Clear();
                ConsoleTableBuilder.From((new[] { new { shuffledcards[i].Front } }.ToList())).WithTitle(CurrentLangName).ExportAndWriteLine();
                Console.Write("\n\nYou answer: ");
                var answer = Console.ReadLine().Trim().ToLower();
                if (answer.ToLower() == shuffledcards[i].Back.ToLower())
                {
                    correct++;
                    Console.WriteLine($"\n\nYou are correct!");
                }
                else
                    Console.WriteLine("\n\nYou are incorrect!");
                Console.WriteLine($"You answer is {answer}. (Correct: {shuffledcards[i].Back})\n\nPress anything to continue...");
                Console.ReadKey();
            }
            DateTime endTime = DateTime.Now;
            if (Program.sql.SQLExecute($"INSERT INTO study_sessions VALUES('{CurrentLangName}','{startDate.ToString()}','{endTime.ToString()}', '{correct + "/" + end}')") < 1)
            {
                Console.WriteLine("Failed to create Study Session!");
                Console.ReadKey();
                ShowMenu();
            }
            Console.Clear();
            Console.WriteLine($"You got {correct} out of {end} cards correct!\n\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
            ShowMenu();
        }
        private void IncorrectInputMessage(Action func)
        {
            Console.Clear();
            Console.WriteLine("Wrong input!\nPress anything to continue...");
            Console.ReadKey();
            func();
        }
        private void ShowMenu()
        {
            Console.Clear();
            Console.Write($@"--------STUDY AREA--------
0. Back
1. Start Study Session
2. List all Study Sessions
------------------
Input: ");
            var choice = Console.ReadLine();
            if (choice != null)
                OpenChosenMenu(choice);
        }
    }
}
