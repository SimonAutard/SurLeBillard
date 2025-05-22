using UnityEngine;
using System;
public class Controller : MonoBehaviour
{
    public static event Action<Vector3> OnMouseClicked;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 clickPosition = ray.GetPoint(distance);
                OnMouseClicked?.Invoke(clickPosition); // Émet l'événement
            }
        }
    }
}

