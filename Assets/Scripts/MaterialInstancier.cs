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
			var mats = new Material[mr.sharedMaterials.Length];
			for(int i = 0; i < mats.Length; i++)
			{
				mats[i] = mr.sharedMaterials[i] = new Material(mr.sharedMaterials[i]);
			}
			var glitch = GetComponent<GlitchController>();
			if(glitch != null)
			{
				glitch.materials = mats;
			}
		}
		Destroy(this);
	}
}
