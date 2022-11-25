namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class PeriodErrorModel
    {
        public List<FarmInPeriodErrorModel> FarmErrors { get; set; }
        public List<PoultryInPeriodErrorModel> PoultryErrors { get; set; }
        public bool HasFarmError => FarmErrors != null && FarmErrors.Any();
        public bool HasPoultryError => PoultryErrors != null && PoultryErrors.Any();
        public bool HasError => HasFarmError || HasPoultryError;
    }
}