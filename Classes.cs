using System;
using System.Collections.Generic;

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
            damage = EquippedWeapon.AttackPower - enemy.Defence;
        }
        else
        {
            damage = AttackPower - enemy.Defence;
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

// Example usage
class Program
{
    static void Main(string[] args)
    {
        Player player = new Player("Beng", 100, 10, 5, null);

        Weapon sword = new Weapon("Sword", 15, 10);
        player.Inventory.AddWeapon(sword);
        player.EquipWeapon("Sword");

        Enemy goblin = new Enemy("Goblin", 30, 5, 2);

        player.Attack(goblin);
        player.Inventory.DisplayInventory();

        Console.WriteLine($"{goblin.Name} Health: {goblin.Health}");
    }
}


