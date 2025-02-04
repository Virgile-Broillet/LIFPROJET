using UnityEngine;

public class RookController : PieceController // Hérite de PieceController
{
    public float moveSpeed = 5f; // Vitesse de déplacement
    private bool isSelected = false; // État de sélection
    private Vector3 targetPosition; // Position de destination
    private bool isMoving = false; // Si la tour est en train de se déplacer

    private void Update()
    {
        // Si la tour est sélectionnée, on permet de la déplacer
        if (isSelected && !isMoving)
        {
            HandleMouseClick();
        }

        // Si la tour est en mouvement, on la déplace
        if (isMoving)
        {
            MoveRook();
        }
    }

    // Gère le clic souris pour déplacer la tour
    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Si clic gauche
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Lancer un raycast depuis la position de la souris
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Vérifie si on clique sur une case ("Tiles")
                if (hit.collider.CompareTag("Tiles"))
                {
                    Vector3 hitPosition = hit.collider.transform.position;

                    // Vérifie si la case est une case valide pour la tour
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

    // Vérifie si le déplacement est valide
    bool IsValidMove(Vector3 hitPosition)
    {
        Vector3 currentPosition = transform.position;

        // Convertir en entiers pour éviter les imprécisions
        int currentX = Mathf.RoundToInt(currentPosition.x);
        int currentZ = Mathf.RoundToInt(currentPosition.z);
        int targetX = Mathf.RoundToInt(hitPosition.x);
        int targetZ = Mathf.RoundToInt(hitPosition.z);

        // Calcul des distances
        int distanceX = Mathf.Abs(targetX - currentX);
        int distanceZ = Mathf.Abs(targetZ - currentZ);

        // La tour peut se déplacer uniquement verticalement ou horizontalement
        if (distanceX == 0 || distanceZ == 0)
        {
            // Vérifie si la trajectoire est dégagée et valide pour le mouvement
            if (IsPathClear(currentPosition, hitPosition))
            {
                // Vérifie si la case de destination est libre ou occupée par une pièce ennemie
                if (IsDestinationFree(hitPosition))
                {
                    return true;
                }
                else
                {
                    Debug.Log("La case de destination est occupée par une pièce de la même couleur.");
                }
            }
        }

        return false; // Mouvement invalide
    }

    // Vérifie si la case de destination est libre
    bool IsDestinationFree(Vector3 targetPosition)
    {
        // Vérifie si la case de destination est occupée par une pièce de la même couleur
        Collider[] colliders = Physics.OverlapSphere(targetPosition, 0.1f);
        foreach (Collider collider in colliders)
        {
            PieceController piece = collider.GetComponent<PieceController>();
            if (piece != null && piece.isPlayerWhite == this.isPlayerWhite)
            {
                return false; // Il y a une pièce alliée sur la case
            }
        }
        return true; // La case est libre
    }

    // Vérifie si le chemin entre la position actuelle et la position cible est dégagé
    bool IsPathClear(Vector3 currentPosition, Vector3 targetPosition)
    {
        // Vérifie si le déplacement est horizontal (X change)
        if (currentPosition.x != targetPosition.x)
        {
            float minX = Mathf.Min(currentPosition.x, targetPosition.x);
            float maxX = Mathf.Max(currentPosition.x, targetPosition.x);

            // Vérifie chaque case intermédiaire sur le chemin horizontal
            for (float x = minX + 1; x < maxX; x++)
            {
                Vector3 checkPosition = new Vector3(x, currentPosition.y, currentPosition.z);
                Collider[] colliders = Physics.OverlapSphere(checkPosition, 0.1f);

                // Si une pièce est présente sur cette case, le déplacement est invalide
                foreach (Collider collider in colliders)
                {
                    PieceController piece = collider.GetComponent<PieceController>();
                    if (piece != null)
                    {
                        // Si la pièce est de la même couleur, le déplacement est invalide
                        if (piece.isPlayerWhite == this.isPlayerWhite)
                        {
                            return false; // Il y a une pièce alliée sur la trajectoire
                        }
                    }
                }
            }
        }
        // Vérifie si le déplacement est vertical (Z change)
        else if (currentPosition.z != targetPosition.z)
        {
            float minZ = Mathf.Min(currentPosition.z, targetPosition.z);
            float maxZ = Mathf.Max(currentPosition.z, targetPosition.z);

            // Vérifie chaque case intermédiaire sur le chemin vertical
            for (float z = minZ + 1; z < maxZ; z++)
            {
                Vector3 checkPosition = new Vector3(currentPosition.x, currentPosition.y, z);
                Collider[] colliders = Physics.OverlapSphere(checkPosition, 0.1f);

                // Si une pièce est présente sur cette case, le déplacement est invalide
                foreach (Collider collider in colliders)
                {
                    PieceController piece = collider.GetComponent<PieceController>();
                    if (piece != null)
                    {
                        // Si la pièce est de la même couleur, le déplacement est invalide
                        if (piece.isPlayerWhite == this.isPlayerWhite)
                        {
                            return false; // Il y a une pièce alliée sur la trajectoire
                        }
                    }
                }
            }
        }

        return true; // Le chemin est dégagé
    }

    // Déplace la tour vers la position cible
    void MoveRook()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Si la tour atteint la position cible, arrêter le mouvement
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            Debug.Log(gameObject.name + " a atteint sa destination.");

            // Si la case d'arrivée est occupée par une pièce ennemie, la capturer
            Collider[] colliders = Physics.OverlapSphere(targetPosition, 0.1f);
            foreach (Collider collider in colliders)
            {
                PieceController piece = collider.GetComponent<PieceController>();
                if (piece != null && piece.isPlayerWhite != this.isPlayerWhite)
                {
                    // Capture la pièce ennemie
                    Destroy(piece.gameObject);
                    Debug.Log("Pièce ennemie capturée.");
                }
            }
            DeselectRook();
            GameManager.SwitchTurn(); // Passer au joueur suivant
        }
    }

    // Méthode appelée lors du clic sur la tour
    public void OnRookClicked()
    {
        // Vérifie si c'est le tour du joueur auquel appartient cette tour
        if (isPlayerWhite != GameManager.IsWhiteTurn)
        {
            Debug.Log("Ce n'est pas le tour de ce joueur !");
            return; // Ne rien faire si ce n'est pas le tour du joueur
        }

        isSelected = !isSelected; // Alterner l'état sélectionné
        Debug.Log(isSelected ? gameObject.name + " a été sélectionné." : gameObject.name + " a été désélectionné.");
    }

    // Méthode pour désélectionner la tour
    public void DeselectRook()
    {
        isSelected = false;
        Debug.Log(gameObject.name + " a été désélectionné.");
    }
}
