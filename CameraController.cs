using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float MOVEMENT_BASE_SPEED, movementSpeed;
    public Vector3 movementDirection;
    void Update()
    {
        if (GetComponent<Camera>().enabled == true)
        {
            movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            transform.position += movementDirection * (Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f)) * MOVEMENT_BASE_SPEED;
        }
    }
}
