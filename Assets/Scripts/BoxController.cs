using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{

    Rigidbody2D rigidBody;

    public float maxSpeed;
    public float deccelerationRate;
    public float accelerationRate;

    private Vector2 velocity = Vector2.zero;
    public Vector2 Velocity {get {return velocity; }}

    private BoxCollider2D[] walls;

    public GlitchController glitchController;

    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();

        GameObject[] vwalls = GameObject.FindGameObjectsWithTag("VertWall");
        GameObject[] hwalls = GameObject.FindGameObjectsWithTag("HorzWall");

        walls = new BoxCollider2D[vwalls.Length + hwalls.Length];

        for (int i = 0; i < walls.Length; i++) {
            if (i < vwalls.Length) {
                walls[i] = vwalls[i].GetComponent<BoxCollider2D>();
            } else {
                walls[i] = hwalls[i - vwalls.Length].GetComponent<BoxCollider2D>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3) velocity * Time.deltaTime;
        if (transform.position.x > 150) {
            StartCoroutine(glitchController.End());
            return;
        }
        if (transform.position.x > 80) {
            return;
        }
        Vector2 inDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // Debug.LogFormat("Input in direction: {0}", inDir);
        // Debug.LogFormat("Angle {0}", Vector2.Angle(velocity, inDir));
        if (velocity.magnitude > 0 && (Vector2.Angle(velocity, inDir) >= 100 || (Mathf.Abs(inDir.x) < .5 && Mathf.Abs(inDir.y) < .5))) {
            //Debug.Log("Decceleration Case");

            if (velocity.magnitude < .2) {
                velocity = Vector2.zero;
            }
            velocity = velocity.normalized * Mathf.Max(0, velocity.magnitude - deccelerationRate * Time.deltaTime);
        } else {
            // Debug.Log("Acceleration Case");
            Vector2 direction = (velocity.normalized + inDir.normalized * Time.deltaTime).normalized;
            // Debug.LogFormat("Velo direction: {0}", direction);
            velocity = direction * Mathf.Min(maxSpeed, velocity.magnitude + accelerationRate * Time.deltaTime);
            if (velocity.magnitude == maxSpeed) {
                Debug.Log("Achieved max speed");
            }
            // Debug.LogFormat("New velocity: {0}", velocity);
        }


        foreach (BoxCollider2D wall in walls) {
            float eps = .001f;
            if (wall.tag == "HorzWall") {
                float bottom = transform.position.y - GetComponent<BoxCollider2D>().size.y / 2;
                float top = transform.position.y + GetComponent<BoxCollider2D>().size.y / 2;
                if (transform.position.x > wall.transform.position.x - wall.size.x / 2
                    && transform.position.x < wall.transform.position.x + wall.size.x / 2
                    && ((bottom > wall.transform.position.y - wall.size.y / 2 
                    && bottom < wall.transform.position.y + wall.size.y / 2)
                    || (top > wall.transform.position.y - wall.size.y / 2 
                    && top < wall.transform.position.y + wall.size.y / 2))) {
                    float direction;
                    if ((transform.position - wall.transform.position).y >= 0) {
                        direction = 1;
                    } else {
                        direction = -1;
                    }
                    if (Vector2.Angle(velocity, new Vector2(0, direction)) > 90) {
                        Debug.Log("resetting velocity");
                        velocity = Vector2.zero;
                    }
                    Debug.Log(direction);
                    float y = wall.transform.position.y + direction * (eps + wall.GetComponent<BoxCollider2D>().size.y / 2 + GetComponent<BoxCollider2D>().size.y / 2);
                    transform.position = new Vector2(transform.position.x, y);
                }
            }
            if (wall.tag == "VertWall") {
                float bottom = transform.position.x - GetComponent<BoxCollider2D>().size.x / 2;
                float top = transform.position.x + GetComponent<BoxCollider2D>().size.x / 2;
                if (transform.position.y > wall.transform.position.y - wall.size.y / 2
                    && transform.position.y < wall.transform.position.y + wall.size.y / 2
                    && ((bottom > wall.transform.position.x - wall.size.x / 2 
                    && bottom < wall.transform.position.x + wall.size.x / 2)
                    || (top > wall.transform.position.x - wall.size.x / 2 
                    && top < wall.transform.position.x + wall.size.x / 2))) {
                    float direction;
                    if ((transform.position - wall.transform.position).x >= 0) {
                        direction = 1;
                    } else {
                        direction = -1;
                    }
                    if (Vector2.Angle(velocity, new Vector2(direction, 0)) > 90) {
                        Debug.Log("resetting velocity");
                        velocity = Vector2.zero;
                    }
                    float x = wall.transform.position.x + direction * (eps + wall.GetComponent<BoxCollider2D>().size.x / 2 + GetComponent<BoxCollider2D>().size.x / 2);
                    transform.position = new Vector2(x, transform.position.y);                
                }
            }
        }

    // Idea: player changes color based on velocity

    }


}
