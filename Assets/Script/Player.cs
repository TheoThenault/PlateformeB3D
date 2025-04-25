using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            gameObject.transform.position += Vector3.forward;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            gameObject.transform.position += Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            gameObject.transform.position += Vector3.back;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            gameObject.transform.position += Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            gameObject.transform.position += Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            gameObject.transform.position += Vector3.down;
        }
    }
}
