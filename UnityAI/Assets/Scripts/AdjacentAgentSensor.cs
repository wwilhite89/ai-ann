using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class AdjacentAgentSensor : MonoBehaviour {
	public int range = 2;
    public Vector3 startDir = new Vector3(0, 1, 0);
	public GameObject trainingObject;
    private TrainingScript trainer;
    private IList<float> distance = new List<float>();
    private IList<float> relativeAngle = new List<float>();

    void Start()
    {
        distance.Add(0f);
        relativeAngle.Add(0f);
    }

    void FixedUpdate()
    {
        if (trainingObject != null && trainer == null)
            trainer = trainingObject.GetComponent<TrainingScript>();

        Array agents = getObjectsInRadius("Agent");
        ArrayList vectors = new ArrayList();
        float offsetRotation = gameObject.transform.rotation.eulerAngles.z;
        foreach (GameObject agent in agents)
        {
            Vector3 objDir = (agent.transform.position - gameObject.transform.position).normalized;
            objDir.z = 0;

            float angle = Vector3.Angle(startDir, objDir);

            if (Vector3.Cross(startDir, objDir).z < 0)
                angle = 180 + (180 - angle);

            float relativeAngle = (360.0f + angle - offsetRotation) % 360.0f;
            float distance = Vector3.Distance(gameObject.transform.position, agent.transform.position);
            Vector2 vector = new Vector2(distance, relativeAngle);
            vectors.Add(vector);
        }

        this.distance.Clear();
        this.relativeAngle.Clear();

        foreach (Vector2 vector in vectors)
        {
            this.distance.Add(vector.x);
            this.relativeAngle.Add(vector.y);

            if (this.trainer != null)
            {
                // Training for now only cares about the closest one
                trainer.setAgentDistance(vector.x);
                trainer.setAgentAngle(vector.y);
            }
        }
    }

    /*void OnGUI() {
        Array agents = getObjectsInRadius("Agent");
        ArrayList vectors = new ArrayList();
        float offsetRotation = gameObject.transform.rotation.eulerAngles.z;
        foreach(GameObject agent in agents) {
            Vector3 objDir = (agent.transform.position - gameObject.transform.position).normalized;
            objDir.z = 0;

            float angle = Vector3.Angle(startDir, objDir);

            if (Vector3.Cross(startDir, objDir).z < 0)
                angle = 180 + (180 - angle);

            float relativeAngle = (360.0f + angle - offsetRotation) % 360.0f;
            float distance = Vector3.Distance(gameObject.transform.position, agent.transform.position);
            Vector2 vector = new Vector2(distance, relativeAngle);
            vectors.Add(vector);
        }

        foreach(Vector2 vector in vectors) {
            GUI.Label (new Rect (10,60,150,20), "Agent " + vector);
            script.setAgentDistance(vector.x);
            script.setAgentAngle(vector.y);
        }
    }*/

    public IList<float> GetAgentDistances()
    {
        return this.distance;
    }

    public IList<float> GetAgentRelativeAngles()
    {
        return this.relativeAngle;
    }

    private GameObject[] getObjectsInRadius(string agentName) {
        Vector3 pos = gameObject.transform.position;

        GameObject[] agents = GameObject.FindGameObjectsWithTag(agentName)
            .Where(x => Mathf.Abs((x.transform.position - pos).magnitude) <= this.range)
            .ToArray();

        return agents;
    }
}
