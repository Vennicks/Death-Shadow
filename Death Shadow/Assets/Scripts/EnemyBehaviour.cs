using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] public float Seconds = 5;

    [SerializeField] public float Speed = 10f;

    private int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(invertDirection());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(new Vector2(Speed * Time.deltaTime * direction, 0f));
    }

    private IEnumerator invertDirection()
    {
        yield return new WaitForSeconds(Seconds);
        direction *= -1;
        StartCoroutine(invertDirection());
    }
}
