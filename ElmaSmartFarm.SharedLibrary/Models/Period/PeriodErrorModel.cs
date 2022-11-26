namespace ElmaSmartFarm.SharedLibrary.Models
{
    public class PeriodErrorModel
    {
        public List<FarmInPeriodErrorModel> FarmErrors { get; set; }
        public List<PoultryInPeriodErrorModel> PoultryErrors { get; set; }
        public bool HasFarmError => FarmErrors != null && FarmErrors.Any(e => e.DateErased == null);
        public bool HasPoultryError => PoultryErrors != null && PoultryErrors.Any(e => e.DateErased == null);
        public bool HasError => HasFarmError || HasPoultryError;
    }
}