﻿using System;
using Iron_Heart;

    namespace Iron_Heart
    {
        public class Program1
        {

            public static (int x, int y) Move(char[,] map, string? direction, int player_x, int player_y){
                // Check for the direction the player wants to move
                if (direction == "up")
                {
                    // Verify that the next space is within the bounds of the map
                    if (player_y <= 4 && player_y >= 1)
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
                    if (player_y >= 0 && player_y <= 3)
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
                    if (player_x <= 4 && player_x >= 1)
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
                    if (player_x >= 0 && player_x <= 3)
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
                else
                {
                    Console.WriteLine("Invalid direction!");
                }

                return (player_x, player_y);
            }

        }
    }