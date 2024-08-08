using System;
using Iron_Heart;

    namespace Iron_Heart
    {
        public class Program1
        {

            /*
            3 Trap
            1 Chest
            1 Mini boss
            1 Boss
            2 Powerful enemy rooms
            1 Chance Room
            2 Puzzle Rooms
            1 Locked room
            3 Combat rooms
            2 Item
            1 Start
            1 End
            */



            public static (int x, int y) Move(char[,] map, string? direction, int player_x, int player_y){
                // Check for the direction the player wants to move
                if (direction == "up")
                {
                    // Verify that the next space is within the bounds of the map
                    if (player_y <= 9 && player_y >= 1)
                    {   
                        // Make sure that the next space is actually a room
                        if (map[player_y - 1, player_x] == ' ')
                        {
                            Console.WriteLine("Cannot move in that direction!");
                        }
                        else
                        {
                            // Move the player
                            player_y -= 1;
                        }
                    }
                    else
                    {
                        // If the player would move out of the array, flash an error message
                        Console.WriteLine("Cannot move in that direction!");
                    }
                }

                else if (direction == "down")
                {
                    if (player_y >= 0 && player_y <= 8)
                    {
                        if (map[player_y + 1, player_x] == ' ')
                        {
                            Console.WriteLine("Cannot move in that direction!");
                        }
                        else
                        {
                            player_y += 1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cannot move in that direction!");
                    }
                }

                else if (direction == "left")
                {
                    if (player_x <= 9 && player_x >= 1)
                    {
                        if (map[player_y, player_x - 1] == ' ')
                        {
                            Console.WriteLine("Cannot move in that direction!");
                        }
                        else
                        {
                            player_x -= 1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cannot move in that direction!");
                    }
                }
                else if (direction == "right")
                {
                    if (player_x >= 0 && player_x <= 8)
                    {
                        if (map[player_y, player_x + 1] == ' ')
                        {
                            Console.WriteLine("Cannot move in that direction!");
                        }
                        else
                        {
                            player_x += 1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cannot move in that direction!");
                    }
                }

                return (player_x, player_y);
            }

        }
    }