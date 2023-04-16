# Simple Flash-Cards app.

 - This is an application where the users will create Stacks of Flashcards.
 - Two different tables for stacks and flashcards. The tables should be linked by a foreign key.
 - Stacks must have an unique name.
 - Every flashcard is a part of a stack. If a stack is deleted, the same happens with the flashcards.
 - DTOs are being used to show the flashcards to the user without the Id of the stack it belongs to.
 - When showing a stack to the user, the flashcard Ids start with 1 without gaps between them. If you have 10 cards and number 5 is deleted, the table now will show Ids from 1 to 9.
 - All study sessions are stored, with date and score.
 - The study and stack tables are linked. If a stack is deleted, it's study sessions is deleted.
 - The project contains a call to the study table so the users can see all their study sessions.
