using System;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InformationManager : MonoBehaviour
{
    public TMP_InputField Nom;
    public TMP_InputField Prenom;
    public TMP_InputField Age;
    public TMP_InputField Pseudo;
    public TMP_InputField Mail;
    public TMP_InputField Password;

    private void Start()
    {
        SQLManager.Setup();
    }

    public void Inscription()
    {
        string nom = Nom.text;
        string prenom = Prenom.text;
        int age = Int32.Parse(Age.text);
        string pseudo = Pseudo.text;
        string password = Password.text;
        string mail = Mail.text;

        int playerID = SQLManager.CreatePlayer(pseudo, nom, prenom, mail, password, age);
        GameManager.Instance.PlayerID = playerID;
        ChangeScene();
    }

    public void Connexion()
    {
        string pseudo = Pseudo.text;
        string password = Password.text;

        int player = SQLManager.PlayerID(pseudo, password);

        if(player != -1)
        {
            GameManager.Instance.PlayerID = player;
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("ChoosePartie", LoadSceneMode.Single);
    }

}
