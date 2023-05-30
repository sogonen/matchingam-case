public class IslandPair
{
    public Island FirstIsland { get; private set; }
    public Island SecondIsland { get; private set; }


    public IslandPair(Island firstIsland, Island secondIsland)
    {
        FirstIsland = firstIsland;
        SecondIsland = secondIsland;
    }
}