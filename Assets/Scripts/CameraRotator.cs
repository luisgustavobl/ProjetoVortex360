using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float sensibilidade = 3.0f;
    private float rotX = 0.0f;
    private float rotY = 0.0f;

    [Header("Configuração de Zoom")]
    public Camera cam;
    public float minFov = 30f;
    public float maxFov = 70f;
    public float sensibilidadeZoom = 10f;

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        // Rotação Pan (Girar Visão)
        if (Input.GetMouseButton(0))
        {
            rotY += Input.GetAxis("Mouse X") * sensibilidade;
            rotX -= Input.GetAxis("Mouse Y") * sensibilidade;
            rotX = Mathf.Clamp(rotX, -80f, 80f);

            transform.rotation = Quaternion.Euler(rotX, rotY, 0.0f);
        }

        // Zoom com o Scroll do Mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            cam.fieldOfView -= scroll * sensibilidadeZoom;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFov, maxFov);
        }
    }
}