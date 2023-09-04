using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private Dictionary<string, AudioClip> soundDictionary;

    private void Awake()
    {
        Setup();
    }

    public void Setup() //TODO bug fix
    {
        //source = GetComponentInChildren<AudioSource>();
        soundDictionary = new Dictionary<string, AudioClip>();
        for (int i = 0; i < sounds.Length; i++)
        {
            soundDictionary.Add(sounds[i].name, sounds[i]);
        }
    }

    public void PlaySound(string whichSound)
    {
        source.PlayOneShot(soundDictionary.GetValueOrDefault(whichSound), 0.7f);
    }

    public void PlaySound(string whichSound, float volume)
    {
        source.PlayOneShot(soundDictionary.GetValueOrDefault(whichSound), volume);
    }
}
