using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	public float speed, distance;

	private void Update()
	{
		transform.localPosition = new Vector3(0f, 0f, Mathf.Sin(Time.time * speed) * distance);
	}
}
