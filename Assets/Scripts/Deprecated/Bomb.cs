using PickMaster.Game.View;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject blowFX;
    public float radius = 5.0F;
    public float power = 10.0F;

    private ConveyorBelt cb;

    // Start is called before the first frame update
    private void Start()
    {
        cb = FindObjectOfType<ConveyorBelt>();
        cb.ResetSpeed();
    }

    public void Blow()
    {
        print("Blowing up!");
        var newBlowFX = Instantiate(blowFX, transform.position, Quaternion.identity);
        Destroy(newBlowFX, 2);

        var explosionPos = transform.position;
        var colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Debug.Log("Collided with " + hit.gameObject.name);
            var rb = hit.gameObject.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
        }

        Destroy(gameObject);
    }
}
