using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Difficulty Settings")]
public class GameDifficultySettings : ScriptableObject
{
    public int ballLimit;
    public int playerStartHealth;
    public float timeLimit;
}
