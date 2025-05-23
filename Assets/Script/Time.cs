using UnityEngine;
using UnityEngine.UI;

public class TurnTime : MonoBehaviour
{
    public Color player1Color;
    public Color player2Color;
    public float time;
    public float turnTimeLimit = 15;
    private bool running = true;

    [Header("UI")]
    public Image timeBarFill;

    void Update()
    {
        if (!running) return;

        time += Time.deltaTime;

        if (timeBarFill != null)
        {
            timeBarFill.fillAmount = Mathf.Lerp(1f, 0f, time / turnTimeLimit);
        }

        if (time >= turnTimeLimit)
        {
            Debug.Log("Tiempo agotado, cambiando turno");
            running = false;
            GameEvents.TurnChange.Invoke();
        }

    }

    public void SetBarForPlayer(PieceOwner owner)
    {
        if (timeBarFill != null)
        {
            timeBarFill.color = (owner == PieceOwner.Player1) ? player1Color : player2Color;
            timeBarFill.fillOrigin = (owner == PieceOwner.Player1) ? 0 : 1;
        }
    }

    public void ResetTime()
    {
        time = 0;
        running = true;
        if (timeBarFill != null)
            timeBarFill.fillAmount = 1f;
    }
}
