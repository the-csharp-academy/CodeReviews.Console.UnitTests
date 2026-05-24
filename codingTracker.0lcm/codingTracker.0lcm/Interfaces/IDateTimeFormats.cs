namespace codingTracker._0lcm.Interfaces;

public interface IDateTimeFormats
{
    public string[] HourFormats { get; }
    public string[] DateFormats { get; }
    public string DateIso { get; }
    public string FullDateWithTimeFormat { get; }
}