using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlitchController : MonoBehaviour
{
	public Material[] materials;
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

	private float glitchDisappear = 0f;

	public void CallUpdate()
	{
		if (prevGlitch != glitch || prevDisappear != disappear)
		{
			prevGlitch = glitch;
			prevDisappear = disappear;

			var glitchValue = Mathf.Clamp01(glitch + disappear * .8f);
			var gridSize = Mathf.Lerp(minGridSize, maxGridSize, glitchValue + glitchValue * (Mathf.PerlinNoise(3f, Time.time * 8) * 2f - 1f) * glitchNoise);
			foreach (Material material in materials)
			{
				material.SetFloat(_gridSize, gridSize);
				material.SetFloat(_colorGlitchValue, glitchValue);
				material.SetFloat(_disappear, disappear + glitchDisappear);
			}
			meshR.enabled = (disappear < 1f);
		}
	}

	private Coroutine _glitchRoutine;
	public void StartGlitch()
	{
		if (_glitchRoutine != null)
		{
			StopCoroutine(_glitchRoutine);
			_glitchRoutine = null;
		}

		_glitchRoutine = StartCoroutine(GlitchRoutine());
	}

	public void StopGlitch()
	{
		if (_glitchRoutine != null)
		{
			StopCoroutine(_glitchRoutine);
			_glitchRoutine = null;
		}

		_glitchRoutine = StartCoroutine(StopGlitchRoutine());
	}

	private IEnumerator GlitchRoutine()
	{
		var t = glitchDisappear;
		while (true)
		{
			var dt = Time.deltaTime;
			t += Random.Range(-1f * dt, 2f * dt);
			t = Mathf.Clamp01(t);

			glitchDisappear = t * 0.5f;

			foreach (Material material in materials)
			{
				material.SetFloat(_disappear, disappear + glitchDisappear);
			}
			yield return null;
		}
	}

	private IEnumerator StopGlitchRoutine()
	{
		while (glitchDisappear > 0.01f)
		{
			glitchDisappear -= Time.deltaTime;
			foreach (Material material in materials)
			{
				material.SetFloat(_disappear, disappear + glitchDisappear);
			}
			yield return null;
		}
	}
}
