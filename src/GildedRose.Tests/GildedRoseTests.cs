using System.Collections.Generic;
using Xunit;
using GildedRose.Console;

namespace GildedRose.Tests;

public class GildedRoseTests
{
    private Program CreateAppWithSingleItem(string name, int sellIn, int quality)
    {
        return new Program
        {
            Items = new List<Item>
            {
                new Item { Name = name, SellIn = sellIn, Quality = quality }
            }
        };
    }

    [Fact]
    public void NormalItem_DegradesByOne_BeforeSellDate()
    {
        var app = CreateAppWithSingleItem("+5 Dexterity Vest", sellIn: 10, quality: 20);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(9, item.SellIn);
        Assert.Equal(19, item.Quality);
    }

    [Fact]
    public void NormalItem_DegradesByTwo_AfterSellDate()
    {
        var app = CreateAppWithSingleItem("Elixir of the Mongoose", sellIn: 0, quality: 10);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(8, item.Quality); // 10 - 2
    }

    [Fact]
    public void Quality_IsNeverNegative_EvenForConjured()
    {
        var app = CreateAppWithSingleItem("Conjured Mana Cake", sellIn: 0, quality: 1);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(0, item.Quality);
    }

    [Fact]
    public void AgedBrie_IncreasesInQuality_BeforeSellDate()
    {
        var app = CreateAppWithSingleItem("Aged Brie", sellIn: 5, quality: 10);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(4, item.SellIn);
        Assert.Equal(11, item.Quality); // +1
    }

    [Fact]
    public void AgedBrie_IncreasesFaster_AfterSellDate()
    {
        var app = CreateAppWithSingleItem("Aged Brie", sellIn: 0, quality: 10);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(12, item.Quality); // +1 before, +1 after = +2
    }

    [Fact]
    public void BackstagePass_IncreasesBy1_WhenMoreThan10Days()
    {
        var app = CreateAppWithSingleItem("Backstage passes to a TAFKAL80ETC concert", sellIn: 11, quality: 10);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(10, item.SellIn);
        Assert.Equal(11, item.Quality);
    }

    [Fact]
    public void BackstagePass_IncreasesBy2_When10DaysOrLess()
    {
        var app = CreateAppWithSingleItem("Backstage passes to a TAFKAL80ETC concert", sellIn: 10, quality: 10);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(9, item.SellIn);
        Assert.Equal(12, item.Quality); // +2
    }

    [Fact]
    public void BackstagePass_IncreasesBy3_When5DaysOrLess()
    {
        var app = CreateAppWithSingleItem("Backstage passes to a TAFKAL80ETC concert", sellIn: 5, quality: 10);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(4, item.SellIn);
        Assert.Equal(13, item.Quality); // +3
    }

    [Fact]
    public void BackstagePass_DropsToZero_AfterConcert()
    {
        var app = CreateAppWithSingleItem("Backstage passes to a TAFKAL80ETC concert", sellIn: 0, quality: 10);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(0, item.Quality);
    }

    [Fact]
    public void Sulfuras_NeverChanges()
    {
        var app = CreateAppWithSingleItem("Sulfuras, Hand of Ragnaros", sellIn: 0, quality: 80);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(0, item.SellIn);
        Assert.Equal(80, item.Quality);
    }

    [Fact]
    public void ConjuredItem_DegradesTwiceAsFast_BeforeSellDate()
    {
        var app = CreateAppWithSingleItem("Conjured Mana Cake", sellIn: 3, quality: 10);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(2, item.SellIn);
        Assert.Equal(8, item.Quality); // -2
    }

    [Fact]
    public void ConjuredItem_DegradesFourPerDay_AfterSellDate()
    {
        var app = CreateAppWithSingleItem("Conjured Mana Cake", sellIn: 0, quality: 10);

        app.UpdateQuality();

        var item = app.Items[0];
        Assert.Equal(-1, item.SellIn);
        Assert.Equal(6, item.Quality); // -4 total
    }
}
