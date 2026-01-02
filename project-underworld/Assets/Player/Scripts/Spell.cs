using UnityEngine;

public class Spell : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    private Rigidbody2D rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    public void Cast(float direction)
    {
        rigidBody.linearVelocityX = direction * speed;

        if (direction > 0)
        {
            transform.localScale = new Vector3(-6, 6, 6);
        }
    }
}
