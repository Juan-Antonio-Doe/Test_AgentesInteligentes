using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaApi : MonoBehaviour {

    private Actions actions;
    private Sensors sensors;

    [SerializeField] private float stepTime = 0.5f;
    [SerializeField] private Transform originPosition;
    private Vector2 originPos = new Vector2(0, 0);

    [SerializeField, Tooltip("Casillas visitadas")] private List<Vector2> visitedCells;
    [SerializeField, Tooltip("Casillas registradas (durante recorrido)")] private List<Vector2> mappedCells;
    [SerializeField, Tooltip("Casillas bloqueadas por obstáculos")] private List<Vector2> blockedCells;

    [SerializeField] private GameObject marker;
	
    void Start() {
        actions = GetComponent<Actions>();
        sensors = GetComponent<Sensors>();

        visitedCells = new List<Vector2>();
        mappedCells = new List<Vector2>();
        blockedCells = new List<Vector2>();

        if (originPosition != null) {
            originPos = GetRoundedPos(originPosition.position);
        }

        InvokeRepeating("DecisionSystem", stepTime, stepTime);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            CancelInvoke();
            OrientateToHome();
        }
    }

    // Toda la lógica y comprobaciones que tiene que realizar
    void DecisionSystem() {
        Vector2 _currentPos = GetRoundedPos(transform.position);
        Vector2 _frontPos = _currentPos + GetRoundedPos(transform.forward);


        if (sensors.FrontSensor()) {
            actions.RotateRight();

            // Añade la casilla frontal a la lista de casillas bloqueadas
            if (!blockedCells.Contains(_frontPos)) {
                blockedCells.Add(_frontPos);
            }
        }
        else {

            // Mapea todas las casillas adyacentes a la actual y las almacena en la lista de casillas mapeadas
            Vector2[] _adjacentCells = GetAdjacentCells(_currentPos);

            if (!visitedCells.Contains(_currentPos)) {  // Si no ha sido visitada, la marcamos como visitada

                visitedCells.Add(_currentPos);  // Guardamos la casilla por la que ha pasado.

                // Recorremos todas las casillas adyacentes y las añadimos a la lista de casillas mapeadas
                for (int i = 0; i < _adjacentCells.Length; i++) {
                    if (Mathf.Abs(_adjacentCells[i].x) == 5 || Mathf.Abs(_adjacentCells[i].y) == 5) { // Si la casilla está fuera del mapa, la ignoramos.
                        if (!blockedCells.Contains(_adjacentCells[i])) // Si no está en la lista de casillas bloqueadas, la añadimos
                            blockedCells.Add(_adjacentCells[i]);
                        continue;
                    }
                    if (!mappedCells.Contains(_adjacentCells[i])) { // Solo mapea si la casilla no ha sido mapeada ya
                        mappedCells.Add(_adjacentCells[i]);
                    }
                }

                Instantiate(marker, transform.position, marker.transform.rotation);  // Instanciamos un marcador en la casilla visitada
            }

            if (visitedCells.Count == mappedCells.Count) { // Si ya ha visitado todas las casillas, para.
                Debug.Log("Ya he visitado todas las casillas");
                CancelInvoke();

                OrientateToHome();

                InvokeRepeating("MoveToHome", stepTime, stepTime);  // Vuelve a casa
            }

            // Tiene que comprobar si ya ha visitado la casilla frontal
            if (!visitedCells.Contains(_frontPos)) {
                actions.MoveForward();
            }
            else { // Si la casilla frontal ya ha sido visitada...
                for (int i = 0; i < _adjacentCells.Length; i++) { // Comprobar si alguna de las casillas adyacentes está sin visitar
                    if (!visitedCells.Contains(_adjacentCells[i]) && !blockedCells.Contains(_adjacentCells[i])) {    // En cuanto una casilla adyacente no esté visitada, gira hacia la derecha
                        actions.RotateRight();
                        return;  // Deja de comprobar el resto de casillas adyacentes
                    }
                }
                actions.MoveForward(); // Si todas las casillas adyacentes y la frontal están visitadas, avanza.
            }
        }
    }

    private Vector2[] GetAdjacentCells(Vector2 _roundPos) {
        Vector2[] _adjacent = new Vector2[4];
        //Vector2 _roundPos = GetRoundedPos(_originPos);

        /*_adjacent[0] = new Vector2(_roundPos.x + 1, _roundPos.y);
        _adjacent[1] = new Vector2(_roundPos.x - 1, _roundPos.y);
        _adjacent[2] = new Vector2(_roundPos.x, _roundPos.y + 1);
        _adjacent[3] = new Vector2(_roundPos.x, _roundPos.y - 1);*/

        // Array de desplazamientos para simplificar la asignación
        Vector2[] offsets = { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };

        for (int i = 0; i < 4; i++) {
            _adjacent[i] = _roundPos + offsets[i];
        }

        //Debug.Log($"Posición adyacente: {_adjacent}");

        return _adjacent;
    }

    // Para redondear cualquier posicion y pasarla a Vector2
    private Vector2 GetRoundedPos(Vector3 _positionToRound) {
        Vector2 _roundedPos = new Vector2(Mathf.RoundToInt(_positionToRound.x), Mathf.RoundToInt(_positionToRound.z));
        //Debug.Log($"Posición a redondear: {_positionToRound} to {aux}");
        return _roundedPos;
    }

    private void OrientateToHome() {
        //Debug.Log($"Rotación default: {transform.rotation.y} Rounded: {Mathf.RoundToInt(transform.rotation.y)}");
        // Para poder orientarse correctamente tiene que comprobar la posición del punto de inicio según la Rotación de la Rumba en el eje Y
        switch (Mathf.RoundToInt(transform.eulerAngles.y)) {
            case 0: // X > derecha. X < izquierda. Z > adelante. Z < atras.
                if (originPos.x > transform.position.x) {   // A la derecha.
                    actions.RotateRight();
                }
                else if (originPos.x < transform.position.x) {  // A la izquierda.
                    actions.RotateLeft();
                }
                else if (originPos.y < transform.position.z) {  // Giro de 180º
                    actions.RotateFull();
                }
                break;

            case 90: // X > adelante. X < atras. Z < derecha. Z > izquierda.
                if (originPos.y < transform.position.z) {   // A la derecha.
                    actions.RotateRight();
                }
                else if (originPos.y > transform.position.z) {  // A la izquierda.
                    actions.RotateLeft();
                }
                else if (originPos.x < transform.position.x) {  // Giro de 180º
                    actions.RotateFull();
                }
                break;

            case 180: // X < derecha. X > izquierda. Z < adelante. Z > atras.
                if (originPos.x < transform.position.x) {   // A la derecha.
                    actions.RotateRight();
                }
                else if (originPos.x > transform.position.x) {  // A la izquierda.
                    actions.RotateLeft();
                }
                else if (originPos.y > transform.position.z) {  // Giro de 180º
                    actions.RotateFull();
                }
                break;

            case 270: case -90: // X < adelante. X > atras. Z > derecha. Z < izquierda.
                if (originPos.y > transform.position.z) {   // A la derecha.
                    actions.RotateRight();
                }
                else if (originPos.y < transform.position.z) {  // A la izquierda.
                    actions.RotateLeft();
                }
                else if (originPos.x > transform.position.x) {  // Giro de 180º
                    actions.RotateFull();
                }
                break;
        }
    }

    void MoveToHome() {
        Vector2 _currPos = GetRoundedPos(transform.position);

        if (_currPos == originPos) {
            Debug.Log("Ya he llegado a casa");
            CancelInvoke();
            return;
        }

        switch (Mathf.RoundToInt(transform.eulerAngles.y)) {

            // Alineamiento en Z (al estar transformado, eje Y)
            case 0: case 180:
                if (originPos.y == _currPos.y) { // Si está alineado en Z, avanza
                    OrientateToHome();
                }
                else {  // Mientras no este alineado, se mueve hacia adelante
                    actions.MoveForward();
                }
                break;

            // Alineamiento en X
            case 90: case -90: case 270:
                if (originPos.x == _currPos.x) { // Si está alineado en X, avanza
                    OrientateToHome();
                }
                else {  // Mientras no este alineado, se mueve hacia adelante
                    actions.MoveForward();
                }
                break;
        }

    }
}
