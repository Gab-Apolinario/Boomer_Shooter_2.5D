using UnityEngine;

public class BillboardBehavior : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Apenas Y — sprite fica ereto mesmo quando câmera olha pra cima/baixo
        Vector3 direction = cam.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}