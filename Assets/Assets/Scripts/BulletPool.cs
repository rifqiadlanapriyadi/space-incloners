using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool instance;

    public GameObject friendBulletPrefab;
    public GameObject enemyBulletPrefab;
    public int initialCount = 5;

    private Queue<GameObject> _friendBulletQueue;
    private Queue<GameObject> _enemyBulletQueue;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        _friendBulletQueue = new Queue<GameObject>();
        _enemyBulletQueue = new Queue<GameObject>();
        for (int i = 0; i < initialCount; i++)
        {
            GameObject friendBullet = Instantiate(friendBulletPrefab);
            friendBullet.SetActive(false);
            friendBullet.transform.SetParent(transform);
            _friendBulletQueue.Enqueue(friendBullet);

            GameObject enemyBullet = Instantiate(enemyBulletPrefab);
            enemyBullet.SetActive(false);
            enemyBullet.transform.SetParent(transform);
            _enemyBulletQueue.Enqueue(enemyBullet);
        }
    }

    public GameObject GetNextFire(Team team)
    {
        Queue<GameObject> queue = team == Team.Friend ? _friendBulletQueue : _enemyBulletQueue;
        GameObject result;
        if (queue.Count > 0) result = queue.Dequeue();
        else
        {
            result = Instantiate(team == Team.Friend ? friendBulletPrefab : enemyBulletPrefab);
            result.transform.SetParent(transform);
        }
        result.SetActive(true);
        return result;
    }

    public void Enqueue(GameObject go)
    {
        Fireball fireball = go.GetComponent<Fireball>();
        if (fireball != null && go.activeSelf)
        {
            go.SetActive(false);
            if (fireball.originTeam == Team.Friend)
                _friendBulletQueue.Enqueue(go);
            else _enemyBulletQueue.Enqueue(go);
        }
    }
}
