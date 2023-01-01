using System;

namespace ElmaSmartFarm.SharedLibrary.Models;

public class PeriodModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int FarmId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? DayOfPeriod => EndDate == null ? (DateTime.Now - StartDate).Days + 1 : null;
    public int? Duration => EndDate == null ? null : (EndDate - StartDate).Value.Days;
    public ChickenStatisticsModel ChickenStatistics { get; set; } = new();
    public FoodStatisticsModel FoodStatistics { get; set; } = new();
    public int ChickenPrimaryCount { get => ChickenStatistics.ChickenPrimaryCount; set => ChickenStatistics.ChickenPrimaryCount = value; }
    public int UserId { get; set; }
    public string Descriptions { get; set; }
}