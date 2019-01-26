using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HistoryRoom : HistoryItem
{
    private Collider[] obstacles = new Collider[0];
    private NavMeshObstacle[] navObstacles;
    
    private void Awake()
    {
        var his = GetComponentsInChildren<HistoryItem>();
        foreach (var hi in his)
        {
            hi.startTimeline = startTimeline;
            hi.endTimeline = endTimeline;
        }

        obstacles = GetComponentsInChildren<Collider>();
        navObstacles = new NavMeshObstacle[obstacles.Length];
        for (var i = 0; i < obstacles.Length; i++)
        {
            var obstacle = obstacles[i];
            var navObstacle = obstacle.gameObject.AddComponent<NavMeshObstacle>();
            navObstacle.shape = NavMeshObstacleShape.Box;
            navObstacle.size = obstacle.bounds.size;
            navObstacles[i] = navObstacle;
        }
    }

    public void CallUpdate(float timeline, Cat cat)
    {
        var opacity = TheGame.Instance.CurrentFade(startTimeline, endTimeline);

        for (var i = 0; i < obstacles.Length; i++)
        {
            var touching = false;
            var obstacle = obstacles[i];
            if (cat.touchingColliders.Contains(obstacle))
            {
                touching = true;
            }

            navObstacles[i].enabled = !touching && (opacity > 0.2f);
            //if (near.magnitude < 1f)
            {
                //Debug.Log(obstacle.name + " " + touching);
            }
        }
    }
}
