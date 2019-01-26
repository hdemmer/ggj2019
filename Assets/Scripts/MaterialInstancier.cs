using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInstancier : MonoBehaviour
{
	private void Awake()
	{
		var mr = GetComponent<MeshRenderer>();
		if(mr != null)
		{
			var mat = mr.sharedMaterial = new Material(mr.sharedMaterial);
			var glitch = GetComponent<GlitchController>();
			if(glitch != null)
			{
				glitch.material = mat;
			}
		}
		Destroy(this);
	}
}
