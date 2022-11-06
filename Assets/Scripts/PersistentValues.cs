using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentValues
{
    public static Difficulty GameDifficulty {get; set;}
    public static float MusicVolume {get; set;}
    public static float SFXVolume {get; set;}
    public static bool IsFirstScene {get; set;} = true;
}
