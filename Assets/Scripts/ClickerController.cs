using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClickerController : MonoBehaviour
{
    public TextMeshProUGUI count; // texto en pantalla
    public TextMeshProUGUI timer; // temporizador
    public TextMeshProUGUI level; // nivel

    private float timeRemaining, timeRemainingMax = 10; // variable del temporizador (segundos) para modificar

    private int levelCounter = 1; // variable del nivel para modificar

    int counter, counterMax = 30; // contador del numero de clicks, debe llegar a 0 ademas del contador del numero de clicks, debe llegar a

    void OnMouseDown()
    {
        counter--;
        count.SetText("Contador = " + counter);
        Debug.Log(counter);
    }

    void Update(){

        if(counter <= 0)
        {   
            levelCounter++;
            counter = counterMax;
            timeRemaining = timeRemainingMax;
        }

    }

    private void LateUpdate()
    {
        updateInfo();

        // comprobacion del temporizador
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

        }
        else
        {
            if (counter > 0) // si no se ha llegado a vaciar el numero de clicks
            {
                count.SetText("Has fallado");
                timer.SetText("Has fallado");
            }
            else
            {
                // reseteamos el contador y le aumentamos 10, el contador le damos 5 segundos mas y el nivel lo subimos en 1

                lvlUp();
            }
        }

    }

    private void updateInfo()
    {
        // iniciamos escribiendo en los textos la informacion del nivel
        timer.SetText("Tiempo = " + (int)timeRemaining);
        count.SetText("Contador = " + counter);
        level.SetText("Nivel " + levelCounter);
    }

    void lvlUp()
    {
        levelCounter++;
        counter = counterMax + 15;
        timeRemaining = timeRemainingMax + 5;
        counterMax = counter;
        timeRemainingMax = timeRemaining;

        // hacemos el cubo mas pequeño
        Vector3 newScale = transform.localScale;
        newScale *= 1.2f;
        transform.localScale = newScale;

    }
}