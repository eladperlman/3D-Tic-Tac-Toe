//Author Name: Elad Perlman
//File Name: MainScene.cs
//Project Name: PASS1
//Date Created: September 22nd, 2020
//Date Modified: September 29th, 2020
//Description: This Game is a 2 player 3D version of tic tac toe, where there is a 3 by 3 by 3 board that contains 27 different spots where players can drop their pieces
//Turns alternate every move and each player gets either an X or O as their piece, but X always begins the game. To win the game, one of the player's needs to get a line 
//containing the same type of piece.
using System;
using System.Linq;

namespace PASS1
{
    class MainScene
    {
        //Stores a random variable
        private static Random rnd = new Random();
        
        //Stores the menu's state and all of its possible states
        private static byte menu = 1;    
        private const byte START_MENU = 1;
        private const byte EXIT = 2;
        private const byte GAME = 0;

        //Stores the x, y, and z positions of the user's current piece
        private static int row = 0;
        private static int col = 0;
        private static int curLevel = 0;

        //Stores the number of turns that happened during the game and the mininum amount of turns for a win to happen
        private static int turn = 0;
        private const byte MIN_TURNS = 5;
        
        //Stores the x, y, and z dimensions of the tic tac toe board
        private const byte DIM = 3;
        
        //Stores all the 3D positions of each space on the board
        private static string[,,] boardSpace = new string[3, 3, 3];
        
        //Stores all the characters that make up the board
        private static string[] boardChar = { "\\", " ", "+", "-" };
        private static string[] piece = {"X", "O" };

        //Stores the values of the current player and his assigned piece
        private static int curPlayer;
        private static int curPiece = 0;

        private static void Main(string[] args)
        {
            //Calls the ResetBoard method
            ResetBoard();

            //Randomly Chooses the player number that will start  and stores the value inside curPlayer
            curPlayer = rnd.Next(1, 3);

            //Code block runs while menu does not equal EXIT
            while (menu != EXIT)
            {
                //Clears console
                Console.Clear();

                //Checks if menu state is the start menu
                if (menu == START_MENU)
                {
                    //Calls the MenuDisplay method
                    MenuDisplay();

                    //Checks if menu state equals EXIT, if so continue to the next iteration of the loop
                    if (menu == EXIT) 
                    {    
                        continue;
                    }
                    
                    //Sets menu state to GAME
                    menu = GAME;
                }

                //Displays the current player and his piece, as well as the tic tac toe board
                Console.WriteLine("Current Player: Player " + curPlayer + "\tUsing: " + piece[curPiece] + "\n");
                BuildBoard();

                //Checks if the minimum turns for a win to occur happened yet, and if the user's piece was not in a full level
                if (turn >= MIN_TURNS && curLevel >= 0)
                {
                    //Checks if the current piece made a successful line
                    if (LineCheck(curLevel, row, col))
                    {
                        //Displays the winner of the game and the option to press ENTER to go back to menu
                        Console.WriteLine("Player " + curPlayer + " Won! Press ENTER to go back to the menu");
                        Console.ReadLine();
                        
                        //Calls the ResetGame method, and continues to the next iteration of the loop
                        ResetGame();
                        continue;
                    }
                    
                    //Checks if a tie happened
                    if (TieCheck())
                    {
                        //Displays It's a tie! and the option to go back to menu
                        Console.WriteLine("Its a Tie! Press ENTER to go back to menu");
                        Console.ReadLine();

                        //Calls the ResetGame method, and continues to the next iteration of the loop
                        ResetGame();
                        continue;
                    }
                }

                //Displays all the row options the user can choose from
                Console.WriteLine("Enter the number that corresponds to the row you would like to drop your piece: ");
                Console.WriteLine("1. Top\n2. Middle\n3. Bottom");
                
                //Checks if the user entered a non integer value, or if the value was negative or does not correspond to a row
                if (!Int32.TryParse(Console.ReadLine(), out row) || row <= 0 || row > DIM)
                {
                    //Displays that the user entered a non valid input, and then continues to the next iteration of the loop
                    Console.WriteLine("Entered row must be a number between 1 and 3 , please retry");
                    Console.ReadLine();
                    continue;
                }

                //Decrements 1 from row
                row--;

                //Displays all the column options the user can choose from
                Console.WriteLine("Enter the number that corresponds to the column you would like to drop your piece: ");
                Console.WriteLine("1. Left\n2. Middle\n3. Right");

                //Checks if the user entered a non integer value, or if the value was negative or does not correspond to a column
                if (!Int32.TryParse(Console.ReadLine(), out col) || col <= 0 || col > DIM)
                {
                    //Displays that the user entered a non valid input, and then continues to the next iteration of the loop
                    Console.WriteLine("Entered column must be a number between 1 and 3 , please retry");
                    Console.ReadLine();
                    continue;
                }

                //Decrements col by 1
                col--;

                //Assigns a value for the current level based on the level the piece landed on
                curLevel = DropPiece(row, col, piece[curPiece]);
                
                //Checks if the level the piece was dropped in is full, if so it continues to the next iteration
                if (curLevel < 0)
                {
                    continue;
                }

                //Increments turn by 1
                turn++;

                //Checks if player hasn't won, if so switch the current player and its piece
                if (!LineCheck(curLevel, row, col))
                {
                    curPiece = (curPiece * -1) + 1;
                    curPlayer = (curPlayer * -1) + 3;
                }
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Displays the game's instructions and description, as well as checks if the player wants to quit the game, if so it sets menu state to EXIT
        private static void MenuDisplay()
        {
            //Displays game description and instructions
            Console.WriteLine("Welcome to 3D Tic-Tac-Toe\n\nBe the first to place 3 of your letter in a sequence, in any direction.");
            Console.WriteLine("X always goes first, but player order is randomized.\n");
            Console.WriteLine("Enter e to exit the game, or press any other key to start the game");
            
            //Checks if user wants to quit the game, if so game state gets set to EXIT, then console clears
            if (Console.ReadLine() == "e")
            {
                menu = EXIT;
            } 
            Console.Clear();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Builds the 3D tic tac toe board
        private static void BuildBoard()
        {
            //Loops through every level in the 3D board
            for (int level = 0; level < DIM; level++)
            {
                //Stores the indentation variable
                string space = " ";

                //Calls DrawBorderLine with boardChar 2 and 3 as the parameters and adds an indent to the cursor
                DrawBorderLine(boardChar[2], boardChar[3]);
                Console.Write(space);

                //Loops through every row in current level
                for (int row = 0; row < DIM; row++)
                {
                    //Calls DrawPieceLine with boardChar 0 and 1, level, and row as the parameters, and adds an indent to the cursor
                    DrawPieceLine(boardChar[0], boardChar[1], level, row);
                    Console.Write(space + " ");
                    
                    //Calls DrawBorderLine with boardChar2 and 3as the parameters
                    DrawBorderLine(boardChar[2], boardChar[3]);
                    
                    //Adds two space values to variable space and moves the cursor by that amount
                    space += "  ";
                    Console.Write(space);
                }

                //Sets the cursor's position to the beginning of the line
                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }

        //Pre: firstChar and secondChars must not be null and type string
        //Post: N/A
        //Description: Draws the line that borders the possible places where a piece can be dropped
        private static void DrawBorderLine(string firstChar, string secondChar)
        {
            //Displays the first character on a line
            Console.Write(firstChar);
            
            //Loops through all the columns in the board, and for each it displays a line consisting the two characters given
            for (int i = 0; i < DIM; i++)
            {
                Console.Write(secondChar + secondChar + secondChar + firstChar);
            }

            //Moves cursor to the next line
            Console.WriteLine();
        }

        //Pre: firstChar and secondChars must not be null and type string
        //Post: N/A
        //Description: Draws the line that includes the possible places where a piece can be dropped
        private static void DrawPieceLine(string firstChar, string secondChar, int level, int row)
        {
            //Displays the first character on a line
            Console.Write(firstChar);

            //Loops through all the columns in the board, and for each it displays a line consisting the two characters given
            for (int i = 0; i < DIM; i++)
            {
                Console.Write(secondChar + boardSpace[level, row, i] + secondChar + firstChar);
            }

            //Moves cursor to the next line
            Console.WriteLine();
        }

        //Pre: N/A
        //Post: N/A
        //Description: Loops through all the board positions and replaces their value with a space
        private static void ResetBoard()
        {
            for (int i = 0; i < DIM; i++)
            {
                for (int j = 0; j < DIM; j++)
                {
                    for (int n = 0; n < DIM; n++)
                    {
                        boardSpace[i, j, n] = boardChar[1];
                    }
                }
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Resets all the values that were altered during the game to their original values
        private static void ResetGame()
        {
            ResetBoard();
            turn = 0;
            curPiece = 0;
            menu = 1;
            curPlayer = rnd.Next(1, 3);
        }

        //Pre: row and col are positive integers that are between 0 and 2 inclusive, userChar is a string thats not null
        //Post: returns the level which the piece can be dropped at
        //Description: Checks which level the user's piece can be dropped or if it can be at all, and returns the value of the level
        private static int DropPiece(int row, int col, string userChar)
        {
            //Iterates through every level starting from the bottom level
            for (int i = DIM - 1; i >= 0; i--)
            {
                //Checks if a piece is present in the current level, if so it continues to the next iteration
                if (boardSpace[i, row, col] != boardChar[1])
                {
                    continue;
                }

                //Assigns the current position's value with the user's piece and returns the level it is on
                boardSpace[i, row, col] = userChar;
                return i;
            }

            //Displays that all the levels are full, waits for the user to press a button, then returns -1 which represents a full level
            Console.WriteLine("Level is full, press any button to retry");
            Console.ReadLine();
            return -1;
        }

        //Pre: z, y, and x are all positive integers that are between 0 and 2 inclusive
        //Post: Returns if the current piece is in a line
        //Description: Checks if the current piece makes a line with another 2 pieces with the same value, and returns the result
        private static bool LineCheck(int z, int y, int x)
        {
            //Loops through half of all the possible adjacents of the current piece
            for (int i = 1; i <= 17; i++)
            {
                //Stores the x, y, and z directions from the current piece
                int[] adDir = new int[3];
                
                //Loops through all the elements of adDir and sets them to their proper value, and checks if any of them equal to 2, if so it replaces their value with -1
                for (int j = 0; j < DIM; j++)
                {
                    adDir[j] = Base10toBase3(i)[j];
                    
                    if (Base10toBase3(i)[j] == 2)
                    {
                        adDir[j] = -1;
                    }
                }
              
                //Checks if a line was made with two blocks in the same direction from the current piece, if so return true
                if (CheckDirection(adDir[0], adDir[1], adDir[2], 1, 2))
                {
                    return true;
                }

                //Checks if a line was made with two blocks in the opposite directions from eachother from the current piece, if so return true
                if (CheckDirection(adDir[0], adDir[1], adDir[2], -1, 1))
                {
                    return true;
                }

                //Checks if a line was made with two blocks from the opposite direction of the current piece, if so return true
                if (CheckDirection(adDir[0], adDir[1], adDir[2], -1, -2))
                {
                    return true;
                }

                //elimniates the 4 repeated lines by skipping the 2nd, 3rd, 4th, and 5th iterations
                if (i == 1)
                {
                    i = 5;
                }
            }
       
            //returns false meaning that no lines included the current piece
            return false;
   
            //Pre: Adz, y, and x are all integers between -1 and 1 inclusive, and dirMult1 and 2 are valid integers
            //Post: Returns if a line containing the current piece exists
            //Description: The program checks if a line exists with the current piece and two other blocks that are found through the dirMult 1 and 2, and returns the result
            bool CheckDirection(int adZ, int adY, int adX, int dirMult1, int dirMult2)
            {
                //Checks if the adjacent piece exists on the board, if its x, y, and z components are between 0 and 2 inclusive
                if (adZ * dirMult1 + z >= 0 && adZ * dirMult1 + z < DIM &&
                            adY * dirMult1 + y >= 0 && adY * dirMult1 + y < DIM &&
                            adX * dirMult1 + x >= 0 && adX * dirMult1 + x < DIM)
                {
                    //Checks if the adjacent piece has the same value as the current piece
                    if (boardSpace[z + adZ * dirMult1, y + adY * dirMult1, x + adX * dirMult1] == boardSpace[z, y, x])
                    {
                        //Checks if the 3rd piece exists on the board, if its x, y, and z components are between 0 and 2 inclusive
                        if (adZ * dirMult2 + z >= 0 && adZ * dirMult2 + z < DIM &&
                            adY * dirMult2 + y >= 0 && adY * dirMult2 + y < DIM &&
                            adX * dirMult2 + x >= 0 && adX * dirMult2 + x < DIM)
                        {
                            //Checks if the 3rd piece has the same value as the current piece, if so return true
                            if (boardSpace[z + adZ * dirMult2, y + adY * dirMult2, x + adX * dirMult2] == boardSpace[z, y, x])
                            {
                                return true;
                            }
                        }
                    }
                }

                //Returns that a line doesn't include the current piece and the other two pieces based on the direction vectors
                return false;
            }
        }

        //Pre: N/A
        //Post: Returns if a tie exists
        //Description: Checks if the top board's pieces all have values that aren't a space, then returns the result
        private static bool TieCheck()
        {
            //Iterates through the base10 values of all the top board's positions
            for (int i = 0; i < Math.Pow(DIM, 2); i++)
            {
                //Stores the x y and z values of the current piece in the loop
                int z = Base10toBase3(i)[0];
                int y = Base10toBase3(i)[1];
                int x = Base10toBase3(i)[2];

                //Checks if the piece's value is a space, if so return false
                if (boardSpace[z, y, x] == boardChar[1])
                {
                    return false;
                }       
            }

            //Returns that there is a tie
            return true;
        }

        //Pre: decNum is a positive integer between 0 and 26 inclusive
        //Post: Returns an array containing 3 elements, the x y and z positions
        //Description: This function converts a base 10 number between 0 and 26 to a base 3 number and then returns it
        private static int[] Base10toBase3(int decNum)
        {
            //Stores the x, y, and z positions, and the current index of the array
            int[] base3Num = { 0, 0, 0 };
            int index = DIM - 1;

            //code executes while the decimal number is greater than 0 
            while (decNum > 0)
            {                            
                //adds the remainder of the number when divided by 3 to base3Num's corresponding element
                base3Num[index] = decNum % DIM;
                            
                //divides decimal number by 3
                decNum /= DIM;

                //decrements index by 1 
                index--;
            }

            //returns the final base 3 number
            return base3Num;
        }
    }
}