using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShipBlock : MonoBehaviour
{
    public Vector3Int position;
    public Ship ship;
    public Material materialTouched;
    public Material materialDestroyed;
    public Renderer renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        renderer = GetComponent<Renderer>();
    }

    private void OnCollisionStay(Collision collision)
    {
        GameManager.Instance.playground[position.x, position.y, position.z] = PlaygroundData.ShipTouched;
        SetToTouched();

        int touchedBlocks = 0;

        foreach (KeyValuePair<Vector3Int, ShipBlock> child in ship.childs)
        {
            if (GameManager.Instance.playground[child.Key.x, child.Key.y, child.Key.z] == PlaygroundData.ShipTouched)
                touchedBlocks++;
        }

        if (touchedBlocks == ship.childs.Count)
        {
            foreach (KeyValuePair<Vector3Int, ShipBlock> child in ship.childs)
            {
                GameManager.Instance.playground[child.Key.x, child.Key.y, child.Key.z] = PlaygroundData.ShipDestroyed;
                child.Value.SetToDestroyed();
            }
        }
    }

    public void SetToTouched()
    {
        GetComponent<Collider>().isTrigger = true;
        renderer.material = materialTouched;
    }

    public void SetToDestroyed()
    {
        renderer.material = materialDestroyed;
    }
}
