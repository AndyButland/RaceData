namespace RaceData.CustomData
{
    public class StageEventArgs
    {
        public StageEventArgs(Stage stage)
        {
            Stage = stage;
        }

        public Stage Stage { get; }
    }
}
