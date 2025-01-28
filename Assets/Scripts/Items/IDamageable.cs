// Interface used by any object that can be damaged/collided by an item.
public interface IDamageable
{
    // Returns the team in witch the object is
    public uint GetTeam();

    // Sets the team in witch the object is
    public void SetTeam(uint team);
}
