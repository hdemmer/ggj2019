﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cat : MonoBehaviour
{
    private readonly Queue<HistoryItem> targets = new Queue<HistoryItem>();
    
    private NavMeshAgent nma;
    private Coroutine currentBehaviour;

    [SerializeField]
    private float courage = 0f;

    public enum State
    {
        None,
        Wander,
        Seek
    }

    public State state;

    private void Awake()
    {
        nma = GetComponent<NavMeshAgent>();
    }

    public void CallStart()
    {
        for (var i = 0; i < 10; i++)
        {
            ChooseTarget();
        }
        Do(Wander());
    }

    private void ChooseTarget()
    {
        var allItems = TheGame.Instance.items;
        var randomOne = allItems[Random.Range(0, allItems.Length)];
        QueueItem(randomOne);
    }

    private void Stop()
    {
        state = State.None;
        if (currentBehaviour != null)
        {
            StopCoroutine(currentBehaviour);
            currentBehaviour = null;
        }
    }

    private void Do(IEnumerator action)
    {
        Stop();
        currentBehaviour = StartCoroutine(action);
    }

    private Vector3 previousDestination;
    private IEnumerator Wander()
    {
        yield return null;
        state = State.Wander;
        Debug.Log("WANDER");
        var speed = nma.speed;
        while (true)
        {
            var pos = transform.position;
            var targetCenter = pos + (previousDestination - pos) * 0.5f;
            var move = RandomNavSphere(targetCenter, speed, -1);
            previousDestination = move;
            var t = Time.time;
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
            
            courage += (Time.time - t);
            courage = Mathf.Clamp(courage, -1f, 1f);
            if (courage > 0.5f)
            {
                courage -= 0.5f;
                Do(SeekNext());
            }
        }
    }

    private IEnumerator SeekNext()
    {
        yield return null;
        state = State.Seek;
        
        if (targets.Count == 0)
        {
            Debug.Log("SEEK NO ITEM");

            Do(Wander());
            yield break;
        }

        var target = targets.Dequeue();
        Debug.Log("SEEK "+ target);

        var destination = target.transform.position + target.transform.forward;
        if (!nma.SetDestination(destination))
        {
            Do(Wander());
            yield break;
        }

        while (true)
        {
            var d = (transform.position - destination);
            d.y = 0f;
            if (d.sqrMagnitude < 0.1f)
            {
                // there!
                Debug.Log("THERE");
                var opacity = target.GetOpacity();
                if (opacity < 0.9f)
                {
                    Debug.Log("ITEM NOT THERE");
                    courage -= 1f;
                    Do(Wander());
                }
                else
                {
                    Debug.Log("CONTEMPLATING");
                    yield return new WaitForSeconds(Random.Range(0.5f, 1f)); // contemplate
                    ChooseTarget();
                    Do(SeekNext());
                }

                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void QueueItem(HistoryItem item)
    {
        targets.Enqueue(item);
    }

    private static Vector3 RandomNavSphere (Vector3 origin, float distance, int layermask) {
        var randomDirection = UnityEngine.Random.insideUnitSphere.normalized * distance;
               
        randomDirection += origin;
               
        NavMeshHit navHit;
               
        NavMesh.SamplePosition (randomDirection, out navHit, distance, layermask);
               
        return navHit.position;
    }

}
