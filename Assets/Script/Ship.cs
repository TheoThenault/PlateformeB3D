using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum Axe
{
    i, j, k
}

[ExecuteInEditMode]
public class Ship : MonoBehaviour
{
    public int size;
    public Vector3Int position;
    public Axe axe;

    private Dictionary<Vector3Int, GameObject> childs = new();

    private void Start()
    {
        childs = new();
    }
    public bool AddChild(Vector3Int position)
    {
        if (GameManager.Instance.playground[position.x, position.y, position.z] != PlaygroundData.Empty)
        {
            Clear();
            return false;
        }

        childs.Add(position, null);
        GameManager.Instance.playground[position.x, position.y, position.z] = PlaygroundData.Ship;

        return true;
    }
    public void Clear()
    {
        foreach(KeyValuePair<Vector3Int, GameObject> child in childs)
        {
            DestroyImmediate(child.Value);
            GameManager.Instance.playground[child.Key.x, child.Key.y, child.Key.z] = PlaygroundData.Empty;
        }

        childs.Clear();
    }

    public void InstanciateChilds(GameObject blockPrefab)
    {
        foreach (KeyValuePair<Vector3Int, GameObject> child in childs)
        {
            if (!child.Value)
            {
                GameObject block = Instantiate(blockPrefab, transform);
                block.transform.position = child.Key - new Vector3Int(GameManager.Instance.playgroundSize / 2, 
                    GameManager.Instance.playgroundSize / 2, 
                    GameManager.Instance.playgroundSize / 2);
            }
        }
    }
}
