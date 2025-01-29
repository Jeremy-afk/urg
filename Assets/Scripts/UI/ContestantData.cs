using System;

[Serializable]
public struct ContestantData
{
    public string Name;
    public int Placement;
    public bool IsYou;

    public ContestantData(string name, int placement, bool isYou)
    {
        Name = name;
        Placement = placement;
        IsYou = isYou;
    }
};