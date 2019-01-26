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
	public float disappear;

	public MeshRenderer meshR;

	public float maxGridSize = .2f;
	public float glitchNoise;
	public const float minGridSize = 0.0001f;

	private static readonly int _gridSize = Shader.PropertyToID("_GridSize");
	private static readonly int _colorGlitchValue = Shader.PropertyToID("_ColorGlitchValue");
	private static readonly int _disappear = Shader.PropertyToID("_Disappear");

	private float prevGlitch = -1f;
	private float prevDisappear = -1f;
	
	public void CallUpdate()
	{
		if (prevGlitch != glitch || prevDisappear != disappear)
		{
			prevGlitch = glitch;
			prevDisappear = disappear;
			
			var glitchValue = Mathf.Clamp01(glitch + disappear * .8f);
			material.SetFloat(_gridSize,
				Mathf.Lerp(minGridSize, maxGridSize,
					glitchValue + glitchValue * (Mathf.PerlinNoise(3f, Time.time * 8) * 2f - 1f) * glitchNoise));
			material.SetFloat(_colorGlitchValue, glitchValue);
			material.SetFloat(_disappear, disappear);
			meshR.enabled = (disappear < 1f);
		}
	}
}
