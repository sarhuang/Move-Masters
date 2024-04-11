using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using Unity.VisualScripting;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite defaultImage;
    public Sprite pressedImage;
    public KeyCode keyToPress;

   
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode key = GameManager.GetKeyVal();
        //CHANGE THIS LINE FOR THE BUTTON
        if(Input.GetKeyDown(keyToPress)){
        //if(key == keyToPress){
            Debug.Log(key + " pressed down");
            spriteRenderer.sprite = pressedImage;
        }    
        //CHANGE THIS LINE FOR THE BUTTON
        if(Input.GetKeyUp(keyToPress)){
        //else{
        //if(key != keyToPress){
            Debug.Log(key + " key off");
            spriteRenderer.sprite = defaultImage;
        }
    }
}
