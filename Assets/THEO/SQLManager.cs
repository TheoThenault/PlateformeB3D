using UnityEngine;
using MySqlConnector;
using System.Xml.Schema;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using Mysqlx.Crud;
using NUnit.Framework.Internal;
using Azure;
using System;
using Random = UnityEngine.Random;

public class SQLManager : MonoBehaviour
{
    private string Connexion = "Server=theothenault.fr;Port=8090;Database=plateforme_B3D;Uid=Unity;Pwd=";

    private MySqlConnection sqlConnection;

    private void Start()
    {
        sqlConnection = new MySqlConnection(Connexion);
        //MySqlConnector.Logging.MySqlConnectorLogManager.Provider = new MySqlConnector.Logging.ConsoleLoggerProvider(MySqlConnector.Logging.MySqlConnectorLogLevel.Debug);

        sqlConnection.Open();

        if (sqlConnection.Ping())
            Debug.Log("OK");
    }

    private void OnDestroy()
    {
        sqlConnection.Close();
    }

    public void test()
    {
        //CreatePlayer("Theo", "AZERTY", "Theo", "Theo@example.com", "AZERTY123", 12);

        Debug.Log(CreatePartie());

        //var players = NameAndAgeOfPlayersFromPartie(1);

        //Debug.Log(NameAndAgeAndCourrielOfMostPlayerWithMostGamePlayed());
    }

    public void CreatePlayer(string Pseudo, string Nom, string Prenom, string Courriel, string Motdepasse, int Age)
    {
        var cmd = new MySqlCommand("INSERT INTO Joueur (Pseudo, Nom, Prenom, Courriel, MotDePasse, Age)" +
                                    "VALUES ('" + Pseudo + "', '" + Nom + "', '" + Prenom + "', '" + Courriel + "', '" + Motdepasse + "', '" + Age + "')", sqlConnection);
        cmd.ExecuteNonQuery();
    }

    public int CreatePartie()
    {
        int partieId = 0;
        var date = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
        
        var cmd = new MySqlCommand("INSERT INTO Partie (Jeu, Debut) VALUES ('B3D', '"+date+"')", sqlConnection);
        cmd.ExecuteNonQuery();

        cmd = new MySqlCommand("SELECT id FROM Partie ORDER BY id DESC LIMIT 1;", sqlConnection); 
        var reader = cmd.ExecuteReader();
        if(reader.Read())
            partieId = reader.GetInt32("id");
        reader.Close();
        return partieId;
    }

    public List<(string, string, int)> NameAndAgeOfPlayersFromPartie(int partieId)
    {
        var cmd = new MySqlCommand("SELECT Nom, Prenom, Age FROM Joueur j INNER JOIN Participants p ON j.id = p.idJoueur INNER JOIN Partie pr ON p.idPartie = pr.id WHERE pr.Jeu = 'B3D' AND j.Age >= 0 AND p.idPartie = " + partieId, sqlConnection);
        var reader = cmd.ExecuteReader();
        List<(string, string, int)> res = new();
        while (reader.Read())
        {
            Debug.Log(reader.GetString("Nom") + " " + reader.GetString("Prenom") + " " + reader.GetInt32("Age"));
            res.Add((reader.GetString("Nom"), reader.GetString("Prenom"), reader.GetInt32("Age")));
        }
        reader.Close();
        return res;
    }

    public (string, int, string) NameAndAgeAndCourrielOfMostPlayerWithMostGamePlayed()
    {
        var cmd = new MySqlCommand("SELECT Nom, Age, Courriel FROM Joueur j INNER JOIN Participants p ON j.id = p.idJoueur INNER JOIN Partie pr ON p.idPartie = pr.id WHERE pr.Jeu = 'B3D' AND j.Age >= 0 GROUP BY j.id LIMIT 1;", sqlConnection);
        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var v = (reader.GetString("Nom"), reader.GetInt32("Age"), reader.GetString("Courriel"));
            reader.Close();
            return v;
        }
        return ("null", -1, "null");
    }

    public void SupprimerJoueur(int idJoueur)
    {
        var cmd = new MySqlCommand("UPDATE Joueur SET Pseudo = 'null', MotDePasse = 'null', Courriel = 'null', Nom = 'null', Prenom = 'null', Age = -1 WHERE id = "+ idJoueur, sqlConnection);
        cmd.ExecuteNonQuery();
    }

    public int AveragePlayerAge()
    {
        var cmd = new MySqlCommand("SELECT AVG(sub.Age) AS avg_age FROM(SELECT DISTINCT j.id, j.Age FROM Joueur j INNER JOIN Participants p ON j.id = p.idJoueur INNER JOIN Partie pa ON p.idPartie = pa.id WHERE pa.Jeu = 'B3D' AND j.Age >= 0) AS sub;", sqlConnection);
        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            int v = reader.GetInt32("avg_age");
            reader.Close();
            return v;
        }
        return -1;
    }

    public string NomJoueurAvecLePlusDeVictoire()
    {
        var cmd = new MySqlCommand("SELECT Nom FROM Joueur j INNER JOIN Gagnants g ON j.id = g.idJoueur INNER JOIN Partie p ON g.idPartie = p.id WHERE p.Jeu = 'B3D' AND j.Age >= 0 GROUP BY j.id LIMIT 1;", sqlConnection);
        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var v = reader.GetString("Nom");
            reader.Close();
            return v;
        }
        return "null";
    }

    public List<string> NomsJeuxClassesParNbPartieDecroissant()
    {
        var cmd = new MySqlCommand("SELECT Jeu FROM Partie p GROUP BY Jeu ORDER BY COUNT(Jeu) DESC", sqlConnection);
        var reader = cmd.ExecuteReader();
        List<string> res = new();
        while (reader.Read())
        {
            Debug.Log(reader.GetString("Nom"));
            res.Add(reader.GetString("Nom"));
        }
        reader.Close();
        return res;
    }

    public int NombreJoueurB3D()
    {
        var cmd = new MySqlCommand("SELECT COUNT(*) AS nb FROM(SELECT p.idJoueur FROM Participants p INNER JOIN Partie pa ON p.idPartie = pa.id INNER JOIN Joueur j ON p.idJoueur = j.id WHERE j.Age >= 0 GROUP BY p.idJoueur HAVING SUM(pa.Jeu = 'B3D') > 0 AND SUM(pa.Jeu<> 'B3D') = 0) AS sub;", sqlConnection);
        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            int v = reader.GetInt32("nb");
            reader.Close();
            return v;
        }
        return -1;
    }

    public List<(int, int, int)> NombrePartieParSemaineB3D()
    {
        var cmd = new MySqlCommand("SELECT YEAR(pa.Debut) AS annee, WEEK(pa.Debut, 3) AS semaine, COUNT(*) AS nb_parties FROM Partie pa WHERE pa.Jeu = 'B3D' GROUP BY YEAR(pa.Debut), WEEK(pa.Debut, 3) ORDER BY annee, semaine;", sqlConnection);
        var reader = cmd.ExecuteReader();
        List<(int, int, int)> res = new();
        while (reader.Read())
        {
            Debug.Log(reader.GetInt32("annee") + " " + reader.GetInt32("semaine") + " " + reader.GetInt32("nb_parties"));
            res.Add((reader.GetInt32("annee"), reader.GetInt32("semaine"), reader.GetInt32("nb_parties")));
        }
        reader.Close();
        return res;
    }
}
