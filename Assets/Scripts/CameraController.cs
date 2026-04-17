/// Contrôleur simplifié ; vous pouvez le supprimer ou le remplacer si vous voulez

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(1f, 10f)]
    public float dragSpeed = 5f;

    Vector3 lastMousePos;
    bool isDragging = false;


    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * dragSpeed * Time.deltaTime;
            transform.Translate(move, Space.Self);
            lastMousePos = Input.mousePosition;
        }
    }
}