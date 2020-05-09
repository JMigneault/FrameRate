using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    public string[] messages;
    public string[] switchMessages;

    public float[] xcoords;
    public float[] ycoords;

    // whether y coord should check > or <
    public bool[] ygt;
    private int index = 0;

    public Text text;

    private bool switched = false;

    void Update() {
        if (index < xcoords.Length) {
            if (transform.position.x > xcoords[index] && ((ygt[index] && ycoords[index] < transform.position.y) || (!ygt[index] && ycoords[index] >= transform.position.y))) {
                index++;
            }
        }
        if (switched) {
            text.text = switchMessages[index];
        } else {
            text.text = messages[index];
        }
    }

    public void Switch() {
        switched = !switched;
        if (switched) {
            text.text = switchMessages[index];
        } else {
            text.text = messages[index];
        }

    }
    

}
