using System.Runtime.Serialization;

namespace Common.Models
{
    [DataContract]
    public class RatingModel
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public double Rating { get; set; } = 0;
        [DataMember]
        public int NumOfRates { get; set; } = 0;
    }
}
