using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIManagerScript : MonoBehaviour
{
    public GameObject car;
    public GameObject currentCar;
    public carScript currentCarScript;
    public double[][] currentWeights;
    public string currentWeightsS;
    public double[][] bestWeights;
    public InputField bestWeightsS;

    public int maxUpdates = 1000;
    public int currentUpdates = 0;

    public int iterations = 0;

    public double trainSpeed = 10;

    public bool training = true;
    public bool visualizing = false;
    public visualizerScript visualizer;

    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Time.timeScale = Time.timeScale != 0 ? (float)trainSpeed : 0;
        if (training) {
            if (currentCar == null) {
                iterations++;
                currentUpdates = 0;
                currentCar = (GameObject)Instantiate(car, new Vector2(-10, 0), (Quaternion)Quaternion.Euler(0, 0, -90));
                currentCarScript = currentCar.GetComponent<carScript>();
                double[][] carWeights = currentCarScript.weights;
                currentWeights = new double[carWeights.Length][];
                bestWeights = new double[carWeights.Length][];
                System.Array.Copy(carWeights, currentWeights, carWeights.Length);
                currentWeightsS = weightsString(currentWeights);
                print(currentWeightsS);
            } else {
                if (currentCarScript.win) {
                    bestWeightsS.text = currentWeightsS;
                    print(gameObject.name + " has had it's car complete the course.");
                    Time.timeScale = 0;
                } else {
                    currentUpdates++;
                    if (currentUpdates > maxUpdates || (currentUpdates > maxUpdates/5 && Vector3.Distance(new Vector3(-10, 0, 0), currentCar.transform.position) < 1)) {
                        Destroy(currentCar);
                    }
                }
            }
        } else {
            if (currentCar == null) {
                currentCar = (GameObject)Instantiate(car, new Vector2(-10, 0), (Quaternion)Quaternion.Euler(0, 0, -90));
                currentCarScript = currentCar.GetComponent<carScript>();
                currentCarScript.weights = stringToWeights(bestWeightsS.text);
                
            } else if (currentCarScript.win) {
                print(gameObject.name + " has had it's car complete the course.");
                Time.timeScale = 0;
            } else if (visualizing) {
                visualizer.setWeights(currentCarScript.nodes, currentCarScript.weights);
            }
        }
    }

    string weightsString(double[][] weights) {
        string output = "";
        output += "[\n";
        for(int i = 0; i < weights.Length; i++) {
            output += "\t[\n";
            for(int j = 0; j < weights[i].Length; j++) {
                output += ("\t\t" + weights[i][j] + (j != weights[i].Length-1 ? "," : "") + "\n");
            }
            output += ("\t]" + (i != weights.Length-1 ? "," : "") + "\n");
        }
        output += "]";
        return output;
    }

    double[][] stringToWeights(string s) {
        s = s.Replace("\t", "").Replace(" ", "");
        s = s.Substring(1, s.Length-2);
        List<string> w = new List<string>(s.Split('['));
        w.RemoveAt(0);
        double[][] weights = new double[w.Count][];
        for(int i = 0; i < w.Count; i++) {
            w[i] = w[i].Substring(0, w[i].Length-1);
            print(w[i]);
            string[] ww = w[i].Split(',');
            weights[i] = new double[ww.Length];
            for(int j = 0; j < ww.Length; j++) {
                print(ww[j]);
                weights[i][j] = double.Parse(ww[j].Replace("]", ""));
            }
        }
        print(weightsString(weights));
        return weights;
    }
}
