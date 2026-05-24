namespace Contracts.TripDtos
{
    public class TripDashboardSummaryDto
    {
        public int TotalTrips { get; set; }
        public int PlannedTrips { get; set; }
        public int OngoingTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledTrips { get; set; }
        public decimal TotalPlannedBudget { get; set; }
        public decimal TotalActualCost { get; set; }
        public DateTime? NextTripStartDate { get; set; }
        public string? NextTripTitle { get; set; }
        public int VisitedCountryCount { get; set; }
        public int VisitedCityCount { get; set; }
        public int TotalTravelDays { get; set; }
    }
}
