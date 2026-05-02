
using DatabaseTest;
using LinqToDB;

var options = new DataOptions().UseSQLite("Data Source=gw2.db");

var ctx = new DbTest(new DataOptions<DbTest>(options));

try
{
    ctx.CreateTable<Charakter>();
}
catch
{
    Console.WriteLine("Table exists already!");
}

// --- new datarow --- \\

Charakter charakter = new Charakter()
{
    name = "Allmightychaos",
    klasse = "Waldläufer",
    level = 80
};

ctx.Insert(charakter);

foreach (var item in ctx.GetTable<Charakter>())
{
    Console.WriteLine($"Name: {item.name} | Klasse: {item.klasse} | Level: {item.level}");
}

