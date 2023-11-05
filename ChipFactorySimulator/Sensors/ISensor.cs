namespace ChipFactorySimulator.Sensors;

public interface ISensor
{
    public abstract void PublishData();
    public abstract string GetInfo();

    double GenerateDataPoint(bool random=true)
    {
        return random ? GenerateRandomDataPoint() : GenerateSetDataPoint();
    }

    double GenerateRandomDataPoint()
    {
        var random = new Random();
        return random.NextDouble() * (GeneratedValueRange.from - GeneratedValueRange.to) + GeneratedValueRange.to;
    }

    double GenerateSetDataPoint()
    {
        return GeneratedSetValue;
    }
    
    void StartGenerating()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"{Name} - {GenerateDataPoint()} - {DateTime.Now}");
            Thread.Sleep((int)(60000/Interval));
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{Name} finished");
        Console.ForegroundColor = ConsoleColor.White;


    }

    (double from, double to) GeneratedValueRange { get; set; }
    double Interval { get; set; }

    double GeneratedSetValue { get; set; }

    Guid Id { get; set; }

    string Name { get; set; }
    string UnitOfMeasurement
    {
        get;
        set;
    }
}