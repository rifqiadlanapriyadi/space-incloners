using UnityEngine;

public class RandomBulletRain : MonoBehaviour
{
    public float minPeriod = 0.5f;
    public float periodRandomRange = 0.5f;

    private float _nextFriendFireTime;
    private float _nextEnemyFireTime;
    private System.Random _rand = new System.Random();

    public GameObject left;
    public GameObject right;
    public GameObject top;
    public GameObject bottom;

    private float _left;
    private float _right;
    private float _top;
    private float _bottom;

    private BulletPool _bp;

    private void Start()
    {
        _bp = BulletPool.instance;
        _nextFriendFireTime = Time.time + GetRandomInterval();
        _nextEnemyFireTime = Time.time + GetRandomInterval();

        _left = left.transform.position.x;
        _right = right.transform.position.x;
        _top= top.transform.position.y;
        _bottom = bottom.transform.position.y;
    }

    private void Update()
    {
        HandleFire(ref _nextFriendFireTime, Team.Friend, _bottom);
        HandleFire(ref _nextEnemyFireTime, Team.Enemy, _top);
        //if (Time.time >= _nextFriendFireTime)
        //{
        //    _nextFriendFireTime = Time.time + minPeriod + GetRandomInterval();
        //    GameObject fireball = BulletPool.instance.GetNextFire(Team.Friend);
        //    fireball.transform.position = new Vector2(GetRandomXPos(), _bottom);
        //}
        //if (Time.time >= _nextEnemyFireTime)
        //{
        //    _nextEnemyFireTime = Time.time + minPeriod + GetRandomInterval();
        //    GameObject fireball = BulletPool.instance.GetNextFire(Team.Enemy);
        //    fireball.transform.position = new Vector2(GetRandomXPos(), _top);
        //}
    }

    private void HandleFire(ref float time, Team team, float fromY) {
        if (Time.time >= time)
        {
            time = Time.time + minPeriod + GetRandomInterval();
            GameObject fireball = BulletPool.instance.GetNextFire(team);
            fireball.transform.position = new Vector2(GetRandomXPos(), fromY);
        }
    }

    private float GetRandomInterval()
    {
        return (float)(_rand.NextDouble() * periodRandomRange);
    }

    private float GetRandomXPos()
    {
        float length = Mathf.Abs(_right - _left);
        float relXPos = (float)(_rand.NextDouble() * length);
        return Mathf.Min(_right, _left) + relXPos;
    }
}
