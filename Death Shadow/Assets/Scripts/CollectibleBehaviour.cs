using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class CollectibleBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public float Speed = 10f;

    private GameObject targetToFollow;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetToFollow != null)
        {
            Vector3 destination = targetToFollow.transform.position - transform.position;
            transform.Translate(destination.normalized * Speed * Time.deltaTime);
        }
    }

    public void DefineTarget(GameObject target)
    {
        targetToFollow = target;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject obj = collider.gameObject;
        if (obj.gameObject.tag == TAGS.PLAYER)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
    }
}