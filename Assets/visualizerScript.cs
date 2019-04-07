using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class visualizerScript : MonoBehaviour
{
    public float xSpacing = 3;
    public float ySpacing = -1.6f;

    public void setWeights(double[][] nodes, double[][] weights) {
        // Remove children
        for(int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < nodes.Length; i++) {
            double[] layer = nodes[i];
            float x = i*xSpacing;
            for (int j = 0; j < layer.Length; j++) {
                float y = j*ySpacing;

                if (i < layer.Length-1) {
                    for(int k = 0; k < nodes[i+1].Length; k++) {
                        double value = weights[i][i*layer.Length+k];
                        GameObject weight = (GameObject)Instantiate(
                            Resources.Load("Weight"),
                            transform.position+new Vector3(x+xSpacing/2, (y+ySpacing*k)/2, 0),
                            new Quaternion()
                        );
                        weight.transform.SetParent(transform);
                        weight.transform.localScale = new Vector3(Vector3.Distance(new Vector3(0, y, 0), new Vector3(xSpacing, ySpacing*k, 0)), 0.3f*(float)value, 1);
                        weight.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(value < 0 ? 1 : 1-Mathf.Abs((float)value), value > 0 ? 1 : 1-Mathf.Abs((float)value), 1-Mathf.Abs((float)value));
                        Vector3 r = new Vector3(0, y, 0)-new Vector3(xSpacing, ySpacing*k, 0);
                        weight.transform.Rotate(0, 0, Mathf.Atan2(r.y, r.x)*180f/Mathf.PI);
                    }
                }

                GameObject node = (GameObject)Instantiate(Resources.Load("Node"), transform.position+new Vector3(x, y, 0), new Quaternion());
                node.transform.SetParent(transform);
                node.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color((float)(1*(1-layer[j])), 1, (float)(1*(1-layer[j])));
                node.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = string.Format("{0:00.00}", layer[j]);
            }
        }
    }
}
