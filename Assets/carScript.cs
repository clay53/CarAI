using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carScript : MonoBehaviour
{
    public static double forwardSpeed = 0.1;
    public static double rotationSpeed = 2;
    
    public bool manualInput = false;

    public double forward = 0;
    public double rotation = 0;

    public int frontSensors = 1;
    public int leftSensors = 2;
    public int bottomSensors = 1;
    public int rightSensors = 2;

    public int middleLayers = 1;
    public int middleNodes = 6;
    public double[][] weights;
    public double[][] nodes;

    public bool win = false;

    void Awake()
    {
        weights = new double[middleLayers+1][];
        for (int i = 0; i < weights.Length; i++) {
            weights[i] = new double[i == 0 ?
                (frontSensors+leftSensors+bottomSensors+rightSensors)*(weights.Length == 1 ? 2 : middleNodes) : (
                    i == weights.Length-1 ? middleNodes*2 :
                    middleNodes*middleNodes
                )
            ];
            for (int j = 0; j < weights[i].Length; j++) {
                weights[i][j] = Random.Range(-1.0f, 1.0f);
                //print(weights[i][j]);
            }
            //print("");
        }
        
        nodes = new double[middleLayers+2][];
        nodes[0] = new double[frontSensors+leftSensors+bottomSensors+rightSensors];
        nodes[nodes.Length-1] = new double[2];
        for (int i = 1; i < nodes.Length-1; i++) {
            nodes[i] = new double[middleNodes];
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Clear nodes
        for (int i = 0; i < nodes.Length; i++) {
            for (int j = 0; j < nodes[i].Length; j++) {
                nodes[i][j] = 0;
            }
        }

        // Get sensor information
        for (int i = 0; i < frontSensors+leftSensors+bottomSensors+rightSensors; i++) {
            string sensor = i < frontSensors ? "frontSensor" : (i < frontSensors+leftSensors ? "leftSensor" : (i < frontSensors+leftSensors+bottomSensors ? "bottomSensor" : "rightSensor"));
            int currentSensor = (
                (sensor == "frontSensor" ?
                    i : (
                        sensor == "leftSensor" ?
                        i-frontSensors : (
                            sensor == "bottomSensor" ?
                            i-frontSensors-leftSensors :
                            i-frontSensors-leftSensors-bottomSensors
                        )
                    )
                )+1
            );
            int sensorCount = sensor == "frontSensor" ? frontSensors : (sensor == "leftSensor" ? leftSensors : (sensor == "bottomSensor" ? bottomSensors : rightSensors));
            Vector3 pos = (
                transform.position+
                (sensor == "frontSensor" ?
                    transform.up*transform.localScale.y : (
                        sensor == "leftSensor" ?
                        -transform.right*transform.localScale.x : (
                            sensor == "bottomSensor" ?
                            -transform.up*transform.localScale.y :
                            transform.right*transform.localScale.x
                        )
                    )
                )/2+
                (sensor == "frontSensor" || sensor == "bottomSensor" ? transform.right : transform.up)*
                (sensor == "frontSensor" || sensor == "bottomSensor" ? -transform.localScale.x/2+(currentSensor)*(transform.localScale.x/(sensorCount+1)) : -transform.localScale.y/2+(currentSensor)*(transform.localScale.y/(sensorCount+1)))
            );
            Vector2 rotation = (
                (sensor == "frontSensor" ?
                    transform.up : (
                        sensor == "leftSensor" ?
                        -transform.right : (
                            sensor == "bottomSensor" ?
                            -transform.up :
                            transform.right
                        )
                    )
                )
            );
            nodes[0][i] = distanceToWall(pos, rotation);
            // print(distanceToWall(pos, rotation));
            // Instantiate(Resources.Load("rDot"), pos, new Quaternion());
            // Instantiate(Resources.Load("bDot"), pointToWall(pos, rotation), new Quaternion());
        }
        // string thing = "(";
        // for (int i = 0 ; i < nodes[0].Length; i++) {
        //     thing += nodes[0][i] + ", ";
        // }
        // print(thing.Substring(0, thing.Length-2) + ")");

        // Calculate nodes
        for (int i = 0; i < nodes.Length-1; i++) {
            for (int j = 0; j < nodes[i].Length; j++) {
                for (int k = 0; k < nodes[i+1].Length; k++) {
                    //print("going through weight: "  + (nodes[i].Length*k+j) + " in weights: " + i);
                    nodes[i+1][k] += nodes[i][j]*weights[i][nodes[i].Length*k+j];
                    //print("i: " + i + " j: " + j + " k: " + k + " changed to: " + nodes[i+1][k]);
                }
            }
        }

        //print(nodes[nodes.Length-1][0] + ", " + nodes[nodes.Length-1][1]);
        forward = Mathf.Clamp((float)nodes[nodes.Length-1][0], -1, 1);
        rotation = Mathf.Clamp((float)nodes[nodes.Length-1][1], -1, 1);

        if (!manualInput) {
            move(forward, rotation);
        } else {
            move(
                (Input.GetKey("w") ? 1 : (Input.GetKey("s") ? -1 : 0)),
                (Input.GetKey("a") ? 1 : (Input.GetKey("d") ? -1 : 0))
            );
        }
    }

    void move(double speed, double rotation) {
        transform.Rotate(0f, 0f, (float)(rotationSpeed*rotation*speed));
        transform.Translate(Vector2.up*(float)(forwardSpeed*speed));
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "wall") {
            Destroy(gameObject);
        } else if (col.gameObject.tag == "win") {
            win = true;
        }
    }

    Vector2 pointToWall(Vector2 position, Vector2 direction) {
        RaycastHit2D ray = Physics2D.Raycast(position, direction, Mathf.Infinity, 1 << 9);
        return ray.point;
    }

    double distanceToWall(Vector2 position, Vector2 direction) {
        RaycastHit2D ray = Physics2D.Raycast(position, direction, Mathf.Infinity, 1 << 9);
        if (ray.collider != null) {
            return (
                Mathf.Sqrt(
                    Mathf.Pow(position.x-ray.point.x, 2)+
                    Mathf.Pow(position.y-ray.point.y, 2)
                )
            );
        } else {
            return(0);
        }
    }
}
