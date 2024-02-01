using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingTarget : MonoBehaviour {

    [field: Header("Target properties")]
    [field: SerializeField] private float radius { get; set; } = 3f;
    public float Radius { get { return radius; } }
    [field: SerializeField] private List<Transform> enemies = new List<Transform>();

    private float pi2 { get; set; } = 2 * Mathf.PI;

    void Start() {
        //CalculatePositionAround();
    }

    public Vector3 CalculatePositionAround(Transform enemy, bool maintainDistance = false) {
        Vector3 _pos = Vector3.zero;

        if (enemies.Count < 2) {
            if (maintainDistance) {
                _pos = enemy.position - transform.position;
                _pos.y = enemy.position.y;
                return transform.position + _pos.normalized * radius;
            }
            else {
                return Vector3Int.one * 9999;
            }
        }

        float _rads = (pi2 / enemies.Count) * enemies.IndexOf(enemy);
        _rads *= radius;
        float _x = Mathf.Cos(_rads);
        float _z = Mathf.Sin(_rads);
        _pos = new Vector3(_x, 0f, _z);

        return transform.position + _pos;
    }

    public Vector3 CalculateRandomPositionAround(Transform enemy) {
        Vector3 dirToEnemy = enemy.position - transform.position;
        dirToEnemy.y = enemy.position.y;

        float angle = Vector3.Angle(transform.forward, dirToEnemy);

        int[] angleAmounts = new int[] { 3, -3, 4, -4, 5, -5, 6, -6 };

        angle += angleAmounts[Random.Range(0, angleAmounts.Length)];

        float _rads = angle * Mathf.Deg2Rad;
        float _x = Mathf.Cos(_rads);
        float _z = Mathf.Sin(_rads);

        return transform.position + new Vector3(_x, 0f, _z) * radius;
    }

    public void AddToEnemyList(Transform _enemy) {
        if (!enemies.Contains(_enemy))
            enemies.Add(_enemy);
    }

    public void RemoveFromEnemyList(Transform _enemy) {
        if (enemies.Contains(_enemy))
            enemies.Remove(_enemy);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
