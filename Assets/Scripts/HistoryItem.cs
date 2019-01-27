using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryItem : MonoBehaviour
{
	public int startTimeline = 9;
	public int endTimeline = 9;
	public GlitchController[] gcs = new GlitchController[0];

	[SerializeField]
	private float opacity = 0f;
	
	

	private void OnEnable()
	{
		var mrs = gameObject.GetComponentsInChildren<MeshRenderer>();
		gcs = new GlitchController[mrs.Length];
		for (var i = 0; i < mrs.Length; i++)
		{
			var meshRenderer = mrs[i];
			var gc = meshRenderer.gameObject.GetComponent<GlitchController>();
			if (!gc)
			{
				gc = meshRenderer.gameObject.AddComponent<GlitchController>();
			}
			gc.meshR = meshRenderer;
			gc.materials = meshRenderer.materials;
			gcs[i] = gc;
		}
	}

	private float previousTimeline = -1;
	public void CallUpdate(float timeline)
	{
		if (timeline != previousTimeline)
		{
			previousTimeline = timeline;
			var opacity = TheGame.Instance.CurrentFade(startTimeline, endTimeline);

			foreach (var gc in gcs)
			{
				gc.disappear = 1f - opacity;
				gc.CallUpdate();
			}

			this.opacity = opacity;
		}

	}

	public float GetOpacity()
	{
		return opacity;
	}

	public void StartGlitch()
	{
		foreach (var gc in gcs)
		{
			gc.StartGlitch();
		}
	}
	
	public void StopGlitch()
	{
		foreach (var gc in gcs)
		{
			gc.StopGlitch();
		}
	}
}
