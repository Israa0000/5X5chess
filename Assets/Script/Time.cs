using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTime : MonoBehaviour
{
    public float time;
    public float turnTimeLimit = 15;
    private bool running = true;

    //el temporizador se utiliza para cambiar turno despues de q se sobre pase el tiempo,
    //una vez q se ponga en true, se tiene q volver a poner en false para el siguiente turno 

    private void Update() 
    {
        if (!running) return;

        time += Time.deltaTime;

        if (time >= turnTimeLimit)
        {
            running = false; // Detiene el temporizador
            GameEvents.TurnChange.Invoke();
        }
    }

    public void ResetTime()
    {
        time = 0;
        running = true; // Reactiva el temporizador para el nuevo turno
    }
}

