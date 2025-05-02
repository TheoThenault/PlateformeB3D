using System.Diagnostics;
using UnityEngine;

public class MongoDBQuestion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MongoDBConnection.Init();
        MongoDBConnection.InsertMongoPartie(0, 0, 0, 0, 0, "");
        for (int i = 0; i < 10; i++)
        {
            MongoDBConnection.InsertMongoBateau(0,0,i,i,i,"");
        }
        for (int i = 0; i < 10; i++)
        {
            MongoDBConnection.InsertMongoJoueur(0, 0, "");
            for (int j = 0; j < 10; j++)
            {
                MongoDBConnection.InsertMongoDeplacement(0, 0, 0, j, j);
            }
            for (int j = 0; j < 10; j++)
            {
                MongoDBConnection.InsertMongoTir(0, 0, 0, j, j, j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
