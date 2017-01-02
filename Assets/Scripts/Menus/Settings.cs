using System.Collections.Generic;

public static class Settings
{
    public enum WaitTypes
    {
        CREATE_MATCH_CALLBACK,
        DESTROY_MATCH_CALLBACK,
        CREATE_MATCH,
        JOIN_MATCH
    };

    public static Dictionary<WaitTypes, string> WAIT_MESSAGE = new Dictionary<WaitTypes, string>()
    {
        { WaitTypes.CREATE_MATCH_CALLBACK, "Creating Game" },
        { WaitTypes.DESTROY_MATCH_CALLBACK, "Destroying Game" },
        { WaitTypes.CREATE_MATCH, "Creating Game" },
        { WaitTypes.JOIN_MATCH, "Joining Game" }
    };


    /// <summary>
    /// An list that is set to what we're waiting for. If the list is empty, we're not waiting for anything.
    /// </summary>
    public static List<WaitTypes> WAIT_FOR = new List<WaitTypes>();


    public static string letters = "";
    public static string PLAYER_NAME = "";

    public static string[] LOADING_WORDS = new string[]{"a loading screen...",
                                                        "the wait begins...",
                                                        "systems warming up...",
                                                        "do not press any key to continue...",
                                                        "preparing game...",
                                                        "your entertainment awaits..."};
    public static int LOADING_WORDS_INDEX = 0;

    public static string GetLoadingWords()
    {
        return Settings.LOADING_WORDS[Settings.LOADING_WORDS_INDEX];
    }

    public static void RandomizeLoadingWords()
    {
        Settings.LOADING_WORDS_INDEX = UnityEngine.Random.Range(0, Settings.LOADING_WORDS.Length);
    }

    public static bool IsValidName(string s)
    {
        if (s == "")
            return false;
        return new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]*$").IsMatch(s);
    }
}