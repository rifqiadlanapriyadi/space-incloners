using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    public static EnemiesController instance;

    public GameObject enemyPrefab;
    public int firstEnemyColumnCount = 2;
    public int firstEnemyRowCount = 2;
    public int levelEnemyRowColRamp = 1;
    public float height;
    public float width;
    public float initialSpeed = 1f;
    public float speedRamp = 1f;
    public float descendIncrements = 1f;

    private int _currEnemyColumnCount;
    private int _currEnemyRowCount;

    public float shootIntervalSeconds = 1f;
    public float shootIntervalAdditionalRange = 1f;

    private float _nextShootTime;
    private System.Random _rand = new System.Random();
    private LinkedList<GameObject> _enemyLinkedList;
    private Dictionary<GameObject, LinkedListNode<GameObject>> _enemyLinkedListNodes;

    private float _leftBounds;
    private float _rightBounds;
    private float _leftRel;
    private float _rightRel;
    private float _halfEnemyWidth;

    private Dictionary<GameObject, int> _enemyColumns;
    private int[] _remainingInColumn;
    private float[] _columnRels;
    private int _leftMostCol;
    private int _rightMostCol;

    private float xVel;
    private float persistXVel;

    private GameManager _gm;

    private void Awake()
    {
        if (instance == null) instance = this;
        _gm = GameManager.instance;
        ResetFirstLevelSettings();
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        SetNextShootTime();
        SetupEnemyPositions();
    }

    public void ResetFirstLevelSettings()
    {
        _currEnemyColumnCount = firstEnemyColumnCount;
        _currEnemyRowCount = firstEnemyRowCount;
    }

    public void SetupNextLevelSettings()
    {
        _currEnemyColumnCount += levelEnemyRowColRamp;
        _currEnemyRowCount += levelEnemyRowColRamp;
    }

    private void SetupEnemyPositions()
    {
        _leftBounds = transform.position.x - width / 2;
        _rightBounds = transform.position.x + width / 2;
        float widthDiff = width / (_currEnemyColumnCount + 1);
        float heightDiff = height / (_currEnemyRowCount + 1);
        float firstX = _leftBounds + widthDiff;
        float currX = firstX;
        float currY = transform.position.y - height / 2 + heightDiff;
        float maxX = _rightBounds;
        float maxY = transform.position.y + height / 2;

        _enemyLinkedList = new LinkedList<GameObject>();
        _enemyLinkedListNodes = new Dictionary<GameObject, LinkedListNode<GameObject>>();
        _enemyColumns = new Dictionary<GameObject, int>();
        _remainingInColumn = new int[_currEnemyColumnCount];
        _columnRels = new float[_currEnemyRowCount];
        _leftMostCol = 0;
        _rightMostCol = _currEnemyColumnCount - 1;

        while (currY < maxY)
        {
            int column = 0;
            while (currX < maxX)
            {
                GameObject enemy = Instantiate(enemyPrefab, new Vector2(currX, currY), Quaternion.identity);
                enemy.transform.SetParent(transform, true);

                LinkedListNode<GameObject> node = _enemyLinkedList.AddLast(enemy);
                _enemyLinkedListNodes.Add(enemy, node);
                _enemyColumns.Add(enemy, column);
                _remainingInColumn[column]++;
                column++;

                currX += widthDiff;
            }
            currX = firstX;
            currY += heightDiff;
        }

        GameObject tempEnemy = Instantiate(enemyPrefab);
        _halfEnemyWidth = tempEnemy.GetComponent<PolygonCollider2D>().bounds.size.x / 2;
        Destroy(tempEnemy);

        float currRel = -(width / 2) + widthDiff;
        for (int i = 0; i < _currEnemyColumnCount; i++)
        {
            _columnRels[i] = currRel;
            currRel += widthDiff;
        }

        _leftRel = _columnRels[0] - _halfEnemyWidth;
        _rightRel = _columnRels[_currEnemyColumnCount - 1] + _halfEnemyWidth;
        UpdateXVel(initialSpeed);
    }

    void Update()
    {
        if (_gm.IsSuspended) xVel = 0;
        else xVel = persistXVel;

        transform.position = transform.position + Vector3.right * xVel * Time.deltaTime;

        if (Time.time >= _nextShootTime && !_gm.IsSuspended)
        {
            RandomEnemyShoot();
            SetNextShootTime();
        }
    }

    private void SetNextShootTime()
    {
        _nextShootTime = Time.time + shootIntervalSeconds + (float)(_rand.NextDouble() * shootIntervalAdditionalRange);
    }

    private void RandomEnemyShoot()
    {
        GameObject randomEnemy = _enemyLinkedList.ElementAt(_rand.Next(_enemyLinkedList.Count));
        if (randomEnemy != null)
        {
            EnemyController randomEnemyController = randomEnemy.GetComponent<EnemyController>();
            randomEnemyController.Shoot();
        }
    }

    private void UpdateXVel(float newXVel)
    {
        xVel = newXVel;
        persistXVel = newXVel;
    }

    public void HandleDestroyed(GameObject enemyObject)
    {
        if (_enemyColumns.ContainsKey(enemyObject)) {
            _gm.HandleEnemyKilled(1);
            int enemyColumn = _enemyColumns[enemyObject];
            int colCount = _remainingInColumn[enemyColumn] - 1;
            _remainingInColumn[enemyColumn] = colCount;
            if (colCount <= 0)
            {
                if (enemyColumn == _leftMostCol)
                {
                    if (!UpdateMostCol(ref _leftMostCol, _leftMostCol + 1, _currEnemyColumnCount, 1, _halfEnemyWidth))
                    {
                        _gm.HandleAllEnemiesKilled();
                        return;
                    }
                }
                else if (enemyColumn == _rightMostCol)
                {
                    if (!UpdateMostCol(ref _rightMostCol, _rightMostCol - 1, -1, -1, _halfEnemyWidth))
                    {
                        _gm.HandleAllEnemiesKilled();
                        return;
                    }
                }
            }
            _enemyColumns.Remove(enemyObject);
            _enemyLinkedList.Remove(_enemyLinkedListNodes[enemyObject]);
            _enemyLinkedListNodes.Remove(enemyObject);

            // Increase speed of enemies
            UpdateXVel(xVel < 0 ? xVel - speedRamp : xVel + speedRamp);
        }
    }

    private bool UpdateMostCol(ref int mostCol, int start, int end, int direction, float adjustment)
    {
        for (int i = start; i != end; i += direction)
        {
            if (_remainingInColumn[i] > 0)
            {
                mostCol = i;
                if (direction > 0) _leftRel = _columnRels[i] - adjustment;
                else _rightRel = _columnRels[i] + adjustment;
                return true;
            }
        }
        return false;
    }

    void LateUpdate()
    {
        if (transform.position.x + _rightRel >= _rightBounds)
        {
            transform.position = new Vector2(_rightBounds - _rightRel, transform.position.y - descendIncrements);
            UpdateXVel(-xVel);
        }
        if (transform.position.x + _leftRel <= _leftBounds)
        {
            transform.position = new Vector2(_leftBounds - _leftRel, transform.position.y - descendIncrements);
            UpdateXVel(-xVel);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 currPos = transform.position;
        Vector2 halfWidth = new Vector2(width / 2, 0);
        Vector2 halfHeight = new Vector2(0, height / 2);

        Gizmos.color = Color.red;
        // Bottom line
        Gizmos.DrawLine(currPos - halfWidth - halfHeight, currPos + halfWidth - halfHeight);
        // Right line
        Gizmos.DrawLine(currPos + halfWidth - halfHeight, currPos + halfWidth + halfHeight);
        // Top line
        Gizmos.DrawLine(currPos + halfWidth + halfHeight, currPos - halfWidth + halfHeight);
        // Bottom line
        Gizmos.DrawLine(currPos - halfWidth + halfHeight, currPos - halfWidth - halfHeight);
    }
}
