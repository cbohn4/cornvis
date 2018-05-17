using System;
using System.IO;

public class Class1
{
	public Class1()
	{
        int numSec = 0;
        var lines = File.ReadAllLines("TestData.csv");
        foreach(var line in lines) {
            numSec = line.Split(':').Length - 1;
            Console.WriteLine("Line: " + line + " has " + +numSec.ToString() + " colons.");


        }


	}
}
