using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool IsWhiteTurn = true; // Indique si c'est le tour du joueur blanc

    // Change le tour du joueur
    public static void SwitchTurn()
    {
        IsWhiteTurn = !IsWhiteTurn; // Alterne entre les tours
        Debug.Log(IsWhiteTurn ? "C'est le tour des Blancs." : "C'est le tour des Noirs.");
    }
}
