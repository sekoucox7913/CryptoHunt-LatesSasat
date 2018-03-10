using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour 
{
	public static SoundManagerScript instance;
	public AudioSource BtnSound; 
	public AudioSource SlideSound;
	public AudioSource PlayingSound;
	public AudioSource MagicSound;



	//ok this is 2 new loop sounds, and one extra button sound for back button

	public AudioSource backBtn;
	public AudioSource charSelectMusic;

	public AudioSource ToggleButtonSound;


    public AudioSource InMenuMusic; // this is it mean


	void Awake()
	{
		instance = this;
	}
	/// <summary>
	/// Raises the play button click sound event.
	/// when you click on button
	/// </summary>
	internal void OnPlayButtonClickSound()
	{
		BtnSound.mute = !UserSettingScript.instance.TglSFX.isOn;
		BtnSound.Play ();
	}
	/// <summary>
	/// Raises the play slide sound event.
	/// when the open menu and also open any animated panel (Left to center, center to right, etc mean show panel and hide panel)
	/// </summary>
	internal void OnPlaySlideSound()
	{
		SlideSound.mute = !UserSettingScript.instance.TglSFX.isOn;
		SlideSound.Play ();
	}

	/// <summary>
	/// Raises the play playing sound event.
	/// 
	/// </summary>
	internal void OnPlayPlayingSound()
	{
		PlayingSound.mute = !UserSettingScript.instance.TglSound.isOn;
		InMenuMusic.Stop();  
		charSelectMusic.Stop (); 
		PlayingSound.Play ();
	}

	/// <summary>
	/// Raises the play magic sound event.
	/// when you are click on magic box
	/// </summary>
	internal void OnPlayMagicSound()
	{
		MagicSound.mute = !UserSettingScript.instance.TglSFX.isOn;
		MagicSound.Play ();
	}

	/// <summary>
	/// Raises the vibration event.
	/// if vibration mode is on then this function called (open chest time)
	/// </summary>
	internal void OnVibration()
	{
		if(UserSettingScript.instance.TglVibration.isOn)
		{
			Handheld.Vibrate();
		}
	}

	/// <summary>
	/// Ins the menu clicked.
	/// when you are click on menu
	/// </summary>
    internal void InMenuClicked()
    {
        InMenuMusic.mute = !UserSettingScript.instance.TglSound.isOn;
		PlayingSound.Stop(); 
		charSelectMusic.Stop (); 
        InMenuMusic.Play();
    }
	///for example 
	/// 

	internal void OnBackButtonClick()
	{
		backBtn.mute = !UserSettingScript.instance.TglSFX.isOn;
		backBtn.Play ();
	}

	internal void OnCharacterSelectSound()
	{
		charSelectMusic.mute = !UserSettingScript.instance.TglSound.isOn;
		PlayingSound.Stop ();
		InMenuMusic.Stop ();
		charSelectMusic.Play ();
	}
	internal void OnToggleButtonClick()
	{
		ToggleButtonSound.mute = !UserSettingScript.instance.TglSFX.isOn;
		ToggleButtonSound.Play ();
	}

	internal void OnAllSoundMute()
	{
		PlayingSound.Pause(); 
		charSelectMusic.Pause (); 
		InMenuMusic.Pause();

//		AudioListener[] listerobjects = GetComponents<AudioListener>();
//		if (listerobjects != null) {
//			for (int i = 0; i < listerobjects.Length; i++) {
//				listerobjects [i].enabled = false;
//			}
//		}
	}
	internal void OnAllSoundUnMute()
	{
		PlayingSound.UnPause(); 
		charSelectMusic.UnPause (); 
		InMenuMusic.UnPause();
//		AudioListener[] listerobjects = GetComponents<AudioListener>();
//		print (listerobjects.Length);
//		if (listerobjects != null) {
//			for (int i = 0;  i < listerobjects.Length; i++) {
//				listerobjects [i].enabled = true;
//			}
//		}
	}
}
