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
	public float value, center, range, blending;

	private void Update()
	{
		UpdateVolume();
	}

	private void UpdateVolume()
	{
		var solidRange = new Range()
		{
			min = center - range,
			max = center + range
		};
		var blendingRange = new Range()
		{
			min = solidRange.min - blending,
			max = solidRange.max + blending
		};

		if(volume == null)
		{
			volume = GetComponent<PostProcessVolume>();
			if (volume == null) return;
		}
		if (value < blendingRange.min)
		{
			volume.weight = 0f;
			volume.enabled = false;
		}
		else if (value < solidRange.min)
		{
			volume.weight = (value - blendingRange.min) / (solidRange.min - blendingRange.min);
			volume.enabled = true;
		}
		else if (value < solidRange.max)
		{
			volume.weight = 1f;
			volume.enabled = true;
		}
		else if (value < blendingRange.max)
		{
			volume.weight = 1f - (value - solidRange.max) / (blendingRange.max - solidRange.max);
			volume.enabled = true;
		}
		else
		{
			volume.weight = 0f;
			volume.enabled = false;
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
