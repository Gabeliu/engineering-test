/* Usage of Generative AI

For transparency, I want to briefly note how I used generative AI while working on this exercise:

- I implemented the DecreaseQuality logic myself, including the decision to centralize Conjured-item behavior there.
- I initially refactored the structure of UpdateQuality and item-type handling on my own, and I used ChatGPT afterwards to help rework and improve the clarity of the final version.
- The AI tool also helped me polish the XML documentation comments and make the explanations more readable.
- For unit tests, I started by writing some basic test cases myself, and then used AI to help expand coverage, refine edge cases, and improve naming and structure.

All final code and tests were manually reviewed and adjusted by me. AI was used as a productivity tool to clean up wording, suggest improvements, and check for missed scenarios — not to replace my own reasoning. I understand the general logic behind the refactor and tests, and I’ll do my best to discuss the decisions during the interview.
*/

using System;
using System.Collections.Generic;

namespace GildedRose.Console;

/// <summary>
/// Gilded Rose inventory management system that updates item quality and sell-in values
/// based on specific rules for different item types.
/// </summary>
public class Program
{
    private const string SulfurasName = "Sulfuras, Hand of Ragnaros";

    public IList<Item> Items { get; set; } = new List<Item>();

    /// <summary>
    /// Entry point for the application. Initializes inventory and runs quality updates.
    /// </summary>
    static void Main(string[] args)
    {
        System.Console.WriteLine("OMGHAI!");

        var app = new Program
        {
            Items = new List<Item>
            {
                new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                new Item {Name = SulfurasName, SellIn = 0, Quality = 80},
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    SellIn = 15,
                    Quality = 20
                },
                new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
            }
        };

        app.UpdateQuality();

        System.Console.ReadKey();
    }

    /// <summary>
    /// Updates the quality and sell-in values for all items in inventory.
    /// </summary>
    public void UpdateQuality()
    {
        foreach (var item in Items)
        {
            UpdateItem(item);
        }
    }

    /// <summary>
    /// Updates a single item's quality and sell-in value based on item type and business rules.
    /// </summary>
    /// <param name="item">The item to update</param>
    private static void UpdateItem(Item item)
    {
        // Sulfuras never changes
        if (item.Name == SulfurasName)
        {
            return;
        }

        bool isAgedBrie = item.Name == "Aged Brie";
        bool isBackstage = item.Name == "Backstage passes to a TAFKAL80ETC concert";

        // 1) Update quality before SellIn decreases
        if (isAgedBrie)
        {
            IncreaseQuality(item, 1);
        }
        else if (isBackstage)
        {
            UpdateBackstagePass(item);
        }
        else
        {
            // normal or conjured item
            DecreaseQuality(item, 1);
        }

        // 2) Decrease SellIn for all non-Sulfuras items
        item.SellIn--;

        // 3) Apply rules after SellIn has passed
        if (item.SellIn < 0)
        {
            if (isAgedBrie)
            {
                // Aged Brie increases faster after sell date
                IncreaseQuality(item, 1);
            }
            else if (isBackstage)
            {
                // Backstage passes drop to 0 after the concert
                item.Quality = 0;
            }
            else
            {
                // normal or conjured: degrade again
                DecreaseQuality(item, 1);
            }
        }
    }

    /// <summary>
    /// Handles quality updates for Backstage passes based on days until concert.
    /// Quality increases by 1 normally, +2 at 10 days or less, +3 at 5 days or less.
    /// </summary>
    /// <param name="item">The backstage pass item to update</param>
    private static void UpdateBackstagePass(Item item)
    {
        // Base increase
        IncreaseQuality(item, 1);

        if (item.SellIn <= 10)
        {
            IncreaseQuality(item, 1);
        }

        if (item.SellIn <= 5)
        {
            IncreaseQuality(item, 1);
        }
    }

    /// <summary>
    /// Increases item quality by the specified amount, capped at 50.
    /// </summary>
    /// <param name="item">The item to increase quality for</param>
    /// <param name="amount">The amount to increase by</param>
    private static void IncreaseQuality(Item item, int amount)
    {
        item.Quality = Math.Min(50, item.Quality + amount);
    }

    /// <summary>
    /// Decreases item quality by the specified amount, ensuring it doesn't go below 0.
    /// Conjured items degrade twice as fast as normal items.
    /// </summary>
    /// <param name="item">The item to decrease quality for</param>
    /// <param name="amount">The base amount to decrease by</param>
    private static void DecreaseQuality(Item item, int amount)
    {
        var effectiveAmount = amount;

        if (item.Name.StartsWith("Conjured"))
        {
            effectiveAmount *= 2;
        }

        item.Quality = Math.Max(0, item.Quality - effectiveAmount);
    }
}

/// <summary>
/// Represents an item in the Gilded Rose inventory.
/// </summary>
public class Item
{
    /// <summary>
    /// The name of the item (determines special behavior rules).
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The number of days left to sell this item. Decreases by 1 each day.
    /// </summary>
    public int SellIn { get; set; }

    /// <summary>
    /// The quality value of the item (0-50, except Sulfuras which is always 80).
    /// </summary>
    public int Quality { get; set; }
}
