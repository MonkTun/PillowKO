using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SettingsManager : MonoBehaviour
{
	[SerializeField] private AudioMixer AX;
	[SerializeField] private Slider masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider;

	private void Awake()
	{
		
	}

	public int GetHighestScore()
	{
		return SavesManager.SavePlayerData.bestScore;
	}

	public void UpdateHighestScore(int score)
	{
		SavesManager.SetBestScore(score);
		//saveFile.highscore = score;
	}

	public void AdjustMasterVolume(float value)
	{

		if (value <= -19)
		{
			value = -80;
		}

		//saveFile.masterVolume = value;
		SavesManager.SetMasterVolume(value);


		AX.SetFloat("MasterVolume", SavesManager.SavePlayerData.masterVolume);
	}

	public void LoadMasterVolume()
	{
		masterVolumeSlider.value = SavesManager.SavePlayerData.masterVolume;
		AX.SetFloat("MasterVolume", SavesManager.SavePlayerData.masterVolume);
	}

	public void AdjustMusicVolume(float value)
	{

		if (value <= -19)
		{
			value = -80;
		}

		//saveFile.musicVolume = value;
		SavesManager.SetMusicVolume(value);


		AX.SetFloat("MusicVolume", SavesManager.SavePlayerData.musicVolume);
	}

	public void LoadMusicVolume()
	{
		musicVolumeSlider.value = SavesManager.SavePlayerData.musicVolume;
		AX.SetFloat("MusicVolume", SavesManager.SavePlayerData.musicVolume);
	}

	public void AdjustSFXVolume(float value)
	{

		if (value <= -19)
		{
			value = -80;
		}

		//saveFile.sfxVolume = value;
		SavesManager.SetSFXVolume(value);

		AX.SetFloat("SFXVolume", SavesManager.SavePlayerData.sfxVolume);
	}

	public void LoadSFXVolume()
	{
		sfxVolumeSlider.value = SavesManager.SavePlayerData.sfxVolume;
		AX.SetFloat("SFXVolume", SavesManager.SavePlayerData.sfxVolume);
	}
}
