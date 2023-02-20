using UnityEngine;

public class OnDestroySlime : MonoBehaviour
{

    public bool canJump = true;
    public void Destruir()
    {
        Destroy(this.gameObject);
    }

    private float generateRandomJumpNumber(float lvl)
    {
        double min = 220 + (lvl * 0.5);
        double max = 250 + (lvl * 0.5);
        return Random.Range((float)min, (float)max);
    }

    public void Jump(float lvl)
    {
        Rigidbody rb_slime = GetComponent<Rigidbody>();
        //Apply a force to this Rigidbody in direction of this GameObjects up axis
        rb_slime.AddForce(transform.up * generateRandomJumpNumber(lvl));
    }


    public void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "Terrain")
        {
            canJump = true;
        }
        
    }
    
}
