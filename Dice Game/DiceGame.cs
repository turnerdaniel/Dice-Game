using System;
using System.Collections.Generic; //used for lists
using System.Linq; //used for counting number of occurences within an array

namespace Dice_Game
{
    class Game //Game class
    {
        static uint scoreLimit = 50; //unsigned integer for setting score limit. Does not allow for a negative score limit to be entered.
        static uint ScoreLimit //private accessor for score limit variable
        {
            set
            {
                if (value <= 0) //checks to see if entered score limit value is less than or equal to 0
                {
                    scoreLimit = 50; //sets value to 50 by default
                    Console.WriteLine("Score limit has defaulted to 50."); //alerts user that score value is set to 50.
                }
                else
                    scoreLimit = value; //if the score limit is valid, it is assigned to the score limit variable
            }
        } 
        static int player = 1; //used for counter that decides current player
        static byte currentPlayer; //holds current player as either a 1 or 2.
        static int player1Turns = 0; //holds number of turns for player 1
        static int player2Turns = 0; //hold number of turns for player 2
        static int total = 0; //holds the total number of turns
        static double player1Average = 0; //holds the average roll for player 1
        static double player2Average = 0; //holds the average roll for player 2
        static ConsoleKeyInfo userSelection; //holds the key pressed for rolling dice
        static bool validLimit = false; //holds flag to see if a valid score limit has been entered

        static void Main(string[] args) //entry point
        {
            //decalarations 
            Random rand = new Random(); //initialise instance of random class
            Die Die1 = new Die(rand); //initilase 5 instances of die class to create 5 dice
            Die Die2 = new Die(rand); //pass random number to each object so that the number is unique
            Die Die3 = new Die(rand);
            Die Die4 = new Die(rand);
            Die Die5 = new Die(rand);
            Player player1 = new Player(); //initialise 2 instance of player class to create 2 players
            Player player2 = new Player();

            int[] currentRoll = new int[5]; //initialise integer array with a length of 5 elements for use with holding the current roll of all 5 dice
            List<int[]> player1History = new List<int[]>(); //intialise list of integer arrays to hold the roll history of player 1 and player 2
            List<int[]> player2History = new List<int[]>();

            //code
            do //start of do while loop
            {
                Console.Write("Please enter the score limit for this game: ");
                try //code that should be executed
                {
                    ScoreLimit = Convert.ToUInt32(Console.ReadLine()); //takes entered value and converts it into an unsigned integer to be stored in the scoreLimit variable
                    validLimit = true; //allows the loop to be exited
                }
                catch (FormatException) //catches inputs of letters and other non-numeric values
                {
                    Console.WriteLine("Please only enter numbers.");
                    //loop continues
                }
                catch (OverflowException) //catches inputs outside the range of unisigned integers (negative to more than 4,294,967,295)
                {
                    Console.WriteLine("Please only enter numbers greater than 0 and less than 4,294,967,295.");
                    //loop continues
                }
            } //end of do while loop
            while (validLimit == false); //loop runs while the score limit is not valid. Repeats until a valid limit is set. 

            while (player1.Score < scoreLimit && player2.Score < scoreLimit) //while loop for taking turns //exits only when score limit reached
            {
                Console.WriteLine(""); //Adds line space for better formatting
                checkCurrentPlayer(); //executes method

                Console.Write("Press spacebar to roll the dice normally OR press any other key to throw dice once for x2 points."); //states how to throw and rules
                userSelection = Console.ReadKey(); //stores the user's key press as the userSelection variable

                Console.WriteLine("\nRolling dice...");  //Alerts user and rolls all 5 dice
                Die1.roll();
                Die2.roll();
                Die3.roll();
                Die4.roll();
                Die5.roll();

                currentRoll[0] = Die1.getNumberOnTop(); //stores each roll inside an array named currentRoll
                currentRoll[1] = Die2.getNumberOnTop();
                currentRoll[2] = Die3.getNumberOnTop();
                currentRoll[3] = Die4.getNumberOnTop();
                currentRoll[4] = Die5.getNumberOnTop();

                Console.WriteLine("{0} {1} {2} {3} {4}", Die1.getNumberOnTop(), Die2.getNumberOnTop(), Die3.getNumberOnTop(), Die4.getNumberOnTop(), Die5.getNumberOnTop());
                //outputs the rolls for each die

                switch (checkSelection(userSelection)) //switch case statement for executing code depending on whether spacebar was pressed or not
                {                                      //uses method to return numerical values
                    case 1: //spacebar pressed = normal throw
                        if (checkReroll(currentRoll) != -1) //checks to see if two of a kind is rolled. If -1 is returned, there is no two of a kind and the statemnt is skipped.
                        {
                            Console.Write("Two-of-a-kind detected! Press any key to rethrow the remaining dice."); //alert player
                            Console.ReadKey(); //allow them time to read and throw dice again

                            Console.WriteLine("\nRolling dice..."); //alert user of dice rolls
                            Console.WriteLine("The first two dice will contain the numbers from the previous roll."); //alert user that the first 2 dice contain the two of a kind
                            Die1.setNumberOnTop(checkReroll(currentRoll)); //set first 2 dice to the values of the two of a kind
                            Die2.setNumberOnTop(checkReroll(currentRoll)); //method used to return the values
                            Die3.roll(); //roll remaining 3 dice
                            Die4.roll();
                            Die5.roll();

                            currentRoll[0] = Die1.getNumberOnTop(); //overwrites each roll's new values inside an array named currentRoll
                            currentRoll[1] = Die2.getNumberOnTop();
                            currentRoll[2] = Die3.getNumberOnTop();
                            currentRoll[3] = Die4.getNumberOnTop();
                            currentRoll[4] = Die5.getNumberOnTop();

                            Console.WriteLine("{0} {1} {2} {3} {4}", Die1.getNumberOnTop(), Die2.getNumberOnTop(), Die3.getNumberOnTop(), Die4.getNumberOnTop(), Die5.getNumberOnTop());
                            // output the new rolls for each die
                        }
                        /*** Scoring ***/
                        if (currentPlayer == 1) //checks to see if the current player is player 1
                        {
                            player1History.Add(currentRoll.ToArray()); //Adds the array of rolled values for this turn to list used to hold game history for player 1 //The array is converted another array so that it is not affected by altering the currentRoll array on each turn
                            player1.Score += checkDice(currentRoll, false); //Adds the appropriate amount of score to player 1 //an argument of false = regular points
                            Console.WriteLine("Player 1's score: {0}", player1.Score); //outputs player 1's score
                        }
                        if (currentPlayer == 2) //checks to see if the current player is player 2
                        {
                            player2History.Add(currentRoll.ToArray()); //Adds the array of rolled values for this turn to list used to hold game history for player 2 //The array is converted another array so that it is not affected by altering the currentRoll array on each turn
                            player2.Score += checkDice(currentRoll, false); //Adds the appropriate amount of score to player 2 //an argument of false = regular points
                            Console.WriteLine("Player 2's score: {0}", player2.Score); //outputs player 2's score
                        }
                        break; //end of case 1
                    default: //press any other key = roll all dice once for double points
                        if (currentPlayer == 1) //checks to see if the current player is player 1
                        {
                            player1History.Add(currentRoll.ToArray()); //Adds the array of rolled values for this turn to list used to hold game history for player 1 //The array is converted another array so that it is not affected by altering the currentRoll array on each turn
                            player1.Score += checkDice(currentRoll, true); //Adds the appropriate amount of score to player 1 //an argument of true = double points
                            Console.WriteLine("Double points added if applicable."); //alert user of possible double points
                            Console.WriteLine("Player 1's score: {0}", player1.Score); //outputs player 1's score
                        }
                        if (currentPlayer == 2)
                        {
                            player2History.Add(currentRoll.ToArray()); ///Adds the array of rolled values for this turn to list used to hold game history for player 2 //The array is converted another array so that it is not affected by altering the currentRoll array on each turn
                            player2.Score += checkDice(currentRoll, true); //Adds the appropriate amount of score to player 2 //an argument of true = double points
                            Console.WriteLine("Double points added if applicable."); //alert user of possible double points
                            Console.WriteLine("Player 2's score: {0}", player2.Score); //outputs player 2's score
                        }
                        break; //end of default case
                }//end of switch statement

                player++; //increment player count change current player for next turn

            }//end of while loop

            if (player1.Score >= scoreLimit) //checks to see if player 1 has hit the score limit
                Console.WriteLine("Player 1 Wins!"); //alert players
            if (player2.Score >= scoreLimit) //checks to see if player 2 has hit the score limit
                Console.WriteLine("Player 2 Wins!"); //alert players

            Console.WriteLine("\nDisplaying History:"); //alerts users that the history of the game is going to be shown
            Console.WriteLine("\nPlayer 1:"); //history for player 1
            foreach(int[] diceRolls in player1History) //loop that iterates through each integer array with the player1History list
            {
                Console.Write("Turn {0}: ", ++player1Turns); //Shows which turn the displayed rolls were thrown on //pre-increments on each iteration to avoid over-incrementation
                foreach(int roll in diceRolls) //loop that iterates through each integer within the diceRolls array 
                {
                    Console.Write(roll + " "); //outputs the roll and a space afterwards for better formatting
                    total += roll; //adds the roll to the total
                }
                Console.WriteLine("Total: {0}", total); //outputs the total for this turn
                total = 0; //reset total back to zero for next turn
                player1Average += diceRolls.Average(); //calculates average for each roll and adds together
            }
            player1Average /= (player1Turns); //divides sum of averages of each roll by number of turns to produce overall average

            Console.WriteLine("\nPlayer 2:"); //history for player 1
            foreach (int[] diceRolls in player2History) //loop that iterates through each integer array with the player2History list
            {
                Console.Write("Turn {0}: ", ++player2Turns); //Shows which turn the displayed rolls were thrown on //pre-increments on each iteration to avoid over-incrementation
                foreach (int roll in diceRolls) //loop that iterates through each integer within the diceRolls array 
                {
                    Console.Write(roll + " "); //outputs the roll and a space afterwards for better formatting
                    total += roll; //adds the roll to the total
                }
                Console.WriteLine("Total: {0}", total); //outputs the total for this turn
                total = 0; //reset total back to zero for next roll
                player2Average += diceRolls.Average(); //calculates average for each roll and adds together
            }
            player2Average /= (player2Turns); //divides sum of averages of each roll by number of turns to produce overall average

            /*** Statistics ***/
            Console.WriteLine("\nTotal number of turns: {0}", player1Turns + player2Turns); //outputs the total number of turns during the game

            Console.WriteLine("Average of all throws for player 1: {0}", (int)Math.Round(player1Average)); //output average dice roll for player 1 //Math.Round() used to provide accurate conversion from double to int as opposed to Convert.ToInt32()
                                                                            //parsed as int
            Console.WriteLine("Average of all throws for player 2: {0}", (int)Math.Round(player2Average)); //output average dice roll for player 1 //Math.Round() used to provide accurate conversion from double to int as opposed to Convert.ToInt32()
                                                                            //parsed as int
            exit(); //allows user to see roll history before game closes
        }
        public static void checkCurrentPlayer()
        {
            if (player % 2 == 0)//Checks to see if the player count is even and therefors can be divided by 2 with no remainder
            {
                Console.WriteLine("Player 2's Turn"); //outputs that it is player 2's turn
                currentPlayer = 2; //sets the current player to 2
            }
            else //Otherwise the player count must be odd and therefore has a remainder when divided by 2
            {
                Console.WriteLine("Player 1's Turn"); //outputs that it is player 1's turn
                currentPlayer = 1; //sets the current player to 1
            }
        }

        public static int checkDice(int[] currentRoll, bool doublePoints) //takes array of dice rolls and the flag for setting double points
        {
            int countOnes = currentRoll.Where(num => num == 1).Count();  //uses LINQ to count the occurences of 1's within the array of dice rolls and assigns them to the variable
            int countTwos = currentRoll.Where(num => num == 2).Count();  //occurences of 2's counted and assigned to seperate variable
            int countThrees = currentRoll.Where(num => num == 3).Count();//occurences of 3's counted and assigned to seperate variable
            int countFours = currentRoll.Where(num => num == 4).Count(); //occurences of 4's counted and assigned to seperate variable
            int countFives = currentRoll.Where(num => num == 5).Count(); //occurences of 5's counted and assigned to seperate variable
            int countSixes = currentRoll.Where(num => num == 6).Count(); //occurences of 6's counted and assigned to seperate variable

            if (countOnes == 3 || countTwos == 3 || countThrees == 3 || countFours == 3 || countFives == 3 || countSixes == 3) //checks to see if any roll occured three times
            {
                if (doublePoints == true) //checks to see if the current roll is worth double points
                    return 6;
                else //otherwise it is a normal throw worth no additional points
                    return 3;
            }
            else if (countOnes == 4 || countTwos == 4 || countThrees == 4 || countFours == 4 || countFives == 4 || countSixes == 4) //checks to see if any roll occured four times
            {
                if (doublePoints == true) //checks to see if the current roll is worth double points
                    return 12;
                else //otherwise it is a normal throw worth no additional points
                    return 6;
            }
            else if (countOnes == 5 || countTwos == 5 || countThrees == 5 || countFours == 5 || countFives == 5 || countSixes == 5) //checks to see if any roll occured five times
            {
                if (doublePoints == true) //checks to see if the current roll is worth double points
                    return 24;
                else //otherwise it is a normal throw worth no additional points
                    return 12;
            }
            else //otherwise, a roll only occured two times or less
                return 0;
        }

        public static int checkReroll(int[] currentRoll) //takes integer array of dice rolls
        {
            int countOnes = currentRoll.Where(num => num == 1).Count(); //uses LINQ to count the occurences of 1's within the array of dice rolls and assigns them to the variable
            int countTwos = currentRoll.Where(num => num == 2).Count(); //occurences of 2's counted and assigned to seperate variable
            int countThrees = currentRoll.Where(num => num == 3).Count(); //occurences of 3's counted and assigned to seperate variable
            int countFours = currentRoll.Where(num => num == 4).Count(); //occurences of 4's counted and assigned to seperate variable
            int countFives = currentRoll.Where(num => num == 5).Count(); //occurences of 5's counted and assigned to seperate variable
            int countSixes = currentRoll.Where(num => num == 6).Count(); //occurences of 6's counted and assigned to seperate variable

            if (countOnes == 2 && countTwos <= 2 && countThrees <= 2 && countFours <= 2 && countFives <= 2 && countSixes <= 2) //checks to see if there are 2 occurnces of a dice roll of 1 //ensures only executed when there are only two of a kinds rolled
            {
                return 1;
            }
            if (countTwos == 2 && countOnes <= 2 && countThrees <= 2 && countFours <= 2 && countFives <= 2 && countSixes <= 2) //checks to see if there are 2 occurnces of a dice roll of 2 //ensures only executed when there are only two of a kinds rolled
            {
                return 2;
            }
            if (countThrees == 2 && countOnes <= 2 && countTwos <= 2 && countFours <= 2 && countFives <= 2 && countSixes <= 2) //checks to see if there are 2 occurnces of a dice roll of 3 //ensures only executed when there are only two of a kinds rolled
            {
                return 3;
            }
            if (countFours == 2 && countOnes <= 2 && countTwos <= 2 && countThrees <= 2 && countFives <= 2 && countSixes <= 2) //checks to see if there are 2 occurnces of a dice roll of 4 //ensures only executed when there are only two of a kinds rolled
            {
                return 4;
            }
            if (countFives == 2 && countOnes <= 2 && countTwos <= 2 && countThrees <= 2 && countFours <= 2 && countSixes <= 2) //checks to see if there are 2 occurnces of a dice roll of 5 //ensures only executed when there are only two of a kinds rolled
            {
                return 5;
            }
            if (countSixes == 2 && countOnes <= 2 && countTwos <= 2 && countThrees <= 2 && countFours <= 2 && countFives <= 2) //checks to see if there are 2 occurnces of a dice roll of 6 //ensures only executed when there are only two of a kinds rolled
            {
                return 6;
            }
            else //otherwise, there are no two of a kinds that have been rolled or there is also a three of a kind present
                return -1;
        }

        public static byte checkSelection(ConsoleKeyInfo userSelection) //takes the information of a key
        {
            if (userSelection.Key == ConsoleKey.Spacebar) //checks to see if the spacebar has been pressed
            {
                return 1;
            }
            else //otherwise, any other key has been pressed 
            {
                return 0;
            }
        }

        public static void exit() //no arguments //used to hold prgram before exit
        {
            Console.WriteLine("\nPress any key to exit program..."); //alerts user that program will be closed
            Console.ReadKey(); //holds program until user presses any key
        }
    } //end of Game class

    class Die //Die class
    {
        //declarations
        int numberOnTop; //holds number on top of die
        Random randomNumber; //holds random number

        //code
        public Die(Random number) //constuctor accepting a number from the random class
        {
            randomNumber = number; //sets the randomNumber variable as the argument value
        }

        public void roll() //used for rolling die
        {
            numberOnTop = randomNumber.Next(1, 7); //generate a random number between 1 and 6 and assign it to the numberOnTop variable
        }

        public int getNumberOnTop() //used for returning the number displayed by the die
        {
            return numberOnTop; //returns the number on top variable
        }

        public void setNumberOnTop(int number) //accepts an integer as argument //used for seeting the displayed number on the die
        {
            numberOnTop = number; //sets the numberOnTop variable as the argument value
        }
    }//end of Die class

    class Player //Player class
    {
        int score = 0; //integer used for holding the player's score
        public int Score //public accessor for the player's score
        {
            get //returns the value of score variable
            {
                return score;
            }
            set //sets the value of the score variable
            {
                score = value;
            }
        } 
    }//end of Player class
}//end of namespace
