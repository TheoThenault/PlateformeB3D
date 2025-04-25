using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartieManager : MonoBehaviour
{
    public GameObject button;
    public GameObject parent;

    void Start()
    {
        List<int> Parties = SQLManager.PartieList();
        for (int i = 0; i < Parties.Count; i++)
        {
            GameObject go = Instantiate(button);
            (go.transform.Find("ID").GetComponent<TMP_Text>()).text = "" + Parties[i];
            go.transform.SetParent(parent.transform);
        }
    }
}