using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Cat : MonoBehaviour
{
    public readonly List<CatItem> targets = new List<CatItem>();
    
    private NavMeshAgent nma;
    private Coroutine currentBehaviour;
    
    public List<Collider> touchingColliders = new List<Collider>();

    public float courageTuning = 0.3f;

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
        GetComponent<Collider>().isTrigger = true;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.O))
        {
            StopAllCoroutines();
            targets.RemoveAt(0);
            Do(SeekNext());
        }
    }

    public void CallStart()
    {
        var lastStage = -1;
        var allItems = new List<CatItem>( TheGame.Instance.catItems);
        for (var i = 9; i >= 1; i--)
        {
            var distance = 3;
            if (distance > i) distance = i;
            var itemsInThisStage = allItems.FindAll((catItem) =>
            {
                return catItem.startTimeline >= i && i <= catItem.endTimeline + distance;
            });
            for (var j = 0; j < 2; j++)
            {
                if (itemsInThisStage.Count > 0)
                {
                    var pick = itemsInThisStage[Random.Range(0, itemsInThisStage.Count)];
                    if (!targets.Contains(pick) && lastStage != pick.startTimeline)
                    {
                        Debug.Log("ADDING " + pick);
                        targets.Add(pick);
                        lastStage = pick.startTimeline;
                    }
                }
            }
        }
        Do(Wander());
    }

    private void OnTriggerEnter( Collider other)
    {
        touchingColliders.Add(other);
        var hi = other.GetComponent<HistoryItem>();
        if (hi)
        {
            hi.StartGlitch();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        touchingColliders.Remove(other);
        var hi = other.GetComponent<HistoryItem>();
        if (hi)
        {
            hi.StopGlitch();
        }
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
            
            courage += (Time.time - t) * courageTuning;
            courage = Mathf.Clamp(courage, -1f, 1f);
            if (courage > 0.5f)
            {
                AudioManager.Instance.PlayCatPurr();
                courage -= 0.5f;
                Do(SeekNext());
                yield break;
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

            TheGame.Instance.GameOver();

            Do(Wander());
            yield break;
        }

        var target = targets[0];
        var targetCollider = target.GetComponent<Collider>();
        Debug.Log("SEEK "+ target);

        var targetPos = target.transform.position;
        var offset = new Vector3(0.8f,0f,0.8f);
        var destination = targetPos + offset;
        destination.y = 0f;
        if (!nma.SetDestination(destination))
        {
            Do(Wander());
            yield break;
        }

        while (true)
        {
            var d = (transform.position - destination);
            d.y = 0f;
            Debug.DrawLine(transform.position, destination, Color.magenta, 0.1f);
            if (d.sqrMagnitude < 0.1f || (targetCollider != null && touchingColliders.Contains(targetCollider)))
            {
                // there!
                Debug.Log("THERE");
                var opacity = target.GetOpacity();
                if (opacity < 0.9f)
                {
                    AudioManager.Instance.PlayCatConfused();
                    Debug.Log("ITEM NOT THERE");
                    courage = -1f;
                    Do(Wander());
                    yield break;
                }
                else
                {
                    AudioManager.Instance.PlayCatContemplate();
                    TheGame.Instance.PlayContemplateParticles(target);

                    Debug.Log("CONTEMPLATING");
                    targets.RemoveAt(0);
                    nma.destination = transform.position;
                    yield return new WaitForSeconds(Random.Range(1.5f, 2f)); // contemplate
                    Do(SeekNext());
                    yield break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private static Vector3 RandomNavSphere (Vector3 origin, float distance, int layermask) {
        var randomDirection = UnityEngine.Random.insideUnitSphere.normalized * distance;
               
        randomDirection += origin;
               
        NavMeshHit navHit;
               
        NavMesh.SamplePosition (randomDirection, out navHit, distance, layermask);
               
        return navHit.position;
    }

}
