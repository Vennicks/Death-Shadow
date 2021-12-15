using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("GROUNDED");
        if (collider.gameObject.tag == TAGS.PLATFORM)
        {
            Debug.Log("GROUNDED");
            gameObject.GetComponentInParent<PlayerBehaviour>().checkIfGrounded(true);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == TAGS.PLATFORM)
        {
            gameObject.GetComponentInParent<PlayerBehaviour>().checkIfGrounded(false);
        }
    }

}
