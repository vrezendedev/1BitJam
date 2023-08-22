using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerInteractivity : MonoBehaviour
{
    public float PullForce;
    [HideInInspector] public string LookingAt = "";
    public GameObject dir;

    void Update()
    {
        InteractWith();
    }

    private void InteractWith()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5, 1 << 6))
            if (hit.distance < 2)
            {
                LookingAt = hit.transform.gameObject.tag;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    switch (hit.transform.gameObject.tag)
                    {
                        case "Pointer":
                            EventManager.PlayerRotateLight.Invoke();
                            break;
                        case "Lever":
                            Debug.Log("Hit!");
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                LookingAt = "";
            }
        else
            LookingAt = "";

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ball")
        {
            other.rigidbody.AddForce(this.transform.forward + this.transform.up * PullForce, ForceMode.Impulse);
            other.rigidbody.AddTorque(-this.transform.forward, ForceMode.Impulse);
        }

    }
}
