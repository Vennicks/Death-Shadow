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
        transform.localRotation = (direction == 1) ? Quaternion.Euler(0, 0, 0) : transform.localRotation = Quaternion.Euler(0, 180, 0);

        transform.Translate(new Vector2(Speed * Time.deltaTime, 0f));
    }

    private IEnumerator invertDirection()
    {
        yield return new WaitForSeconds(Seconds);
        direction *= -1;
        StartCoroutine(invertDirection());
    }
}
