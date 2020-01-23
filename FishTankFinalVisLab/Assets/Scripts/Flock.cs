using UnityEngine;

public class Flock : MonoBehaviour
{
    public float speed = 0.001f;
    //public float rotationSpeed = 4.0f;
    //Vector3 averageHeading;
    //Vector3 averagePosition;
    float neighborDistance = GlobalFlock.tankSize;

    bool turning = false;

    // Use this for initialization
    void Start()
    {
        speed = Random.Range(0.5f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //ApplyTankBoundary();

        //if (Vector3.Distance(transform.position, Vector3.zero) >= GlobalFlock.tankSize)
        if(System.Math.Abs(transform.position.x) >= GlobalFlock.tankSize || transform.position.y <= GlobalFlock.TankFloorY
            || transform.position.y >= GlobalFlock.TankTopY || System.Math.Abs(transform.position.z) >= GlobalFlock.tankSize)
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction),
                rotationSpeed() * Time.deltaTime);
            speed = Random.Range(0.5f, 1);
        }
        else
        {
            if (Random.Range(0, 5) < 1)
                ApplyRules();
        }
        
        //************* X-axis is forward direction *************
        transform.Translate(Time.deltaTime * speed, 0, 0);
    }

    /*void ApplyTankBoundary()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) >= GlobalFlock.tankSize)
        {
            turning = true;
        }
        else
        {
            turning = false;
        }
    }*/

    void ApplyRules()
    {
        int numSmallfish = GlobalFlock.smallFish.Length;
        int numBigfish = GlobalFlock.bigFish.Length;
        GameObject[] gos = new GameObject[numSmallfish + numBigfish];
        GlobalFlock.smallFish.CopyTo(gos, 0);
        GlobalFlock.bigFish.CopyTo(gos, numSmallfish);

        Vector3 vCenter = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = GlobalFlock.goalPos;

        float dist;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighborDistance)
                {
                    vCenter += go.transform.position;
                    groupSize++;

                    if (dist < 0.75f)
                    {
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFish = go.GetComponent<Flock>();
                    gSpeed += anotherFish.speed;
                }

            }
        }

        if (groupSize > 0)
        {
            vCenter = vCenter / groupSize + (goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            Vector3 direction = (vCenter + vAvoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction),
                    rotationSpeed() * Time.deltaTime);
            }
        }

    }

    float rotationSpeed()
    {
        return Random.Range(1, 5);
    }
}
