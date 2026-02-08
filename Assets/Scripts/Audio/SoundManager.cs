using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct AudioConfig
{
	public AudioClip clip;
	public AudioMixerGroup group;
	public bool isLooping;
}

public class SoundManager : MonoBehaviour
{
	[SerializeField] private AudioConfig clickSFX_Config;
	[SerializeField] private AudioConfig baseBGM_Config;
	[SerializeField] private AudioConfig fishDeathSFX_Config;
	[SerializeField] private AudioConfig buttonSFX_Config;

	[SerializeField] private AudioMixer audioMixer;

	[SerializeField] private AudioSource bgmSource;
	[SerializeField] private AudioSource sfxSource;

	public static SoundManager instance;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		
		DontDestroyOnLoad(gameObject);
		bgmSource.loop = true;

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		PlayBGM();
	}

	public void PlayBGM()
	{
		AudioClip clip = baseBGM_Config.clip;
		if (clip == null) return;

		bgmSource.clip = clip;
		bgmSource.outputAudioMixerGroup = baseBGM_Config.group;
		bgmSource.Play();
	}

	public void PlayClickSFX()
	{
		PlaySFX(clickSFX_Config);
	}

	public void PlayFishDeathSFX()
	{
		PlaySFX(fishDeathSFX_Config);
	}

	public void PlayButtonSFX()
	{
		PlaySFX(buttonSFX_Config);
	}

	private void PlaySFX(AudioConfig audioConfig)
	{
		if (audioConfig.clip == null) return;

		sfxSource.outputAudioMixerGroup = audioConfig.group;

		sfxSource.PlayOneShot(audioConfig.clip);
	}

	#region Audio Slider control

	public void SetMasterVolume(float value)
	{
		audioMixer.SetFloat("MasterVolume", ConbvertToDB(value));
	}

	public void SetBGMVolume(float value)
	{
		audioMixer.SetFloat("BGMVolume", ConbvertToDB(value));
	}

	public void SetSFXVolume(float value)
	{
		audioMixer.SetFloat("SFXVolume", ConbvertToDB(value));
	}

	private float ConbvertToDB(float value)
	{
		value = Mathf.Clamp(value, 0.0001f, 1f);
		return Mathf.Log10(value) * 20f;
	}

	#endregion
}
