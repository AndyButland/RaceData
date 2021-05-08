namespace RaceData.CustomData
{
    public class RiderEventArgs
    {
        public RiderEventArgs(Rider rider)
        {
            Rider = rider;
        }

        public Rider Rider { get; }
    }
}
