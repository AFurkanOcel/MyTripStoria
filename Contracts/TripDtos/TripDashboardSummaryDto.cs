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
    }
}
