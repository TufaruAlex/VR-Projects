using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trebuchet : MonoBehaviour
{
    public Rigidbody weight;

    public GameObject ball;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            weight.isKinematic = false;
        }

        if (ball.transform.position.y >= 14.9)
        {
            HingeJoint hingeJoint;
            hingeJoint = ball.GetComponent<HingeJoint>();
            Destroy(hingeJoint);
        }
    }
}
