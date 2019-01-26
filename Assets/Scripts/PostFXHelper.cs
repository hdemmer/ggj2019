using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostFXHelper : MonoBehaviour
{
	public Slider slider;
	private PostFXBlender[] blenders;

	private void Awake()
	{
		blenders = GetComponentsInChildren<PostFXBlender>();
	}

	private void Update()
	{
		if (slider == null) return;

		var v = slider.value;
		foreach(PostFXBlender b in blenders)
		{
			b.value = v;
		}
	}
}
