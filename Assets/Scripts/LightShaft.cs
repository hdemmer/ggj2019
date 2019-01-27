using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightShaft : MonoBehaviour
{
	[Range(.1f, 4f)]
	public float width;
	[SerializeField]
	private Vector3 from, to;

	[SerializeField]
	private Transform _camera;
	public Transform cam
	{
		get
		{
			return _camera ?? (_camera = FindObjectOfType<Camera>().transform);
		}
	}
#if UNITY_EDITOR

	//protected virtual void OnSceneGUI()
	//{
		
	//}

#endif
	private void Place()
	{
		transform.position = (from + to) * .5f;
		var delta = from - to;
		var dir = delta.normalized;
		transform.rotation = Quaternion.LookRotation(cam.forward, dir);
		transform.localScale = new Vector3(width, delta.magnitude, 1f);
	}

#if UNITY_EDITOR
[CustomEditor(typeof(LightShaft)), CanEditMultipleObjects]
public class PositionHandleExampleEditor : Editor
{
	protected virtual void OnSceneGUI()
	{
		LightShaft example = (LightShaft)target;

		example.from = Handles.PositionHandle(example.from, Quaternion.identity);
		example.to = Handles.PositionHandle(example.to, Quaternion.identity);

		example.Place();
	}
}
#endif

}