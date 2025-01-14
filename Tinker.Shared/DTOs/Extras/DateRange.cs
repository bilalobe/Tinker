namespace Tinker.Shared.DTOs.Extras;

public record DateRange
{
    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be after start date");

        StartDate = startDate;
        EndDate = endDate;
    }

    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    public int DayCount =>
        (EndDate - StartDate).Days + 1;

    public bool Contains(DateTime date)
    {
        return date >= StartDate && date <= EndDate;
    }
}