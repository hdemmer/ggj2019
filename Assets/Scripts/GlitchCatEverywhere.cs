using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchCatEverywhere : MonoBehaviour
{
	public Transform cat, rot;
	public Range interval, decenterX, decenterY, distance, rotation;
	[Range(0f, .5f)]
	public float margin;
	public float glitchTime;

	private IEnumerator Start()
	{
		var camera = Camera.main;
		var glitch = GetComponent<GlitchController>();

		var center = camera.ViewportToWorldPoint(new Vector3(.5f, .5f, .5f));

		while (true)
		{
			var st = Time.time;
			var part = 0f;
			while(part < 1f)
			{
				yield return null;
				part = (Time.time - st) / glitchTime;
				glitch.disappear = part;
				glitch.CallUpdate();
			}
			Place(camera, center);
			st = Time.time;
			part = 0f;
			while (part < 1f)
			{
				yield return null;
				part = (Time.time - st) / glitchTime;
				glitch.disappear = 1f - part;
				glitch.CallUpdate();
			}
			yield return new WaitForSeconds(interval.random);
		}
	}

	private void Place(Camera camera, Vector3 center)
	{
		var pivot = center + new Vector3(decenterX.random, decenterY.random);
		var dist = distance.random;
		var pos = cat.position;
		var target = (pivot - pos).normalized * dist;
		var viewPos = camera.WorldToViewportPoint(target);
		if (viewPos.x < margin)
			viewPos.x = margin;
		if (viewPos.x > 1f - margin)
			viewPos.x = 1f - margin;
		if (viewPos.y < margin)
			viewPos.y = margin;
		if (viewPos.y > 1f - margin)
			viewPos.y = 1f - margin;
		target = camera.ViewportToWorldPoint(viewPos);
		target.z = 0;
		cat.position = target;
		cat.rotation = rot.rotation;
		cat.Rotate(Vector3.up, rotation.random, Space.World);
	}
}

[System.Serializable]
public struct Range
{
	public float min;
	public float max;

	public float random
	{
		get
		{
			return Random.Range(min, max);
		}
	}
}
