// See https://aka.ms/new-console-template for more information
using Wallhacks_C;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("----$$ WALLHACKS v. 0.1 $$----");
Netscan scanner = new Netscan();
scanner.ScanForCamerasCLI();

