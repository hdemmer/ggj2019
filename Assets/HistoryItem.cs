using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryItem : MonoBehaviour
{
	public int startTimeline = 9;
	public int endTimeline = 9;
	private GlitchController gc;

	[SerializeField]
	private float opacity = 0f;

	private void OnEnable()
	{
		gc = GetComponent<GlitchController>();
	}

	private float previousTimeline = -1;
	public void CallUpdate(float timeline)
	{
		if (timeline != previousTimeline && gc != null)
		{
			previousTimeline = timeline;
			var opacity = TheGame.Instance.CurrentFade(startTimeline, endTimeline);

			gc.disapear = 1f - opacity;

			this.opacity = opacity;
		}

	}

	public float GetOpacity()
	{
		return opacity;
	}
}
