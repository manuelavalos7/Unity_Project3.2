using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class torchScript : MonoBehaviour
{

    [SerializeField] Sprite litSprite;//sprite when light is on



    public void toggleLight() {
       
        GetComponent<SpriteRenderer>().sprite = litSprite;// change sprite
        GetComponent<Animator>().SetBool("lit", true);//set animator to start animation
        GetComponentInChildren<Light2D>().intensity = 0.75f;//set intensity to less than player's intensity

    }
}
