using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static bool IsWhiteTurn = true; // Tour des Blancs commence
    public Camera mainCamera; // La caméra principale
    public float rotationSpeed = 2f; // Vitesse de rotation
    public float positionSpeed = 2f; // Vitesse de déplacement

    private Vector3 whitePosition = new Vector3(-0.5f, 8, -5.5f);
    private Vector3 blackPosition = new Vector3(-0.5f, 8, 4.5f);
    private Vector3 whiteRotation = new Vector3(60, 0, 0);
    private Vector3 blackRotation = new Vector3(60, 180, 0);

    public static void SwitchTurn()
    {
        IsWhiteTurn = !IsWhiteTurn;
        Debug.Log(IsWhiteTurn ? "Tour des Blancs." : "Tour des Noirs.");

        GameManager instance = FindFirstObjectByType<GameManager>();
        instance.RotateAndMoveCamera();
    }

    void RotateAndMoveCamera()
    {
        Vector3 targetPosition = IsWhiteTurn ? whitePosition : blackPosition;
        Vector3 targetRotation = IsWhiteTurn ? whiteRotation : blackRotation;
        StartCoroutine(MoveAndRotateCamera(targetPosition, targetRotation));
    }

    IEnumerator MoveAndRotateCamera(Vector3 targetPosition, Vector3 targetRotation)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        Quaternion targetQuat = Quaternion.Euler(targetRotation);
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * positionSpeed;
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetQuat, elapsedTime);
            yield return null;
        }

        // Pour s'assurer que la caméra atteint bien la position et la rotation finale
        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetQuat;
    }
}
