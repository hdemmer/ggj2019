using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource catSource;

	public List<AudioClip> catConfused;
	public AudioClip catContemplate;
	public AudioClip catPurr;

	public AudioSource musicSource;
	public AudioSource[] glitchSources;
	public AudioSource darkPastSource;
	public AudioSource sliderSource;

	public float MAX_MUSIC = 0.2f;

	private static AudioManager _instance;

	public static AudioManager Instance => _instance;

	void Awake()
	{
		if (_instance != null)
		{
			Destroy(gameObject);	// Kill us
			return;
		}
		DontDestroyOnLoad(gameObject);
		_instance = this;
		Reset();
	}

	public void PlayCatConfused()
	{
		var clip = catConfused[Random.Range(0, catConfused.Count)];
		catSource.PlayOneShot(clip);
	}

	public void PlayCatContemplate()
	{
		catSource.PlayOneShot(catContemplate);
	}
	
	public void PlayCatPurr()
	{
		if (catSource.isPlaying) return;
		catSource.PlayOneShot(catPurr);
	}

	private float prevDizz = -1f;
	public void SetDizzy(float dizzy)
	{
		if (prevDizz == dizzy) return;

		for (var i = 0; i < glitchSources.Length; i++)
		{
			var on = dizzy > (i / (float) glitchSources.Length);
			var source = glitchSources[i];
			var vol = source.volume;
			if (on)
			{
				vol += Time.deltaTime;
			}
			else
			{
				vol -= Time.deltaTime;
			}

			vol = Mathf.Clamp01(vol);
			source.volume = vol;

			musicSource.volume = (1f - vol) * MAX_MUSIC;
		}
	}
	
	private float prevDarkPast = -1f;

	public void SetDarkPast(float darkPast)
	{
		if (prevDarkPast == darkPast) return;
		var vol = darkPastSource.volume;
		if (darkPast > vol)
		{
			vol += Time.deltaTime;
		}
		else
		{
			vol -= Time.deltaTime;
		}

		vol = Mathf.Clamp01(vol);
		darkPastSource.volume = vol;
	}

	public void UseSlider()
	{
		var vol = sliderSource.volume;
		vol += 0.3f;
		vol = Mathf.Clamp01(vol);
		sliderSource.volume = vol;
	}
	
	protected void Update()
	{
		var vol = sliderSource.volume;
		vol *= 0.8f;
		vol = Mathf.Clamp01(vol);
		sliderSource.volume = vol;
	}

	public void Reset()
	{
		for (var i = 0; i < glitchSources.Length; i++)
		{
			var source = glitchSources[i];
			source.volume = 0f;
		}

		darkPastSource.volume = 0f;
		sliderSource.volume = 0f;
		musicSource.volume = MAX_MUSIC;
		catSource.Stop();
	}
}
