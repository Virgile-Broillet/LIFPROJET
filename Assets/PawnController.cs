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

    // Gère le clic souris pour déplacer le pion
    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Si clic gauche
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Lancer un raycast depuis la position de la souris
            if (Physics.Raycast(ray, out hit))
            {
                // Vérifie si on clique sur une case ("Tiles")
                if (hit.collider.CompareTag("Tiles"))
                {
                    Vector3 hitPosition = hit.collider.transform.position;

                    // Vérifie si la case est une case valide (1 case ou 2 cases au premier mouvement)
                    if (IsValidMove(hitPosition))
                    {
                        targetPosition = hitPosition; // Met à jour la position cible
                        isMoving = true; // Active le mouvement

                        // Désactive le premier mouvement après le déplacement
                        if (isFirstMove)
                            isFirstMove = false;

                        // Une fois le pion déplacé, passer au joueur suivant
                        GameManager.SwitchTurn();
                    }
                    else
                    {
                        Debug.Log("Déplacement invalide.");
                    }
                }
            }
        }
    }

    // Vérifie si le déplacement est valide
    bool IsValidMove(Vector3 hitPosition)
    {
        Vector3 currentPosition = transform.position;

        // Calcul de la distance entre la position actuelle et la position cible
        float distanceX = Mathf.Abs(hitPosition.x - currentPosition.x);
        float distanceZ = hitPosition.z - currentPosition.z; // Direction "avant" seulement

        // Vérifie si la cible est à 1 case devant le pion
        if (distanceX == 0 && distanceZ == (isPlayerWhite ? 1 : -1)) // 1 case en avant
        {
            return true;
        }

        // Vérifie si le pion peut avancer de 2 cases au premier mouvement
        if (isFirstMove && distanceX == 0 && distanceZ == (isPlayerWhite ? 2 : -2)) // 2 cases en avant pour le premier tour
        {
            return true;
        }

        // Si aucune condition n'est remplie, le mouvement est invalide
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
        }
    }

    // Méthode appelée lors du clic sur le pion
    public void OnPawnClicked()
    {
        // Vérifie si c'est le tour du joueur auquel appartient ce pion
        if (isPlayerWhite != GameManager.IsWhiteTurn)
        {
            Debug.Log("Ce n'est pas le tour de ce joueur !");
            return; // Ne rien faire si ce n'est pas le tour du joueur
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
