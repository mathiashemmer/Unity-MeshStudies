using UnityEngine;

public class JetController : MonoBehaviour
{
    public float MaxForce = 10f;
    public float MaxForceDistance = 5f;
    public float GroundOffset = 2.5f;
    public float DisplacementGainFactor = 0.2f;
    public float LinearFactor = 1f;


    Vector3 lastKnownPosition;
    float CurrentForce;

    private void Start()
    {
        lastKnownPosition = transform.position;
    }

    public void Tick()
    {
        lastKnownPosition = transform.position;

        var distanceToGround = GetDistanceToGround();

    }

    float GetDistanceToGround()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, MaxForceDistance))
        {
            return Vector3.Distance(transform.position, hitInfo.point);
        }
        return -1f;
    }

}

public class DroneController : MonoBehaviour
{
    public Transform[] jetPosition = new Transform[4];
    public Vector3[] lastJetPosition = new Vector3[4];

    public Rigidbody body;
    public float Force = 1.2f;
    public float maxForceDisance = 3f;

    public bool EnableJets = false;

    private void Start()
    {
        for(int i = 0; i < jetPosition.Length; i++)
        {
            lastJetPosition[i] = jetPosition[i].position;
        }
    }

    public void FixedUpdate()
    {
        var forceDirection = -transform.up;
        if (!EnableJets) return;
        for(int i = 0; i < jetPosition.Length; i++)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(jetPosition[i].position, forceDirection, out hitInfo, maxForceDisance))
            {
                var target = hitInfo.point;
                var distance = Mathf.Abs(Vector3.Distance(jetPosition[i].position, target));

                var actualForce = Mathf.Lerp(Force/2, Force, Mathf.InverseLerp(maxForceDisance, 0, distance));
                body.AddForceAtPosition(-forceDirection * actualForce * Time.deltaTime, jetPosition[i].position);
            }

            lastJetPosition[i] = jetPosition[i].position;
        }

    }
    private void OnDrawGizmos()
    {
        var forceDirection = -transform.up;
        for (int i = 0; i < jetPosition.Length; i++)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(jetPosition[i].position, forceDirection, out hitInfo, 999f))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(jetPosition[i].position, hitInfo.point);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(jetPosition[i].position, jetPosition[i].position + transform.up);
            }
        }
    }
}
