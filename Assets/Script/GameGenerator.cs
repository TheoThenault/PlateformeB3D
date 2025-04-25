using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class GameGenerator : MonoBehaviour
{
    [Range(4, 20)] public int playgroundSize = 4;
    [Range(0, 10)] public int nbShip1D = 5;
    [Range(0, 5)] public int nbShip2D = 2;
    [Range(0, 2)] public int nbShip3D = 1;
    [Range(2, 5)] public int nbPlayers = 2;
    public GameObject shipPrefab;
    public GameObject playgroundPrefab;

    public void GenerateGame()
    {
        InstanciatePlayground();
        InstanciateShips();
    }

    private void InstanciatePlayground()
    {
        GameManager.Instance.playgroundSize = playgroundSize;
        GameManager.Instance.playground = new PlaygroundData[playgroundSize, playgroundSize, playgroundSize];
  
        for (int i = 0; i < playgroundSize - 1; i++)
        {
            for (int j = 0; j < playgroundSize - 1; j++)
            {
                for (int k = 0; k < playgroundSize - 1; k++)
                {
                    GameManager.Instance.playground[i, j, k] = 0;
                }
            }
        }

        GameObject playground = Instantiate(playgroundPrefab);
        playground.transform.localScale = new Vector3(playgroundSize, playgroundSize, playgroundSize);
    }

    private void InstanciateShips()
    {
        for (int i = 0; i < nbShip1D; i++)
        {
            InstanciateShip1D();
        }

        for (int i = 0; i < nbShip2D; i++)
        {
            InstanciateShip2D();
        }

        for (int i = 0; i < nbShip3D; i++)
        {
            InstanciateShip3D();
        }
    }
    private void InstanciateShip1D()
    {
        GameObject ship = new GameObject("Ship1D");
        Ship1D newShip = ship.AddComponent<Ship1D>();

        newShip.size = Random.Range(1, playgroundSize);
        newShip.axe = (Axe)Random.Range(0, 2);

        switch (newShip.axe)
        {
            case Axe.i:
                newShip.position = new Vector3Int(
                    Random.Range(0, playgroundSize - newShip.size),
                    Random.Range(0, playgroundSize),
                    Random.Range(0, playgroundSize));
                break;
            case Axe.j:
                newShip.position = new Vector3Int(
                    Random.Range(0, playgroundSize),
                    Random.Range(0, playgroundSize - newShip.size),
                    Random.Range(0, playgroundSize));
                break;
            case Axe.k:
                newShip.position = new Vector3Int(
                    Random.Range(0, playgroundSize),
                    Random.Range(0, playgroundSize),
                    Random.Range(0, playgroundSize - newShip.size));
                break;
        }

        int nbBlocks = 0;

        for (int j = 0; j <= newShip.size; j++)
        {
            switch (newShip.axe)
            {
                case Axe.i:
                    nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(j, 0, 0), newShip);
                    break;
                case Axe.j:
                    nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(0, j, 0), newShip);
                    break;
                case Axe.k:
                    nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(0, 0, j), newShip);
                    break;
            }
        }

        if (nbBlocks < newShip.size)
        {
            Debug.Log("abort ship: " + nbBlocks + " blocks instead of " + newShip.size);
            DestroyImmediate(ship);
            InstanciateShip1D();
            return;
        }

        newShip.InstanciateChilds(shipPrefab);
        GameManager.Instance.ships1D.Add(newShip);
    }

    private void InstanciateShip2D()
    {
        GameObject ship = new GameObject("Ship2D");
        Ship2D newShip = ship.AddComponent<Ship2D>();

        newShip.size = Random.Range(1, playgroundSize);
        newShip.axe = (Axe)Random.Range(0, 2);

        switch (newShip.axe)
        {
            case Axe.i:
                newShip.position = new Vector3Int(
                    Random.Range(0, playgroundSize),
                    Random.Range(0, playgroundSize - newShip.size),
                    Random.Range(0, playgroundSize - newShip.size));
                break;
            case Axe.j:
                newShip.position = new Vector3Int(
                    Random.Range(0, playgroundSize - newShip.size),
                    Random.Range(0, playgroundSize),
                    Random.Range(0, playgroundSize - newShip.size));
                break;
            case Axe.k:
                newShip.position = new Vector3Int(
                    Random.Range(0, playgroundSize - newShip.size),
                    Random.Range(0, playgroundSize - newShip.size),
                    Random.Range(0, playgroundSize));
                break;
        }

        int nbBlocks = 0;

        for (int j = 0; j <= newShip.size; j++)
        {
            switch (newShip.axe)
            {
                case Axe.i:
                    for (int k = 0; k < j; k++)
                    {
                        nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(0, k, j), newShip);
                        nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(0, j, k), newShip);
                    }
                    nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(0, j, j), newShip);
                    break;
                case Axe.j:
                    for (int k = 0; k < j; k++)
                    {
                        nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(k, 0, j), newShip);
                        nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(j, 0, k), newShip);
                    }
                    nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(j, 0, j), newShip);
                    break;
                case Axe.k:
                    for (int k = 0; k < j; k++)
                    {
                        nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(k, j, 0), newShip);
                        nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(j, k, 0), newShip);
                    }
                    nbBlocks += SpawnShipBlock(newShip.position + new Vector3Int(j, j, 0), newShip);
                    break;
            }
        }

        if (nbBlocks < (newShip.size + 1) * (newShip.size + 1))
        {
            Debug.Log("abort ship: " + nbBlocks + " blocks instead of " + newShip.size * newShip.size);
            DestroyImmediate(ship);
            InstanciateShip2D();
            return;
        }

        newShip.InstanciateChilds(shipPrefab);
        GameManager.Instance.ships2D.Add(newShip);
    }

    private void InstanciateShip3D()
    {
        GameObject ship = new GameObject("Ship3D");
        Ship3D newShip = ship.AddComponent<Ship3D>();

        newShip.size = Random.Range(1, playgroundSize);
        newShip.axe = (Axe)Random.Range(0, 2);

        newShip.position = new Vector3Int(
            Random.Range(newShip.size / 2 + 1, playgroundSize - newShip.size / 2 - 1),
            Random.Range(newShip.size / 2 + 1, playgroundSize - newShip.size / 2 - 1),
            Random.Range(newShip.size / 2 + 1, playgroundSize - newShip.size / 2 - 1));

        for (int j = 0; j < newShip.size; j++)
        {
        }
    }
    
    private int SpawnShipBlock(Vector3Int position, Ship parentShip)
    {
        if (!parentShip.AddChild(position))
        {
            //TODO : regenerate ship if failed
            return 0;
        }
        return 1;
    }
}

[CustomEditor(typeof(GameGenerator))]
[CanEditMultipleObjects]
public class GameGeneratorEditor : Editor
{
    SerializedProperty playgroundSize;
    SerializedProperty nbShip1D;
    SerializedProperty nbShip2D;
    SerializedProperty nbShip3D;
    SerializedProperty nbPlayers;
    SerializedProperty shipPrefab;
    SerializedProperty playgroundPrefab;

    GameGenerator game;

    void OnEnable()
    {
        playgroundSize = serializedObject.FindProperty("playgroundSize");
        nbShip1D = serializedObject.FindProperty("nbShip1D");
        nbShip2D = serializedObject.FindProperty("nbShip2D");
        nbShip3D = serializedObject.FindProperty("nbShip3D");
        nbPlayers = serializedObject.FindProperty("nbPlayers");
        shipPrefab = serializedObject.FindProperty("shipPrefab");
        playgroundPrefab = serializedObject.FindProperty("playgroundPrefab");
    }

    public override void OnInspectorGUI()
    {
        GameGenerator game = target as GameGenerator;

        serializedObject.Update();

        EditorGUILayout.PropertyField(playgroundSize);
        EditorGUILayout.PropertyField(nbShip1D);
        EditorGUILayout.PropertyField(nbShip2D);
        EditorGUILayout.PropertyField(nbShip3D);
        EditorGUILayout.PropertyField(nbPlayers);
        EditorGUILayout.PropertyField(shipPrefab);
        EditorGUILayout.PropertyField(playgroundPrefab);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate")) {
            game.GenerateGame();
        }
    }
}
