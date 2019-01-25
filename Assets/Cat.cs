using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cat : MonoBehaviour
{
    private NavMeshAgent nma;
    
    private void OnEnable()
    {
        TheGame.Instance.cat = this;
        nma = GetComponent<NavMeshAgent>();

        StartCoroutine(Wander());
    }

    private void OnDisable()
    {
        var theGame = TheGame.Instance;
        if (theGame)
        {
            if (theGame.cat == this)
            {
                theGame.cat = null;
            }
        }
    }

    private Vector3 previousDestination;
    private IEnumerator Wander()
    {
        var speed = nma.speed;
        while (true)
        {
            var pos = transform.position;
            var targetCenter = pos + (previousDestination - pos) * 0.5f;
            var move = RandomNavSphere(targetCenter, speed, -1);
            previousDestination = move;
            if (!nma.SetDestination(move))
            {
                previousDestination = pos;
                yield return null;
            }
            else
            {
                var waitTime = Random.Range(0.5f, 0.7f);
                Debug.DrawLine(targetCenter,move, Color.red, waitTime);
                Debug.DrawLine(pos,move, Color.yellow, waitTime);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    public static Vector3 RandomNavSphere (Vector3 origin, float distance, int layermask) {
        var randomDirection = UnityEngine.Random.insideUnitSphere.normalized * distance;
               
        randomDirection += origin;
               
        NavMeshHit navHit;
               
        NavMesh.SamplePosition (randomDirection, out navHit, distance, layermask);
               
        return navHit.position;
    }

}
