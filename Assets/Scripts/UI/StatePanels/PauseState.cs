using UnityEngine;
using UnityEngine.UI;

public class PauseState : UIState
{
	public override void OnEnter()
	{
		this.gameObject.SetActive(true);

		OnPauseEnabled();
	}

	public override void OnExit()
	{
		this.gameObject.SetActive(false);
		
		OnPauseDisabled();
	}

	private void OnPauseEnabled()
	{
		Time.timeScale = 0f;
	}

	private void OnPauseDisabled()
	{
		Time.timeScale = 1f;
	}

	public void LinkAudioToSliderMaster(Slider slider)
	{
		slider.onValueChanged.RemoveAllListeners();

		slider.onValueChanged.AddListener(value =>
		{
			SoundManager.instance.SetMasterVolume(value);
		});

		float currentDB;
		if(SoundManager.instance.TryGetMixerValue("MasterVolume", out currentDB))
		{
			slider.SetValueWithoutNotify(DBToLinear(currentDB));
		}
	}

	public void LinkAudioToSliderBGM(Slider slider)
	{
		slider.onValueChanged.RemoveAllListeners();

		slider.onValueChanged.AddListener(value =>
		{
			SoundManager.instance.SetBGMVolume(value);
		});

		float currentDB;
		if (SoundManager.instance.TryGetMixerValue("BGMVolume", out currentDB))
		{
			slider.SetValueWithoutNotify(DBToLinear(currentDB));
		}
	}

	public void LinkAudioToSliderSFX(Slider slider)
	{
		slider.onValueChanged.RemoveAllListeners();

		slider.onValueChanged.AddListener(value =>
		{
			SoundManager.instance.SetSFXVolume(value);
		});

		float currentDB;
		if (SoundManager.instance.TryGetMixerValue("SFXVolume", out currentDB))
		{
			slider.SetValueWithoutNotify(DBToLinear(currentDB));
		}
	}

	public static float DBToLinear(float db)
	{
		return Mathf.Pow(10f, db / 20);
	}

	public void OnReturn()
	{
		OnPauseDisabled ();

		this.gameObject.SetActive(false);
	}
}
