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

	public float maxGridSize = .2f;
	public float glitchNoise;
	public const float minGridSize = 0.0001f;

	private static int _gridSize = Shader.PropertyToID("_GridSize");
	private static int _colorGlitchValue = Shader.PropertyToID("_ColorGlitchValue");
	private static int _disappear = Shader.PropertyToID("_Disapear");
		
	
	public void CallUpdate()
	{
		if(material != null)
		{
			var glitchValue = Mathf.Clamp01(this.glitch + disappear * .8f);
			material.SetFloat(_gridSize, Mathf.Lerp(minGridSize, maxGridSize, glitchValue + glitchValue * (Mathf.PerlinNoise(3f, Time.time * 8) * 2f - 1f) * glitchNoise));
			material.SetFloat(_colorGlitchValue, glitchValue);
			material.SetFloat(_disappear, disappear);
		}
	}
}
