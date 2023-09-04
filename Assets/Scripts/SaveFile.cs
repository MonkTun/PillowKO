using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveFile", menuName = "ScriptableObjects/SaveFile")]
public class SaveFile : ScriptableObject
{
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;
    public int highscore;
}
