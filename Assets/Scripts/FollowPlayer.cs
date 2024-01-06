using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    Vector3 gravityUp;
    public Transform objectToFollow;
    void Start()
    {
        
    }

    void Update()
    {
        gravityUp = (transform.position - objectToFollow.GetComponent<PlayerController>().currentPlanet.position).normalized;
    }

    // Update is called once per frame
    void LateUpdate()
    {
       transform.position = objectToFollow.position + objectToFollow.transform.up.normalized;

       transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
    }
}
