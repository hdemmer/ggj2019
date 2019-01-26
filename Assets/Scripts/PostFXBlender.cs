using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public class PostFXBlender : MonoBehaviour
{
	public PostProcessVolume volume;
	[Range(0f, 1f)]
	public float value;
	public Range blendingRange, solidRange;

	private void Update()
	{
		UpdateVolume();
	}

	private void UpdateVolume()
	{
		if(volume == null)
		{
			volume = GetComponent<PostProcessVolume>();
			if (volume == null) return;
		}
		if (value < blendingRange.min)
		{
			volume.priority = 0f;
		}
		else if (value < solidRange.min)
		{
			volume.priority = (value - blendingRange.min) / (solidRange.min - blendingRange.min);
		}
		else if (value < solidRange.max)
		{
			volume.priority = 1f;
		}
		else if (value < blendingRange.max)
		{
			volume.priority = (value - solidRange.max) / (blendingRange.max - solidRange.max);
		}
		else
		{
			volume.priority = 0f;
		}
	}

	[Serializable]
	public struct Range
	{
		[Range(0f, 1f)]
		public float min;
		[Range(0f, 1f)]
		public float max;
	}
}
