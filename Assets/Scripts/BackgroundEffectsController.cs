using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundEffectsController : MonoBehaviour
{
    public GameObject discoBallRef;
    float discoBallSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        discoBallRef.transform.eulerAngles = new Vector3(discoBallRef.transform.eulerAngles.x, discoBallRef.transform.eulerAngles.y + discoBallSpeed*Time.deltaTime, discoBallRef.transform.eulerAngles.x);
    }
}
