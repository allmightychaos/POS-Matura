// See https://aka.ms/new-console-template for more information
using DatabaseTest;
using LinqToDB;

Console.WriteLine("Hello, World!");

DataOptions options = new DataOptions().UseSQLite("Data Source=gw2.db");
var ctx = new TestDb(new DataOptions<TestDb>(options));

// table erstellen
try
{
    ctx.CreateTable<Charakter>();
}
catch (Exception)
{
}

// ausgabe 1
List<Charakter> alle = ctx.Charakter.ToList();


Charakter charakter = new Charakter()
{
    charName = "Allmightychaos",
    klasse = "Waldläufer",
    level = 80
};

ctx.Insert(charakter);

alle = ctx.Charakter.ToList();

foreach (var c in alle)
{
    Console.WriteLine($"{c.id} | {c.charName} | {c.klasse} | {c.level}");
}