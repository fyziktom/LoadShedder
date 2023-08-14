// See https://aka.ms/new-console-template for more information
using LoadShedderSimulator;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

Console.WriteLine("Hello, World!");

// create simulator instance
var simulator = new Simulator();

// place first GamePiece on position on the board
simulator.Board[0] = 454;
await simulator.SendData();

await Task.Delay(5000);

// place second GamePiece on the position on the board
simulator.Board[1] = 3938;
await simulator.SendData();

await Task.Delay(5000);

