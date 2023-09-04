using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
	[SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixer audioMixer;

    private (float min, float max) _lowPassMinMax = (500f, 22000f);

    private const string LOW_PASS_CUTOFF = "LowPassCutoff";

    public bool PlayMusic(AudioClip musicClip, bool loop, float pitch, float volume, bool onlyWhenIsNotPlaying)
    {
        if (musicSource.isPlaying && onlyWhenIsNotPlaying) return false;
        StopCoroutine("PlayMusicRoutine");
        StartCoroutine(PlayMusicRoutine(musicClip, loop, pitch, volume));
        return true;
    }

    IEnumerator PlayMusicRoutine(AudioClip musicClip, bool loop, float pitch, float volume)
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(FadeVolume(0.3f, 0));

            print(musicSource.volume);
            yield return new WaitForSeconds(0.35f);
        }


        musicSource.clip = musicClip;
        musicSource.loop = loop;
        musicSource.volume = volume;
        print(musicSource.volume);
        musicSource.Play();
        musicSource.pitch = pitch;
    }

    public void StopMusic()
    {
        StartCoroutine(StopMusicRoutine());
    }


    IEnumerator StopMusicRoutine()
    {
        StartCoroutine(FadeVolume(0.5f, 0));

        yield return new WaitForSeconds(0.5f);

        musicSource.Stop();
    }

    IEnumerator FadeVolume(float fadeDuration, float targetVolume)
    {
        float start = musicSource.volume;
        float currentTime = 0;

        while (currentTime < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(start, targetVolume, currentTime / fadeDuration);

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

	public void UpdateVolume(float fadeDuration, float targetVolume)
	{
		StartCoroutine(FadeVolume(fadeDuration, targetVolume));
	}
    public void UpdatePitch(float fadeDuration, float targetPitch)
    {
        StartCoroutine(FadePitch(fadeDuration, targetPitch));
    }

    IEnumerator FadePitch(float fadeDuration, float targetPitch)
    {
        float start = musicSource.pitch;
        float currentTime = 0;

        while (currentTime < fadeDuration)
        {
            musicSource.pitch = Mathf.Lerp(start, targetPitch, currentTime / fadeDuration);

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    #region play music - LowPassCutOffFilter

    public void SetLowPassCutOff(float duration, float f)
    {
        StartCoroutine(FadeLowPassFilter(duration, f));
    }

    void SetLowPassFilter(float lowPassTransitionDirection)
    {
        float lowPassValue = Mathf.Lerp(_lowPassMinMax.min, _lowPassMinMax.max, lowPassTransitionDirection);
        audioMixer.SetFloat(LOW_PASS_CUTOFF, lowPassValue);
        //print(lowPassValue);
    }

    IEnumerator FadeLowPassFilter(float fadeDuration, float targetPass)
    {
        float currentTime = 0;

        while (currentTime < fadeDuration)
        {
            SetLowPassFilter(Mathf.Lerp(0, targetPass, currentTime / fadeDuration));

            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    #endregion
}
