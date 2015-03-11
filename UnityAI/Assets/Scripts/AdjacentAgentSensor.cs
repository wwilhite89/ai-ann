using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class AdjacentAgentSensor : MonoBehaviour {
	public int range = 2;
    public Vector3 startDir = new Vector3(0, 1, 0);

	void OnGUI() {
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
		int y = 10;
		foreach(Vector2 vector in vectors) {
			GUI.Label (new Rect (10,70,150,20), "Agent " + vector);
			y += 15;
		}
	}

    private GameObject[] getObjectsInRadius(string agentName) {
        Vector3 pos = gameObject.transform.position;

        GameObject[] agents = GameObject.FindGameObjectsWithTag(agentName)
            .Where(x => Mathf.Abs((x.transform.position - pos).magnitude) <= this.range)
            .ToArray();

        return agents;
    }
}
