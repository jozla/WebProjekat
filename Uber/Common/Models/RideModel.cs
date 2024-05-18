using Common.Enums;
using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class RideModel
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string StartingPoint { get; set; } = string.Empty;
        [DataMember]
        public string EndingPoint { get; set; } = string.Empty;
        [DataMember]
        public int Price { get; set; }
        [DataMember]
        public int DriverTimeInSeconds { get; set; }
        [DataMember]
        public int ArrivalTimeInSeconds { get; set; } = 0;
        [DataMember]
        public Guid DriverId { get; set; }
        [DataMember]
        public Guid PassengerId { get; set; }
        [DataMember]
        public RideStatus Status { get; set; } = RideStatus.Pending;
    }
}
