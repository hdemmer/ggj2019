using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlitchController : MonoBehaviour
{
	public Material material;
	[Range(0f, 1f)]
	public float glitch;
	[Range(0f, 1f)]
	public float disapear;

	public float maxGridSize = .2f;
	public float glitchNoise;
	public const float minGridSize = 0.0001f;

	void Update()
	{
		if(material != null)
		{
			var glitch = Mathf.Clamp01(this.glitch + disapear * .8f);
			material.SetFloat("_GridSize", Mathf.Lerp(minGridSize, maxGridSize, glitch + glitch * (Mathf.PerlinNoise(3f, Time.time * 8) * 2f - 1f) * glitchNoise));
			material.SetFloat("_ColorGlitchValue", glitch);
			material.SetFloat("_Disapear", disapear);
		}
	}
}
