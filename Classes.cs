using System;
using System.Timers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Iron_Heart;

namespace Iron_Heart{
    public class Inventory
    {
        public List<Item> Items { get; set; }
        public List<Weapon> Weapons { get; set; }

        public Inventory()
        {
            Items = new List<Item>();
            Weapons = new List<Weapon>();
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public void AddWeapon(Weapon weapon)
        {
            Weapons.Add(weapon);
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }

        public void RemoveWeapon(Weapon weapon)
        {
            Weapons.Remove(weapon);
        }

        public void DisplayInventory()
        {
            Console.WriteLine("Items:");
            foreach (var item in Items)
            {
                Console.WriteLine($"- {item.Name}: {item.Description}");
            }

            Console.WriteLine("Weapons:");
            foreach (var weapon in Weapons)
            {
                Console.WriteLine($"- {weapon.Name}: Attack Power {weapon.AttackPower}, Durability {weapon.Durability}");
            }
        }

        public Weapon? GetWeapon(string weaponName)
        {
            foreach (var weapon in Weapons)
            {
                if (weapon.Name == weaponName)
                {
                    return weapon;
                }
            }
            return null;
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Item(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public abstract class Weapon
    {
        public string? Name;
        public int AttackPower;
        public int Durability;

        public virtual void Hit(Enemy x){
            x.Health -= this.AttackPower;
            Console.WriteLine("You did damage hopefully!");
            this.Use();
        }

        public virtual void Use()
        {
            Durability--;
        }

        public virtual bool IsBroken()
        {
            return Durability <= 0;
        }
    }

    public class Sword : Weapon
    {
        public Sword(string name, int attackPower, int durability)
        {
            Name = name;
            AttackPower = attackPower;
            Durability = durability;
        }
        
        public override void Hit(Enemy x){
            x.Health -= this.AttackPower;
            Console.WriteLine("You did damage hopefully!");
        }
    }

    public abstract class Enemy
    {
        public string? Name;
        public int Health;
        public int AttackPower;
        public int Defense;

        public virtual void Attack(Player x)
        {
            if (x.Defense >= this.AttackPower)
            {
                Console.WriteLine("Nuh uh no damage dealt to player.");
            }
            else
            {
                x.Health -= (this.AttackPower - x.Defense);
            }
        }

        public virtual bool IsAlive()
        {
            return Health > 0;
        }
    } // Use class inheritance to make more enemies

    public class Goblin : Enemy
    {
        public Goblin(string name, int health, int attackPower, int defense)
        {
            Name = name;
            Health = health;
            AttackPower = attackPower;
            Defense = defense;
        }

        public override void Attack(Player x)
        {
            if (x.Defense >= this.AttackPower)
            {
                Console.WriteLine("Nuh uh no damage dealt to player.");
            }
            else
            {
                x.Health -= (this.AttackPower - x.Defense);
            }
        }
    }

    public class Eminence : Enemy
    {
        public Eminence(string name, int health, int attackPower, int defense)
        {
            Name = name;
            Health = health;
            AttackPower = attackPower;
            Defense = defense;
        }

        public override void Attack(Player x)
        {
            if (x.Defense >= this.AttackPower)
            {
                Console.WriteLine("Nuh uh no damage dealt to player.");
            }
            else
            {
                x.Health -= (this.AttackPower - x.Defense);
            }
        }
    }

    public class Player
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int AttackPower { get; set; }
        public int Defense { get; set; }
        public Weapon? EquippedWeapon { get; set; }
        public Inventory Inventory { get; set; }

        public Player(string name, int health, int attackPower, int defense, Weapon equippedWeapon)
        {
            Name = name;
            Health = health;
            AttackPower = attackPower;
            Defense = defense;
            EquippedWeapon = equippedWeapon;
            Inventory = new Inventory();
        }

        public void Attack(Enemy enemy)
        {
            if (EquippedWeapon != null)
            {
                this.EquippedWeapon.Hit(enemy);
            }
            else
            {
                if (this.AttackPower > enemy.Defense)
                {
                    enemy.Health -= (this.AttackPower - enemy.Defense);
                }
            }
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public void EquipWeapon(string weaponName)
        {
            Weapon? weapon = Inventory.GetWeapon(weaponName);
            if (weapon != null)
            {
                EquippedWeapon = weapon;
            }
        }
    }

    public class Chest
    {
        public bool IsLocked { get; set; }
        public List<Item> Contents { get; set; }

        public Chest(bool isLocked, List<Item> contents)
        {
            IsLocked = isLocked;
            Contents = contents;
        }

        public void Unlock()
        {
            IsLocked = false;
        }

        public List<Item> Open()
        {
            if (!IsLocked)
            {
                return Contents;
            }
            else
            {
                return new List<Item>();
            }
        }
    }

    public class Game
    {
        public Player Player { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<Chest> Chests { get; set; }

        public Game(Player player, List<Enemy> enemies, List<Chest> chests)
        {
            Player = player;
            Enemies = enemies;
            Chests = chests;
        }

        public void Start()
        {
            // Game loop logic goes here
        }

        public void DisplayStats()
        {
            Console.WriteLine($"{Player.Name} - Health: {Player.Health}, Attack: {Player.AttackPower}, Defense: {Player.Defense}");
            foreach (var enemy in Enemies)
            {
                Console.WriteLine($"{enemy.Name} - Health: {enemy.Health}, Attack: {enemy.AttackPower}, Defense: {enemy.Defense}");
            }
        }
    }

    public abstract class Basic_Room
    {
        public int RoomType;
        public int YCoord;
        public int XCoord;
        public bool Cleared;
        public string? Image;
        
        public virtual void Entering(Player x)
        {
            Console.WriteLine("Fung");
        }
        
        public virtual void WhereAmI()
        {
            Console.WriteLine(this.YCoord);
            Console.WriteLine(this.XCoord);
        }
    }

    public class Locked_Room : Basic_Room
    {
        public bool Unlocked;
        public Item RequiredItem;
        
        public Locked_Room(int roomtype, int ycoord, int xcoord, bool cleared, bool unlocked, Item requireditem, string image)
        {
            RoomType = roomtype;
            YCoord = ycoord;
            XCoord = xcoord;
            Cleared = cleared;
            Unlocked = unlocked;
            RequiredItem = requireditem;
            Image = image;
        }
        
        public override void Entering(Player x)
        {
            foreach (Item i in x.Inventory.Items)
            {
                if (i == this.RequiredItem)
                {
                    Console.WriteLine("Room unlocked!");
                    this.Unlocked = true;
                    break;
                }
                else
                {
                    Console.WriteLine("Nuh");
                }
            }
        }
    }

    public class Combat_Room : Basic_Room
    {
        public Enemy Dummyobject;
        public Item Loot;
        
        public Combat_Room(int roomtype, int ycoord, int xcoord, bool cleared, Enemy dummyobject, Item loot, string image)
        {
            RoomType = roomtype;
            YCoord = ycoord;
            XCoord = xcoord;
            Cleared = cleared;
            Dummyobject = dummyobject;
            Loot = loot;
            Image = image;
        }
        
        public override void Entering(Player x)
        {
            Console.WriteLine("Kill monster?");
            string? Action = Console.ReadLine();
            if (Action == "yes")
            {
                this.Dummyobject.Health = 0;
            }
            if (this.Dummyobject.Health <= 0)
            {
                Console.WriteLine("Congrats you did something!");
                x.Inventory.AddItem(this.Loot);
            }
        }
    }

    public class Boss_Room : Basic_Room
    {
        public Enemy Boss;

        public Boss_Room(int roomtype, int ycoord, int xcoord, bool cleared, Enemy boss)
        {
            RoomType = roomtype;
            YCoord = ycoord;
            XCoord = xcoord;
            Cleared = cleared;
            Boss = boss;
        }

        public override void Entering(Player x)
        {
            Console.WriteLine("Attack boss?");
            string? Action = Console.ReadLine();
            if (Action == "")
            {
                while (Action != "")
                {
                    Console.WriteLine("No input detected, try again.");
                    Action = Console.ReadLine();
                }
            }

            if (Action == "yes")
            {
                Boss.Health = 0;
            }
            if (Boss.Health <= 0)
            {
                Console.WriteLine("Congratulations! You beat the game!");
            }
        }
    }

    public class Testing_Room : Basic_Room
    {
        public Weapon Loot;
        public Item Otherloot;
        
        public Testing_Room(int roomtype, int ycoord, int xcoord, bool cleared, Weapon loot, Item otherloot, string image)
        {
            RoomType = roomtype;
            YCoord = ycoord;
            XCoord = xcoord;
            Cleared = cleared;
            Loot = loot;
            Otherloot = otherloot;
            Image = image;
        }
        
        public override void Entering(Player x)
        {
            if (this.Cleared == false)
            {
                Console.WriteLine("Hello I am a room! How are you?");
                string? Answer = Console.ReadLine();
                while (Answer == "")
                {
                    Console.WriteLine("No input detected, try again.");
                    Answer = Console.ReadLine();
                }
                if (Answer == "Fung")
                {
                    Console.WriteLine("Yahoo!");
                    x.Inventory.AddWeapon(this.Loot);
                    x.Inventory.AddItem(this.Otherloot);
                    this.Cleared = true;
                }
                Console.WriteLine("Yippee!");
            }
            else
            {
                Console.WriteLine("You already cleared this room silly!");
            }
        }
    }

    public class Program
    {

        private static System.Timers.Timer testTimer;

        private static void SetTimer()
        {
            testTimer = new System.Timers.Timer(2000);
            testTimer.Elapsed += onTimedEvent;
            testTimer.AutoReset = true;
            testTimer.Enabled = true;
        }

        private static void onTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine($"2 seconds later...", e.SignalTime);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("2 Seconds");
            SetTimer();

            Player player = new Player("Beng", 100, 10, 5, null);
            Eminence Shadow = new Eminence("The eminence himself", 10000, 10000, 10000);
            Item keyobj = new Item("Not sus key...", "Key forged by fung for fung things.");
            Sword sword = new Sword("Sword", 15, 10);
            Goblin goblin2 = new Goblin("Goblin", 30, 5, 10);
            Goblin goblin3 = new Goblin("Goblin", 30, 5, 10);
            Goblin goblin4 = new Goblin("Goblin", 30, 5, 10);
            Goblin goblin5 = new Goblin("Goblin", 30, 5, 10);

            Sword Shadowslime = new Sword("Shadow slime sword", 9999, 9999);
            player.Inventory.AddWeapon(sword);
            player.EquipWeapon("Sword");


       
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

            Combat_Room combat1 = new Combat_Room(1, 0, 0, false, Shadow, keyobj, "");
            Combat_Room combat2 = new Combat_Room(1, 0, 0, false, goblin2, keyobj, "");
            Combat_Room combat3 = new Combat_Room(1, 0, 0, false, goblin3, keyobj, "");

            Boss_Room miniboss1 = new Boss_Room(1, 0, 0, false, goblin2);
            Boss_Room boss1 = new Boss_Room(1, 1, 5, false, goblin2);


            Testing_Room blung2 = new Testing_Room(1, 0, 0, false, Shadowslime, keyobj, "");
            Locked_Room blung3 = new Locked_Room(1, 0, 0, false, false, keyobj, "");
            Goblin goblin1 = new Goblin("Goblin", 30, 5, 10);
            
            char[,] map1 = {
                    
                            {' ', ' ', 'W', '#', ' '},

                            {' ', ' ', ' ', '#', ' '},

                            {' ', ' ', ' ', '#', ' '},

                            {' ', ' ', '#', '#', ' '},

                            {' ', ' ', 'X', ' ', ' '},
                            
                            };
            

                            

                    Random rnd = new Random();


                    char[,] currentMap = map1;
                    int player_x = 4;
                    int player_y = 9;
                    Console.WriteLine("Move in a direction: ");
                    string? direction = Console.ReadLine();
                    while (direction != "") 
                    {
                        (int, int) coords_tuple = (player_x, player_y);
                        if (coords_tuple != Program1.Move(currentMap, direction, player_x, player_y))
                        {
                            
                            int RoomNumber = rnd.Next(0, 3);
                
                            switch (RoomNumber)
                            {
                                case 0:
                                    combat1.Entering(player);
                                    while (combat1.Dummyobject.Health > 0)
                                    {
                                        player.Attack(combat1.Dummyobject);
                                        Console.WriteLine($"{combat1.Dummyobject.Name} Health: {combat1.Dummyobject.Health}");
                                    }
                                    break;

                                case 1:
                                    combat2.Entering(player);
                                    while (combat2.Dummyobject.Health > 0)
                                    {
                                        player.Attack(combat2.Dummyobject);
                                        Console.WriteLine($"{combat2.Dummyobject.Name} Health: {combat2.Dummyobject.Health}");
                                    }
                                    break;

                                case 2:
                                    combat3.Entering(player);
                                    while (combat3.Dummyobject.Health > 0)
                                    {
                                        player.Attack(combat3.Dummyobject);
                                        Console.WriteLine($"{combat3.Dummyobject.Name} Health: {combat3.Dummyobject.Health}");
                                    }
                                    break;
                            }
                        }
                        coords_tuple = Program1.Move(currentMap, direction, player_x, player_y);
                        player_x = coords_tuple.Item1;
                        player_y = coords_tuple.Item2;

                        Console.WriteLine($"Your coordinates are ({player_x}, {player_y})");

                        Console.WriteLine("Move in a direction: ");
                        direction = Console.ReadLine();
                        //player.Inventory.DisplayInventory();
                    }
        }
    }
}
