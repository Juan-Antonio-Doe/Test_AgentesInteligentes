using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour {

    [SerializeField] private BoidUnit unitPrefab;

    [Header("Boid Settings")]
    [SerializeField] private int numberOfUnits = 20;

    [SerializeField] private float spawnRadius = 6f;
    public float SpawnRadius {
        get { return spawnRadius; }
    }

    [Header("Boid Distances")]
    [SerializeField, Range(0, 10f)] 
    private float cohesionDistance = 1f;
    public float CohesionDistance {
        get { return cohesionDistance; }
    }
    [SerializeField, Range(0, 10f)]
    private float aligmentDistance = 1f;
    public float AligmentDistance {
        get { return aligmentDistance; }
    }
    [SerializeField, Range(0, 10f)]
    private float avoidanceDistance = 1f;
    public float AvoidanceDistance {
        get { return avoidanceDistance; }
    }

    [Header("Boid Weights")]
    [SerializeField, Range(0, 10f)]
    private float cohesionWeight = 5f;
    public float CohesionWeight {
        get { return cohesionWeight; }
    }
    [SerializeField, Range(0, 10f)]
    private float aligmentWeight = 5f;
    public float AligmentWeight {
        get { return aligmentWeight; }
    }
    [SerializeField, Range(0, 10f)]
    private float avoidanceWeight = 5f;
    public float AvoidanceWeight {
        get { return avoidanceWeight; }
    }
    [SerializeField, Range(0, 10f)]
    private float insideBoundsWeight = 1f;
    public float InsideBoundsWeight {
        get { return insideBoundsWeight; }
    }

    [SerializeField] public List<BoidUnit> allUnits { get; private set; } = new List<BoidUnit>();

    IEnumerator Start() {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < numberOfUnits; i++) {
            Vector3 _spawnPos = Random.insideUnitSphere * spawnRadius;

            int _rotY = Random.Range(0, 361);

            BoidUnit _unit = Instantiate(unitPrefab, transform.position + _spawnPos, Quaternion.Euler(0, _rotY, 0));
            _unit.BoidManager = this;

            allUnits.Add(_unit);
        }
    }

    void Update() {
        for (int i = 0; i < allUnits.Count; i++) {
            allUnits[i].Movement();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
