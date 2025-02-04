using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool IsWhiteTurn = true; // Tour des Blancs commence

    public static void SwitchTurn()
    {
        IsWhiteTurn = !IsWhiteTurn; // Alterner le tour
        Debug.Log(IsWhiteTurn ? "Tour des Blancs." : "Tour des Noirs.");
    }
}
