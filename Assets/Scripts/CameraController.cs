using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    Vector2[] lastPositions;
    int index;
    public BoxController boxController;

    void Start() {
        lastPositions = new Vector2[5];
        for (int i = 0; i < 5; i++) {
            lastPositions[i] = new Vector2(boxController.transform.position.x, boxController.transform.position.y);
        }
        index = 0;
    }

    void LateUpdate() {
        Vector2 boxPosition = Vector2.zero;
        foreach (Vector2 lp in lastPositions) {
            boxPosition += lp;
        }
        boxPosition /= 5;
        Vector2 cameraPosition = new Vector2(transform.position.x, transform.position.y);
        float cameraSpeed = (boxPosition - cameraPosition).magnitude;
        Vector2 newPosition = cameraPosition + (boxPosition - cameraPosition).normalized * cameraSpeed;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        lastPositions[index] = new Vector2(boxController.transform.position.x, boxController.transform.position.y);
        index = (index + 1) % 5;
    }


}
