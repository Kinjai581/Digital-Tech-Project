using System;
using System.Collections.Generic;
namespace Program
{
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

        public Weapon GetWeapon(string weaponName)
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

    public class Weapon
    {
        public string Name { get; set; }
        public int AttackPower { get; set; }
        public int Durability { get; set; }

        public Weapon(string name, int attackPower, int durability)
        {
            Name = name;
            AttackPower = attackPower;
            Durability = durability;
        }

        public void Use()
        {
            Durability--;
        }

        public bool IsBroken()
        {
            return Durability <= 0;
        }
    } // Use class inheritance if we want to add weapons

    public class Enemy
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int AttackPower { get; set; }
        public int Defense { get; set; }

        public Enemy(string name, int health, int attackPower, int defense)
        {
            Name = name;
            Health = health;
            AttackPower = attackPower;
            Defense = defense;
        }

        public void Attack(Player player)
        {
            int damage = AttackPower - player.Defense;
            if (damage > 0)
            {
                player.Health -= damage;
            }
        }

        public bool IsAlive()
        {
            return Health > 0;
        }
    } // Use class inheritance to make more enemies

    public class Player
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int AttackPower { get; set; }
        public int Defense { get; set; }
        public Weapon EquippedWeapon { get; set; }
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
            int damage;
            if (EquippedWeapon != null)
            {
                damage = EquippedWeapon.AttackPower - enemy.Defense;
            }
            else
            {
                damage = AttackPower - enemy.Defense;
            }

            if (damage > 0)
            {
                enemy.Health -= damage;
                if (EquippedWeapon != null)
                {
                    EquippedWeapon.Use();
                }
            }
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public void EquipWeapon(string weaponName)
        {
            Weapon weapon = Inventory.GetWeapon(weaponName);
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
        
        public Locked_Room(int roomtype, int ycoord, int xcoord, bool cleared, bool unlocked, Item requireditem)
        {
            RoomType = roomtype;
            YCoord = ycoord;
            XCoord = xcoord;
            Cleared = cleared;
            Unlocked = unlocked;
            RequiredItem = requireditem;
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
        
        public Combat_Room(int roomtype, int ycoord, int xcoord, bool cleared, Enemy dummyobject, Item loot)
        {
            RoomType = roomtype;
            YCoord = ycoord;
            XCoord = xcoord;
            Cleared = cleared;
            Dummyobject = dummyobject;
            Loot = loot;
        }
        
        public override void Entering(Player x)
        {
            Console.WriteLine("Kill monster?");
            string Action = Console.ReadLine();
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

    public class Testing_Room : Basic_Room
    {
        public Weapon Loot;
        public Item Otherloot;
        
        public Testing_Room(int roomtype, int ycoord, int xcoord, bool cleared, Weapon loot, Item otherloot)
        {
            RoomType = roomtype;
            YCoord = ycoord;
            XCoord = xcoord;
            Cleared = cleared;
            Loot = loot;
            Otherloot = otherloot;
        }
        
        public override void Entering(Player x)
        {
            if (this.Cleared == false)
            {
                Console.WriteLine("Hello I am a room! How are you?");
                string Answer = Console.ReadLine();
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


    class Program
    {
        static void Main(string[] args)
        {
            Player player = new Player("Beng", 100, 10, 5, null);
            Enemy Shadow = new Enemy("The eminence himself", 1000, 1000, 1000);
            Item keyobj = new Item("Not sus key...", "Key forged by fung for fung things.");
            Weapon sword = new Weapon("Sword", 15, 10);
            Weapon Shadowslime = new Weapon("Shadow slime sword", 1000, 1000);
            player.Inventory.AddWeapon(sword);
            player.EquipWeapon("Sword");
            Combat_Room blung1 = new Combat_Room(1, 0, 0, false, Shadow, keyobj);
            Testing_Room blung2 = new Testing_Room(1, 0, 0, false, Shadowslime, keyobj);
            Locked_Room blung3 = new Locked_Room(1, 0, 0, false, false, keyobj);
            Enemy goblin = new Enemy("Goblin", 30, 5, 2);
            
            blung1.Entering(player);
            int RoomNumber = 2;
            
            switch (RoomNumber)
            {
                case 0:
                    blung1.Entering(player);
                    break;
                case 1:
                    blung2.Entering(player);
                    break;
                case 2:
                    blung3.Entering(player);
                    break;
            }
            
            player.Attack(goblin);
            player.Inventory.DisplayInventory();

            Console.WriteLine($"{goblin.Name} Health: {goblin.Health}");
        }
    }
}