namespace Cringebot.Model;

public class ChartDataPoint
{
    public string Name { get; set; }
    public int Count { get; set; }

    public ChartDataPoint(string name, int count)
    {
        Name = name;
        Count = count;
    }
}