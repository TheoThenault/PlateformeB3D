using MongoDB.Driver;
using MongoDB.Bson;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System;
using static UnityEngine.ParticleSystem;
using UnityEngine.UIElements;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;
using UnityEngine.InputSystem;
using System.Collections;
using System.Data;
using static UnityEditor.Rendering.FilterWindow;

public static class MongoDBConnection
{
    private static IMongoCollection<BsonDocument> collection;
    private static BsonDocument model = new BsonDocument {
        { "Id_Partie" , "" },
        { "Taille" , "" },
        { "Debut" ,"" },
        { "TempsEcoule" , "" },
        { "DureeMax" , "" },
        { "Jeu", "" },
        { "Joueur", new BsonArray
            { new BsonDocument
                {
                    {"Id_Joueur", "" },
                    {"Pseudo", "" },
                    {"Deplacement", new BsonArray
                        { new BsonDocument
                            {
                                { "InGameTime", "" },
                                { "Position", "" },
                            }
                        }
                    },
                    {"Tir", new BsonArray
                        { new BsonDocument
                            {
                                { "Direction", "" },
                                { "InGameTime", "" },
                                { "Position", "" },
                            }
                        }
                    }
                }
            }
        },
        {"Bateau", new BsonArray
            { new BsonDocument
                {
                    { "Id_Bateau", "" },
                    { "Position", "" },
                    { "Taille", "" },
                    { "Type", "" },
                    { "Axe", "" },
                }
            }
        }
    };
    //private  Object = new Object

    public static void Init()
    {
        string connectionString = "mongodb://AdminLJV:!!DBLjv1858**@81.1.20.23:27017";
        MongoClient client = new MongoClient(connectionString);
        IMongoDatabase database = client.GetDatabase("USSI71_2025");
        collection = database.GetCollection<BsonDocument>("HAISSOUS_THENAULT_FOUCHE_Plateforme");
    }

    private static void InsertMongoData(BsonDocument document)
    {
        collection.InsertOne(document);
    }
    private static void UpdateMongoData(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
    {
        collection.UpdateOne(filter, update);
    }
    public static List<BsonDocument> SelectMongoData()
    {
        return collection.Find(new BsonDocument()).ToList();
    }

    private static void SetMongoRequestData(BsonDocument document)
    {
        if (!document.Contains("_id"))
        {
            InsertMongoData(document);
        }
        else
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", document.GetValue("_id"));
            UpdateDefinition<BsonDocument> update = null;
            foreach (BsonElement element in document)
            {
                update = update?.Set(element.Name, element.Value) ?? Builders<BsonDocument>.Update.Set(element.Name, element.Value);
            }
            if (update != null)
            {
                UpdateMongoData(filter, update);
            }
        }
    }

    private static BsonDocument ElementExist(string key, int value, BsonArray array)
    {
        foreach (BsonDocument document in array)
        {
            if (document.Contains(key))
            {
                BsonValue id = document.GetValue(key);
                if (id.IsInt32)
                {
                    if (id.AsInt32 == value)
                    {
                        return document;
                    }
                }
            }
        }
        return null;
    }
    private static Queue<T> CreateQueue<T>(T[] tab)
    {
        Queue<T> queue = new Queue<T>();
        foreach (T element in tab)
        {
            queue.Enqueue(element);
        }
        return queue;
    }

    public static void InsertMongoPartie(int Id_Partie, int Taille, int Debut, int TempsEcoule, int DureeMax, string Jeu)
    {
        BsonDocument PartieData = new BsonDocument {
            { "Id_Partie", Id_Partie },
            { "Taille", Taille },
            { "Debut", Debut },
            { "TempsEcoule", TempsEcoule },
            { "DureeMax", DureeMax },
            { "Jeu", Jeu },
        };
        SetMongoRequestData(PartieData);
    }

    public static void InsertMongoBateau(int Id_Partie, int Id_Bateau, int Position, int Taille, int Type, string Axe)
    {
        BsonDocument BateauData = new BsonDocument {
            { "Id_Bateau", Id_Bateau },
            { "Position", Position },
            { "Taille", Taille },
            { "Type", Type },
            { "Axe", Axe },
        };
        ModifyMongoDataFirstDepth(Id_Partie, "Bateau", "Id_Bateau", Id_Bateau, BateauData);
    }

    public static void InsertMongoJoueur(int Id_Partie, int Id_Joueur, string Pseudo)
    {
        BsonDocument JoueurData = new BsonDocument {
            { "Id_Joueur", Id_Joueur },
            { "Pseudo", Pseudo },
        };
        ModifyMongoDataFirstDepth(Id_Partie, "Joueur", "Id_Joueur", Id_Joueur, JoueurData);
    }

    public static void ModifyMongoDataFirstDepth(int Id_Partie, string type, string Id_Name, int Id_Value, BsonDocument Data)
    {
        BsonDocument document = ElementExist("Id_Partie", Id_Partie, new BsonArray(SelectMongoData().ToArray()));
        if (document != null)
        {
            if (document.Contains(type))
            {
                if (document.GetValue(type).IsBsonArray)
                {
                    BsonArray TypeArray = document.GetValue(type).AsBsonArray;
                    BsonDocument TypeElement = ElementExist(Id_Name, Id_Value, TypeArray);
                    if (TypeElement != null)
                    {
                        TypeArray.Remove(TypeElement);
                    }
                    TypeArray.Add(Data);
                    document = document.Set(type, TypeArray);
                    SetMongoRequestData(document);
                }
            }
            else
            {
                document = document.Set(type, new BsonArray(Data));
                SetMongoRequestData(document);
            }
        }
    }

    public static void ModifyMongoDataSecondDepth(int Id_Partie, string Type, string SecondType, string Id_Name, string Second_Id_Name, int Id_Value, int Second_Id_Value, BsonDocument Data)
    {
        BsonDocument document = ElementExist("Id_Partie", Id_Partie, new BsonArray(SelectMongoData().ToArray()));
        if (document != null)
        {
            if (document.Contains(Type))
            {
                if (document.GetValue(Type).IsBsonArray)
                {
                    BsonArray TypeArray = document.GetValue(Type).AsBsonArray;
                    BsonDocument TypeElement = ElementExist(Id_Name, Id_Value, TypeArray);
                    if (TypeElement != null)
                    {
                        if (TypeElement.Contains(SecondType))
                        {
                            if (TypeElement.GetValue(SecondType).IsBsonArray)
                            {
                                BsonArray SecondTypeArray = document.GetValue(SecondType).AsBsonArray;
                                BsonDocument SecondTypeElement = ElementExist(Second_Id_Name, Second_Id_Value, SecondTypeArray);
                                if (TypeElement != null)
                                {
                                    SecondTypeArray.Remove(SecondTypeElement);
                                }
                                SecondTypeArray.Add(Data);
                                document = document.Set(SecondType, TypeArray);
                                SetMongoRequestData(document);
                            }
                        }
                        else
                        {
                            document = document.Set(SecondType, new BsonArray(Data));
                            SetMongoRequestData(document);
                        }
                    }
                }
            }
        }
    }



    public static void InsertMongoDeplacement(int Id_Partie, int Id_Joueur, int Id_Deplacement, int InGameTime, int Position)
    {
        BsonDocument DeplacementData = new BsonDocument {
            { "Id_Joueur", Id_Deplacement },
            { "InGameTime", InGameTime },
            { "Position", Position },
        };
        ModifyMongoDataSecondDepth(Id_Partie, "Joueur", "Deplacement", "Id_Joueur", "Id_Deplacement", Id_Joueur, Id_Deplacement, DeplacementData);
    }

    public static void InsertMongoTir(int Id_Partie, int Id_Joueur, int Id_Tir, int InGameTime, int Position, int Direction)
    {
        BsonDocument TirData = new BsonDocument {
            { "Id_Joueur", Id_Tir },
            { "InGameTime", InGameTime },
            { "Position", Position },
            { "Direction", Direction },
        };
        ModifyMongoDataSecondDepth(Id_Partie, "Joueur", "Tir", "Id_Joueur", "Id_Tir", Id_Joueur, Id_Tir, TirData);
    }

    public static BsonDocument SelectMongoData(int Id, string Name, BsonArray list)
    {
        foreach (BsonDocument element in list)
        {
            if (element.GetValue(Name) == Id)
            {
                return element;
            }
        }
        return null;
    }

    public static BsonDocument SelectMongoPartie(int Id_Partie)
    {
        return SelectMongoData(Id_Partie, "Id_Partie", new BsonArray(SelectMongoData().ToArray()));
    }

    public static BsonDocument SelectMongoJoueur(int Id_Partie, int Id_Joueur)
    {
        BsonDocument partie = SelectMongoData(Id_Partie, "Id_Partie", new BsonArray(SelectMongoData().ToArray()));
        return SelectMongoData(Id_Joueur, "Id_Joueur", partie.GetValue("Joueur").AsBsonArray);
    }

    public static BsonArray SelectAllMongoJoueur(int Id_Partie, int Id_Joueur)
    {
        BsonDocument partie = SelectMongoData(Id_Partie, "Id_Partie", new BsonArray(SelectMongoData().ToArray()));
        return partie.GetValue("Joueur").AsBsonArray;
    }

    public static BsonDocument SelectMongoBateau(int Id_Partie, int Id_Bateau)
    {
        BsonDocument partie = SelectMongoData(Id_Partie, "Id_Partie", new BsonArray(SelectMongoData().ToArray()));
        return SelectMongoData(Id_Bateau, "Id_Bateau", partie.GetValue("Bateau").AsBsonArray);
    }

    public static BsonArray SelectAllMongoBateau(int Id_Partie, int Id_Bateau)
    {
        BsonDocument partie = SelectMongoData(Id_Partie, "Id_Partie", new BsonArray(SelectMongoData().ToArray()));
        return partie.GetValue("Bateau").AsBsonArray;
    }

    public static BsonDocument SelectMongoTir(int Id_Partie, int Id_Joueur, int Id_Tir)
    {
        BsonDocument partie = SelectMongoData(Id_Partie, "Id_Partie", new BsonArray(SelectMongoData().ToArray()));
        BsonDocument joueur = SelectMongoData(Id_Joueur, "Id_Joueur", partie.GetValue("Joueur").AsBsonArray);
        return SelectMongoData(Id_Tir, "Id_Tir", partie.GetValue("Tir").AsBsonArray);
    }

    public static BsonArray SelectAllMongoTir(int Id_Partie, int Id_Joueur)
    {
        BsonDocument partie = SelectMongoData(Id_Partie, "Id_Partie", new BsonArray(SelectMongoData().ToArray()));
        BsonDocument joueur = SelectMongoData(Id_Joueur, "Id_Joueur", partie.GetValue("Joueur").AsBsonArray);
        return partie.GetValue("Tir").AsBsonArray;
    }

    public static BsonDocument SelectMongoDeplacement(int Id_Partie, int Id_Joueur, int Id_Deplacement)
    {
        BsonDocument partie = SelectMongoData(Id_Partie, "Id_Partie", new BsonArray(SelectMongoData().ToArray()));
        BsonDocument joueur = SelectMongoData(Id_Joueur, "Id_Joueur", partie.GetValue("Joueur").AsBsonArray);
        return SelectMongoData(Id_Deplacement, "Id_Deplacement", partie.GetValue("Deplacement").AsBsonArray);
    }

    public static BsonArray SelectAllMongoDeplacement(int Id_Partie, int Id_Joueur)
    {
        BsonDocument partie = SelectMongoData(Id_Partie, "Id_Partie", new BsonArray(SelectMongoData().ToArray()));
        BsonDocument joueur = SelectMongoData(Id_Joueur, "Id_Joueur", partie.GetValue("Joueur").AsBsonArray);
        return partie.GetValue("Deplacement").AsBsonArray;
    }

    /*
     * 
{
    "Id_Partie" : "",
    "Taille" : "",
    "Debut" : "",
    "TempsEcoule" : "",
    "DureeMax" : "",
    "Jeu": "",
    "Joueurs" : [
        {
            "Id_Joueur": "",
            "Pseudo": "",
            "Deplacements" : [
                {
                    "InGameTime" : "",
                    "Position" : ""
                }
            ],
            "Tirs" : [
                {
                    "Direction" : "",
                    "InGameTime" : "",
                    "Position" : ""
                }
            ]
        }
    ],
    "Bateau" : [
        {
            "Id_Bateau" : "",
            "Position" : "",
            "Taille" : "",
            "Type" : "",
            "Axe" : ""
        }
    ]
}
    */
}