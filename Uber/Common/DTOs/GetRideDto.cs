using Common.Enums;

namespace Common.DTOs
{
    public class GetRideDto
    {
        public Guid Id { get; set; }
        public string StartingPoint { get; set; } = string.Empty;
        public string EndingPoint { get; set; } = string.Empty;
        public int Price { get; set; }
        public int DriverTimeInSeconds { get; set; }
        public int ArrivalTimeInSeconds { get; set; } = 0;
        public Guid DriverId { get; set; }
        public Guid PassengerId { get; set; }
        public RideStatus Status { get; set; } = RideStatus.Pending;
    }
}
