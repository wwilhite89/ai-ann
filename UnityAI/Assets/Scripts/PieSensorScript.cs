using UnityEngine;
using System.Collections;
using System.Linq;

public class PieSensorScript : MonoBehaviour
{

    public float range = 3F;
    public Vector3 startDir = new Vector3(0, 1, 0);
    private int[] actLevels = new int[4];
	public GameObject trainingObject;

    enum Slice
    {
        TOP_RIGHT = 0,
        TOP_LEFT = 1,
        BOTTOM_RIGHT = 2,
        BOTTOM_LEFT = 3
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        drawPieSlices();
        updateActivationLevels();
    }

    private void drawPieSlices() {
        Debug.DrawRay(transform.position, this.transform.up * range, Color.yellow);
        Debug.DrawRay(transform.position, -this.transform.up * range, Color.yellow);
        Debug.DrawRay(transform.position, this.transform.right * range, Color.yellow);
        Debug.DrawRay(transform.position, -this.transform.right * range, Color.yellow);
    }

    private void updateActivationLevels() {
        int[] oldLevels = new int[4];
        actLevels.CopyTo(oldLevels, 0);

        actLevels[0] = getActivationLevel(Slice.TOP_RIGHT);
        actLevels[1] = getActivationLevel(Slice.TOP_LEFT);
        actLevels[2] = getActivationLevel(Slice.BOTTOM_RIGHT);
        actLevels[3] = getActivationLevel(Slice.BOTTOM_LEFT);

        if (oldLevels [0] != actLevels [0] ||
			oldLevels [1] != actLevels [1] ||
			oldLevels [2] != actLevels [2] ||
			oldLevels [3] != actLevels [3]) {
			Debug.Log (string.Format ("Activation Levels: {0},{1},{2},{3}", 
			                        actLevels [0], actLevels [1], actLevels [2], actLevels [3]));
			// trainingObject.GetComponent<TrainingScript> ().actLevels = actLevels;
		}
    }

    /// <summary>
    /// Returns the number of agents in an activation level given a slice.
    /// </summary>
    /// <param name="slice">Slice area relative to the main agent</param>
    /// <returns>Number activation levels</returns>
    private int getActivationLevel(Slice slice)
    {
        int levels = 0;
        float offsetRotation = transform.rotation.eulerAngles.z;


        foreach (var agent in this.getObjectsInRadius("Agent"))
        {
            Vector3 objDir = (agent.transform.position - gameObject.transform.position).normalized;
            objDir.z = 0;

            float angle = Vector3.Angle(startDir, objDir);

            if (Vector3.Cross(startDir, objDir).z < 0)
                angle = 180 + (180 - angle);

            float relativeAngle = (360.0f + angle - offsetRotation) % 360.0f;

            switch (slice)
            {
                case Slice.TOP_LEFT:
                    levels += relativeAngle >= 0.0f && relativeAngle < 90.0f ? 1 : 0;
                    break;
                case Slice.BOTTOM_LEFT:
                    levels += relativeAngle >= 90.0f && relativeAngle < 180.0f ? 1 : 0;
                    break;
                case Slice.BOTTOM_RIGHT:
                    levels += relativeAngle >= 180.0f && relativeAngle < 270.0f ? 1 : 0;
                    break;
                case Slice.TOP_RIGHT:
                    levels += relativeAngle > 270.0f ? 1 : 0;
                    break;
                default:
                    break;
            }
        }

        return levels;
    }

    private GameObject[] getObjectsInRadius(string agentName)
    {
        Vector3 pos = gameObject.transform.position;

        var agents = GameObject.FindGameObjectsWithTag(agentName)
            .Where(x => Mathf.Abs((x.transform.position - pos).magnitude) <= this.range)
            .ToArray();

        return agents;
    }

}