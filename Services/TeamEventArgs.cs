namespace RaceData.CustomData
{
    public class TeamEventArgs
    {
        public TeamEventArgs(Team team)
        {
            Team = team;
        }

        public Team Team { get; }
    }
}
