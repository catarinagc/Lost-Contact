using UnityEngine;

public class ColliderFinder : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PrintColliderInfo(hit.collider);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PrintColliderInfo(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        PrintColliderInfo(other);
    }

    private void PrintColliderInfo(Collider col)
    {
        if (col == null) return;

        Debug.Log(
            "Player touched collider: " + col.name +
            "\nFull hierarchy: " + GetHierarchyPath(col.transform) +
            "\nLayer: " + LayerMask.LayerToName(col.gameObject.layer) +
            "\nTag: " + col.gameObject.tag +
            "\nIs Trigger: " + col.isTrigger,
            col.gameObject
        );
    }

    private string GetHierarchyPath(Transform obj)
    {
        string path = obj.name;

        while (obj.parent != null)
        {
            obj = obj.parent;
            path = obj.name + "/" + path;
        }

        return path;
    }
}
