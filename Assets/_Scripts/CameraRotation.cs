using UnityEngine;

public class CameraRotation : MonoBehaviour
{

    [SerializeField] private float _rotateSpeed = 0f;

    private float minYAngle = -30.0f;
    private float maxYAngle = 30.0f;

    private float yRotate = 0.0f;
    private float xRotate = 0.0f;

    void Awake()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        yRotate += Input.GetAxis("Mouse Y");
        yRotate = Mathf.Clamp(yRotate, minYAngle, maxYAngle);

        xRotate += Input.GetAxis("Mouse X");
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(-yRotate * _rotateSpeed, xRotate * _rotateSpeed, 0), 0.5f);
    }
}
