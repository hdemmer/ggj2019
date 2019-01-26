using System.Collections;
using UnityEngine;

public class ContemplationEffect : MonoBehaviour
{
    public MeshFilter mf;
    public MeshRenderer mr;

    private static int propId = Shader.PropertyToID("_Effect");

    public void Play()
    {
        StartCoroutine(Rout());
    }

    protected IEnumerator Rout()
    {
        var t = 0f;
        var duration = 0.5f;
        while (t < duration)
        {
            t += Time.deltaTime;
            mr.material.SetFloat(propId, t / duration);
            yield return null;
        }
        mr.material.SetFloat(propId, 1f);
        yield return  new WaitForSeconds(1f);
        while (t > 0f)
        {
            t -= Time.deltaTime;
            mr.material.SetFloat(propId, t / duration);
            yield return null;
        }
        mr.material.SetFloat(propId, 0f);
        GameObject.Destroy(gameObject);
    }
}