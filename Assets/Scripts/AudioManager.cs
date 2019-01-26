using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[SerializeField]
	private AudioSource happySource, sadSource, glitchSource;
	[SerializeField, Range(0f, 1f)]
	private float happiness = 1f;
	public float Happiness
	{
		set
		{
			if (happiness != value)
			{
				happiness = Mathf.Clamp01(value);
				SetVolumes(); 
			}
		}
		get
		{
			return happiness;
		}
	}
	[SerializeField, Range(0f, 1f)]
	private float glitchness = 0f;
	public float Glitchness
	{
		set
		{
			if (glitchness != value)
			{
				glitchness = Mathf.Clamp01(value);
				SetVolumes();
			}
		}
		get
		{
			return glitchness;
		}
	}

#if UNITY_EDITOR
	private void Update()
	{
		SetVolumes();
	}
#endif
	private void SetVolumes()
	{
		happySource.volume = happiness;
		sadSource.volume = 1f - happiness;
		glitchSource.volume = glitchness;
	}
}
