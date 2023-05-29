public class IslandPair
{
    public Island Island1 { get; private set; }
    public Island Island2 { get; private set; }

    public IslandPair(Island island1, Island island2)
    {
        Island1 = island1;
        Island2 = island2;
    }
}