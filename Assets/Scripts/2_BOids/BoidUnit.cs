using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidUnit : MonoBehaviour {

    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float visionAngle = 270f;
    [SerializeField] private float smoothVectorTime = 1f;

    [SerializeField] private BoidManager boidManager;
    public BoidManager BoidManager {
        get { return boidManager; }
        set { boidManager = value; }
    }

    private Vector3 emptyVelocity;

    public void Movement() {

        Vector3 _movement = CalculateBoidVector() + GetInsideBoundsVector();

        _movement = Vector3.Normalize(_movement);

        transform.forward = Vector3.SmoothDamp(transform.forward, _movement, ref emptyVelocity, smoothVectorTime);

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    Vector3 CalculateBoidVector() {

        Vector3 _cohesionVector = Vector3.zero;
        int _cohesionNeighbours = 0;

        // Alineamiento
        /**
         * Para alinear correctamente, el valor por defecto tiene que ser el transform.forward para que lo mantenga en caso
         * de no tener vecinos a los que alinearse.
         */
        Vector3 _alignmentVector = transform.forward;
        int _alignmentNeighbours = 0;

        // Separación
        Vector3 _avoidanceVector = Vector3.zero;
        int _avoidanceNeighbours = 0;

        // Media aritmética de las posiciones de los vecinos
        for (int i = 0; i < boidManager.allUnits.Count; i++) {
            if (boidManager.allUnits[i] != this) {
                if (IsInVisionRange(boidManager.allUnits[i].transform.position)) {
                    Vector3 _direction = transform.position - boidManager.allUnits[i].transform.position;   // Calculamos la dirección hacia cada otro vecino

                    // Si el vecino está dentro del rango de cohesión
                    if (_direction.sqrMagnitude <= boidManager.CohesionDistance * boidManager.CohesionDistance) {  
                        _cohesionVector += boidManager.allUnits[i].transform.position;  // Se almacena la posición de todos los vecinos
                        _cohesionNeighbours++;
                    }

                    // Si el vecino está dentro del rango de alineamiento
                    if (_direction.sqrMagnitude <= boidManager.AligmentDistance * boidManager.AligmentDistance) {
                        _alignmentVector += boidManager.allUnits[i].transform.forward;  // Se almacena la dirección de todos los vecinos
                        _alignmentNeighbours++;
                    }

                    // Si el vecino está dentro del rango de separación
                    if (_direction.sqrMagnitude <= boidManager.AvoidanceDistance * boidManager.AvoidanceDistance) {
                        // Se almacena la dirección en la que se encuentra cada vecino respecto a este boid
                        _avoidanceVector += _direction;
                        _avoidanceNeighbours++;
                    }
                }
            }
        }

        // Calculamos el vector de cohesión haciendo la media.
        if (_cohesionNeighbours > 0) {
            _cohesionVector /= _cohesionNeighbours;
            _cohesionVector -= transform.position;
            _cohesionVector = Vector3.Normalize(_cohesionVector) * boidManager.CohesionWeight;
        }

        // Calculamos el vector de alineamiento haciendo la media.
        if (_alignmentNeighbours > 0) {
            _alignmentVector /= _alignmentNeighbours;
            _alignmentVector = Vector3.Normalize(_alignmentVector) * boidManager.AligmentWeight;
        }

        // Calculamos el vector de separación haciendo la media.
        if (_avoidanceNeighbours > 0) {
            _avoidanceVector /= _avoidanceNeighbours;
            //_avoidanceVector *= -1;
            _avoidanceVector = Vector3.Normalize(_avoidanceVector) * boidManager.AvoidanceWeight;
        }

        return _cohesionVector + _alignmentVector + _avoidanceVector;
    }



    bool IsInVisionRange(Vector3 _position) {
        Vector3 _direction = _position - transform.position;
        float _angle = Vector3.Angle(transform.forward, _direction);

        return _angle <= visionAngle;
    }

    Vector3 GetInsideBoundsVector() {
        Vector3 _dirToBoid = boidManager.transform.position - transform.position;

        // Si el boid está dentro del radio de spawn, no se aplica ninguna fuerza
        if (_dirToBoid.sqrMagnitude <= boidManager.SpawnRadius * boidManager.SpawnRadius) {
            return Vector3.zero;
        }
        else {  // Si el boid está fuera del radio de spawn, se aplica una fuerza hacia el centro
            return _dirToBoid.normalized * boidManager.InsideBoundsWeight;
        }
    }

}
