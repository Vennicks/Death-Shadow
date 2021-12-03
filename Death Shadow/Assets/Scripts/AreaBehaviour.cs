using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBehaviour : MonoBehaviour
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
        GameObject obj = collider.gameObject;
        if (obj.gameObject.tag == TAGS.SOUL)
        {
            obj.gameObject.GetComponent<CollectibleBehaviour>().DefineTarget(transform.parent.gameObject);
        }
    }
}
