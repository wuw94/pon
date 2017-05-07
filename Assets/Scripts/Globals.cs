using System.Collections.Generic;

public static class Globals
{

    public enum WaitTypes
    {
        CREATE_MATCH_CALLBACK,
        DESTROY_MATCH_CALLBACK,
        CREATE_MATCH,
        JOIN_MATCH,
        QUIT_GAME
    };

    public static Dictionary<WaitTypes, string> WAIT_MESSAGE = new Dictionary<WaitTypes, string>()
    {
        { WaitTypes.CREATE_MATCH_CALLBACK, "Creating Game" },
        { WaitTypes.DESTROY_MATCH_CALLBACK, "Destroying Game" },
        { WaitTypes.CREATE_MATCH, "Creating Game" },
        { WaitTypes.JOIN_MATCH, "Joining Game" },
        { WaitTypes.QUIT_GAME, "Quitting Game" }
    };


    /// <summary>
    /// An list that is set to what we're waiting for. If the list is empty, we're not waiting for anything.
    /// </summary>
    public static List<WaitTypes> WAIT_FOR = new List<WaitTypes>();


    public static string letters = "";

    public static string[] LOADING_WORDS = new string[]{"a loading screen...",
                                                        "the wait begins...",
                                                        "systems warming up...",
                                                        "do not press any key to continue...",
                                                        "preparing game...",
                                                        "your entertainment awaits..."};
    public static int LOADING_WORDS_INDEX = 0;

    public static string GetLoadingWords()
    {
        return Globals.LOADING_WORDS[Globals.LOADING_WORDS_INDEX];
    }

    public static void RandomizeLoadingWords()
    {
        Globals.LOADING_WORDS_INDEX = UnityEngine.Random.Range(0, Globals.LOADING_WORDS.Length);
    }

    

    

    // SERVER STUFF

}