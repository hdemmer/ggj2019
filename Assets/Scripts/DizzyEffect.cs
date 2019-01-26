using UnityEngine;

public class DizzyEffect : MonoBehaviour
{
    public ImageEffectApplier target;
    public float multiplier = 1f;

    void Start()
    {
        target = GetComponent<ImageEffectApplier>();
    }

    private void Update()
    {
        var dizziness = TheGame.Instance.GetTotalDizziness();
        dizziness = Mathf.Clamp01(dizziness) * multiplier;
        target.material.SetFloat("_Distortion", dizziness);
    }
}
