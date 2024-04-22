using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundEffectsController : MonoBehaviour
{
    public GameObject discoBallRef;
    public Light spotLight1;
    public Light spotLight2;
    public List<Color> spotLight1Colors;
    public List<Color> spotLight2Colors;
    float discoBallSpeed = 5f;
    float gameTime = 0;

    // Update is called once per frame
    void Update()
    {
        discoBallRef.transform.eulerAngles = new Vector3(discoBallRef.transform.eulerAngles.x, discoBallRef.transform.eulerAngles.y + discoBallSpeed*Time.deltaTime, discoBallRef.transform.eulerAngles.x);

        gameTime += Time.deltaTime;
        if (gameTime > 5) {
            UpdateLights();
            gameTime = 0;
        }
    }

    public void UpdateLights() {
        int colorIndex = Random.Range(0, spotLight1Colors.Count);

        spotLight1.color = spotLight1Colors[colorIndex];
        spotLight2.color = spotLight2Colors[colorIndex%spotLight2Colors.Count];
    }
}
