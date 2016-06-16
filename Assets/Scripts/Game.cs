using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
    public static Game current;

    public LoadManager load_manager;
    public Level level;

    private void Awake()
    {
        Game.current = gameObject.GetComponent<Game>();
        Game.current.load_manager = gameObject.AddComponent<LoadManager>();
        Game.current.level = gameObject.AddComponent<Level>();
        
    }
    private void Start()
    {
        
    }
}