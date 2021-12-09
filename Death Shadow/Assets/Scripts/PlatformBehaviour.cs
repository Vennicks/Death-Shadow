using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    [SerializeField] public float LifeTime = 3;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, LifeTime);
        Destroy(this, LifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DestroyComponent()
    {
        yield return new WaitForSeconds(LifeTime);
    }
}
