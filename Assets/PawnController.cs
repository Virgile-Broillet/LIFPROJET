using UnityEngine;

public class PawnController : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de déplacement
    public bool isPlayerWhite = true; // Détermine si ce pion appartient au joueur blanc
    private bool isSelected = false; // État de sélection
    private Vector3 targetPosition; // Position de destination
    private bool isMoving = false; // Si le pion est en train de se déplacer
    private bool isFirstMove = true; // Premier déplacement du pion

    private void Update()
    {
        // Si le pion est sélectionné et que ce n'est pas son tour, on le désélectionne
        if (isSelected && isPlayerWhite != GameManager.IsWhiteTurn)
        {
            isSelected = false;
        }

        // Si le pion est sélectionné, on permet de le déplacer
        if (isSelected && !isMoving)
        {
            HandleMouseClick();
        }

        // Si le pion est en mouvement, on le déplace
        if (isMoving)
        {
            MovePawn();
        }
    }

    // Gère le clic souris pour sélectionner et déplacer le pion
    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Si clic gauche
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Lancer un raycast depuis la position de la souris
            if (Physics.Raycast(ray, out hit))
            {
                // Si on clique sur une case valide ("Tiles")
                if (hit.collider.CompareTag("Tiles"))
                {
                    Vector3 hitPosition = hit.collider.transform.position;

                    // Vérifie si la case est une case valide
                    if (IsValidMove(hitPosition))
                    {
                        targetPosition = hitPosition; // Met à jour la position cible
                        isMoving = true; // Active le mouvement
                    }
                    else
                    {
                        Debug.Log("Déplacement invalide.");
                    }
                }
            }
        }
    }

    bool IsValidMove(Vector3 hitPosition)
{
    Vector3 currentPosition = transform.position;

    // Convertir en entiers pour éviter les imprécisions
    int currentX = Mathf.RoundToInt(currentPosition.x);
    int currentZ = Mathf.RoundToInt(currentPosition.z);
    int targetX = Mathf.RoundToInt(hitPosition.x);
    int targetZ = Mathf.RoundToInt(hitPosition.z);

    // Calculer les distances
    int distanceX = Mathf.Abs(targetX - currentX);
    int distanceZ = targetZ - currentZ; // Peut être positif (blanc) ou négatif (noir)

    // Debugging pour voir les valeurs exactes
    Debug.Log($"Vérification déplacement de {gameObject.name} : ({currentX}, {currentZ}) → ({targetX}, {targetZ})");
    Debug.Log($"DistanceX: {distanceX}, DistanceZ: {distanceZ}, isFirstMove: {isFirstMove}");

    // Vérifier un mouvement de 2 cases en avant si c'est le premier déplacement
    if (isFirstMove && distanceX == 0 && distanceZ == (isPlayerWhite ? 2 : -2))
    {
        isFirstMove = false;
        return true;
    }else if (distanceX == 0 && distanceZ == (isPlayerWhite ? 1 : -1)) // Vérifier un mouvement de 1 case en avant
    {
        isFirstMove = false;
        return true;
    }

    // Déplacement invalide
    return false;
}

    // Déplace le pion vers la position cible
    void MovePawn()
{
    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

    // Si le pion atteint la position cible, arrêter le mouvement
    if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
    {
        isMoving = false;
        Debug.Log(gameObject.name + " a atteint sa destination.");

        // Désactiver le premier mouvement uniquement si le pion a avancé de 2 cases
        if (Mathf.Abs(targetPosition.z - transform.position.z) == 2)
        {
            isFirstMove = false;
        }

        // Désélectionner le pion après déplacement
        isSelected = false;

        // Changer de tour après un déplacement réussi
        GameManager.SwitchTurn();
    }
}


    // Méthode appelée lors du clic sur le pion
    public void OnPawnClicked()
    {
        // Vérifie si c'est le tour du joueur
        if (isPlayerWhite != GameManager.IsWhiteTurn)
        {
            Debug.Log("Ce n'est pas le tour de ce joueur !");
            return; // Ne pas autoriser la sélection
        }

        isSelected = !isSelected; // Alterner l'état sélectionné
        Debug.Log(isSelected ? gameObject.name + " a été sélectionné." : gameObject.name + " a été désélectionné.");
    }

    // Méthode pour désélectionner le pion
    public void DeselectPawn()
    {
        isSelected = false;
        Debug.Log(gameObject.name + " a été désélectionné.");
    }
}
