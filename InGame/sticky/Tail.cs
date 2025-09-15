using UnityEngine;

public class Tail : MonoBehaviour
{
    public int length;
    public LineRenderer lineRdr;
    public Vector3[] points;
    private Vector3[] pointVelocity;

    public Transform targetDir;
    public float targetDist;
    public float smoothSpeed;
    public float trailSpeed;

    public float wiggleSpeed;
    public float wiggleMagnitude;
    public Transform wiggleDir;
    private int randInt;
    void Start()
    {
        lineRdr.positionCount = length;
        points = new Vector3[length];
        pointVelocity = new Vector3[length];
        randInt = Random.Range(0, 1000);
    }

    void Update()
    {
        wiggleDir.localRotation = Quaternion.Euler(0, 0, Mathf.Sin((Time.time + randInt) * wiggleSpeed) * wiggleMagnitude);

        points[0] = targetDir.position;

        for(int i = 1; i < points.Length; ++i)
        {
            points[i] = Vector3.SmoothDamp(points[i], points[i - 1] + targetDir.right * targetDist, ref pointVelocity[i], smoothSpeed + i/trailSpeed);
        }
        lineRdr.SetPositions(points);
    }
}
