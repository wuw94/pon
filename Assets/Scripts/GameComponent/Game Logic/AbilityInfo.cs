using UnityEngine.Networking;

public struct AbilityInfo
{
    public string name;
    public NetworkInstanceId source;

    public AbilityInfo(string name, NetworkInstanceId source)
    {
        this.name = name;
        this.source = source;
    }
}