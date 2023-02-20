using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlimeController : MonoBehaviour
{
    private bool canJump = true;
    private TextMeshProUGUI count; // texto en pantalla
    private TextMeshProUGUI timer; // temporizador en pantalla
    private TextMeshProUGUI level; // nivel en pantalla
    private TextMeshProUGUI title; // nivel en pantalla
    private TextMeshProUGUI score; // numero total de clicks
    private TextMeshProUGUI explication; // texto de la explicacion
    private TextMeshProUGUI finalTextScore; // texto que mostrara la puntuacion y cuantos bichos has matado
    private GameObject panelScore;
    private Button closePanelFinishButton;
    private GameObject slimeObjectInScene; // el slime en la escena
    private Button startButton;
    private float timeRemaining;
    private float timeRemainingMax; // variable del temporizador (segundos) para modificar
    private bool timerCounting;
    private int levelCounter; // variable del nivel para modificar
    private int counter;
    private int GLOBALCOUNTERS;
    private int counterMax; // contador del numero de clicks, debe llegar a 0 ademas del contador del numero de clicks, debe llegar a
    private Dictionary<string, int> HasBeenKilled;

    private void Awake()
    {
        timeRemaining = 10;
        timeRemainingMax = 10;
        timerCounting = false;
        levelCounter = 1;
        counter = 10; // -> debe de estar en 30
        GLOBALCOUNTERS = 0;
        counterMax = 10; // -> debe de estar en 30

        HasBeenKilled = new Dictionary<string, int>{
            {"Slime_01",0}, {"Slime_01_King",0}, {"Slime_01_MeltalHelmet",0},
            {"Slime_01_Viking",0}, {"Slime_02",0}, {"Slime_03 King",0},
            {"Slime_03",0}, {"Slime_03 Sprout",0}, {"Slime_03 Leaf",0}
        };

        // stateamos los elementos
        startButton = GameObject.Find("Start").GetComponent<Button>();
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        count = GameObject.Find("Contador").GetComponent<TextMeshProUGUI>();
        level = GameObject.Find("Level").GetComponent<TextMeshProUGUI>();
        title = GameObject.Find("Title").GetComponent<TextMeshProUGUI>();
        score = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        panelScore = GameObject.Find("PanelFinish");
        closePanelFinishButton = GameObject.Find("ButtonExitPanel").GetComponent<Button>();
        finalTextScore = GameObject.Find("AllScoreText").GetComponent<TextMeshProUGUI>();


        // le añadimos el comportamiento a los botones
        startButton.onClick.AddListener(() => { StartGame(); });
        closePanelFinishButton.onClick.AddListener(() => { panelScore.gameObject.SetActive(false); }); // le decimos que cuando clickes el boton de cerrar del panel final, se desactive el panel completo

        // desabilitamos lo que visualizamos por pantalla al inicio del juego 
        timer.gameObject.SetActive(false);
        count.gameObject.SetActive(false);
        level.gameObject.SetActive(false);
        score.gameObject.SetActive(false);
        panelScore.gameObject.SetActive(false);
    }

    private void Start() { InitSlime(); }

    private void Update()
    {
        updateInfo();

        // este condicional lanza un rayo desde la posicion del raton hacia la escena y detecta cosas, para detectar el slime basicamente
        if (Input.GetMouseButtonDown(0)) // si detecta un click izquierdo
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // hace un raycast desde la posicion del puntero hasta 
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, 100))
            {
                if (raycastHit.transform.gameObject.name.Contains("Slime") && timerCounting && canJump)
                {

                    // compruebo puede saltar y si el tiempo esta corriendo, el "puedo saltar" viene del mismo script del slime
                    if (slimeObjectInScene.GetComponent<OnDestroySlime>().canJump && timerCounting)
                    {
                        slimeObjectInScene.GetComponent<OnDestroySlime>().canJump = false;
                        slimeObjectInScene.GetComponent<OnDestroySlime>().Jump(levelCounter);
                    }

                    // siempre tiene que descontar el contador
                    counter--;
                    GLOBALCOUNTERS += levelCounter;
                    score.SetText("SCORE: " + GLOBALCOUNTERS);

                }
            }
        }

        // comprobaciones del temporizador

        // si aun queda tiempo, y el contador puede descontar, entonces descuenta
        if (timeRemaining > 0 && timerCounting)
        {
            timeRemaining -= Time.deltaTime;
        }

        if (counter <= 0)
        {
            LvlUp();
        }

        if (timeRemaining <= 0 && counter >= 0 && timerCounting)
        {
            // has fallado
            count.SetText("Has fallado");
            timer.SetText("Has fallado");
            timerCounting = false;
            startButton.gameObject.SetActive(true);
            title.gameObject.SetActive(true);
            panelScore.gameObject.SetActive(true);
            finalTextScore.text = TextFromFinal();
        }

    }

    private string TextFromFinal()
    {
        string finalText = "";

        finalText += "Score: " + GLOBALCOUNTERS + System.Environment.NewLine;
 
        foreach (var item in HasBeenKilled)
        {
            finalText += "->" + item.Key + ": " + item.Value + System.Environment.NewLine; 
        };

        return finalText;
            
    }

    private void updateInfo()
    {
        // iniciamos escribiendo en los textos la informacion del nivel
        timer.SetText("Tiempo = " + (int)timeRemaining);
        count.SetText("Contador = " + counter);
        level.SetText("Nivel " + levelCounter);
        slimeObjectInScene = GameObject.FindGameObjectWithTag("Slime");
    }

    void LvlUp()
    {
        // reseteamos las variables
        levelCounter++;
        counter = counterMax + 15;
        timeRemaining = timeRemainingMax + 5;
        counterMax = counter;
        timeRemainingMax = timeRemaining;

        upKillToScore(slimeObjectInScene.name);

        // destruimos el slime
        Destroy(slimeObjectInScene.gameObject);

        // vaciamos el contenedor
        slimeObjectInScene = null;
        // lo creamos otra vez
        InitSlime();
    }

    void upKillToScore(string slimeName)
    {
        string[] subs = slimeName.Split('('); // splitearemos la frase para que podamos coger el nombre sin el "(Clone)"
        HasBeenKilled[subs[0]]++; // pasandole el nombre del slime, le decimos que hemos derrotado a 1
    }

    void StartGame()
    {
        resetAll();
        // comenzara a contar atras el temporizador
        timerCounting = true;
        // ocultamos el boton
        startButton.gameObject.SetActive(false);
        // mostramos los textos en pantalla
        timer.gameObject.SetActive(true);
        count.gameObject.SetActive(true);
        level.gameObject.SetActive(true);
        title.gameObject.SetActive(false);
        score.gameObject.SetActive(true);
        score.SetText("SCORE: " + GLOBALCOUNTERS);
    }

    void resetAll()
    {
        HasBeenKilled = new Dictionary<string, int>{
            {"Slime_01",0}, {"Slime_01_King",0}, {"Slime_01_MeltalHelmet",0},
            {"Slime_01_Viking",0}, {"Slime_02",0}, {"Slime_03 King",0},
            {"Slime_03",0}, {"Slime_03 Sprout",0}, {"Slime_03 Leaf",0}
        };
        timeRemaining = 10;
        timeRemainingMax = 10;
        counter = 10; // a 30
        counterMax = 10; // a 30
        GLOBALCOUNTERS = 0;
        levelCounter = 1;
    }

    void InitSlime()
    {
        string[] arr = { "Slime_01", "Slime_01_King", "Slime_01_MeltalHelmet", "Slime_01_Viking", "Slime_02", "Slime_03 King", "Slime_03 Leaf", "Slime_03", "Slime_03 Sprout" };
        // crear el minguito, asi lo destruyo cuando suba de nivel
        GameObject sl = Resources.Load<GameObject>("Kawaii Slimes/prefabs/" + arr[Random.Range(0, 7)]);

        Instantiate(sl, new Vector3(5.04f, 2f, 3.14f), Quaternion.Euler(0f, 180f, 0f));
        slimeObjectInScene = GameObject.FindGameObjectWithTag("Slime");
    }

}