using UnityEngine;

public class KingController : PieceController
{
    public float moveSpeed = 5f; // Vitesse de déplacement
    private bool isSelected = false; // État de sélection
    private Vector3 targetPosition; // Position de destination
    private bool isMoving = false; // Si le roi est en train de se déplacer

    private void Update()
    {
        // Si le roi est sélectionné, on permet de le déplacer
        if (isSelected && !isMoving)
        {
            HandleMouseClick();
        }

        // Si le roi est en mouvement, on le déplace
        if (isMoving)
        {
            MoveKing();
        }

        // Vérifie si le roi peut encore bouger, sinon termine la partie
        if (isPlayerWhite == GameManager.IsWhiteTurn && !CanKingMove())
        {
            EndGame("Perdu ! Le roi ne peut plus se déplacer.");
        }
    }

    // Gère le clic souris pour déplacer le roi
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

                    // Vérifie si le mouvement est valide pour le roi
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

    // Vérifie si le mouvement est valide pour le roi
    bool IsValidMove(Vector3 hitPosition)
    {
        Vector3 currentPosition = transform.position;

        // Convertir en entiers pour éviter les imprécisions
        int currentX = Mathf.RoundToInt(currentPosition.x);
        int currentZ = Mathf.RoundToInt(currentPosition.z);
        int targetX = Mathf.RoundToInt(hitPosition.x);
        int targetZ = Mathf.RoundToInt(hitPosition.z);

        // Le roi se déplace d'une case dans n'importe quelle direction
        int dx = Mathf.Abs(targetX - currentX);
        int dz = Mathf.Abs(targetZ - currentZ);

        if (dx <= 1 && dz <= 1) // Le roi peut se déplacer d'une case
        {
            // Vérifie si la case cible est occupée par une pièce alliée
            Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f); // Utilise un rayon pour vérifier la collision

            foreach (Collider collider in colliders)
            {
                PieceController piece = collider.GetComponent<PieceController>();
                if (piece != null && piece.isPlayerWhite == this.isPlayerWhite)
                {
                    return false; // Si une pièce alliée est présente sur la cible, le déplacement est invalide
                }
            }

            return true; // Le mouvement est valide
        }

        return false; // Le mouvement est invalide si le roi se déplace de plus d'une case
    }

    // Déplace le roi vers la position cible
    void MoveKing()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            Debug.Log(gameObject.name + " a atteint sa destination.");

            DeselectPiece();
            GameManager.SwitchTurn();
        }
    }

    // Méthode appelée lors du clic sur le roi
    public void OnKingClicked()
    {
        // Vérifie si c'est le tour du joueur auquel appartient cette pièce
        if (isPlayerWhite != GameManager.IsWhiteTurn)
        {
            Debug.Log("Ce n'est pas le tour de ce joueur !");
            return; // Ne rien faire si ce n'est pas le tour du joueur
        }

        isSelected = !isSelected; // Alterner l'état sélectionné
        Debug.Log(isSelected ? gameObject.name + " a été sélectionné." : gameObject.name + " a été désélectionné.");
    }

    // Méthode pour désélectionner le roi
    public void DeselectPiece()
    {
        isSelected = false;
        Debug.Log(gameObject.name + " a été désélectionné.");
    }

    // Vérifie si le roi peut encore bouger
    bool CanKingMove()
    {
        Vector3 currentPosition = transform.position;

        // Vérifie toutes les cases adjacentes du roi (une case dans chaque direction)
        Vector3[] directions = {
            new Vector3(1, 0, 0), new Vector3(-1, 0, 0), // Horizontale
            new Vector3(0, 0, 1), new Vector3(0, 0, -1), // Verticale
            new Vector3(1, 0, 1), new Vector3(1, 0, -1), // Diagonales
            new Vector3(-1, 0, 1), new Vector3(-1, 0, -1)
        };

        // Parcours toutes les directions possibles
        foreach (Vector3 direction in directions)
        {
            Vector3 targetPosition = currentPosition + direction;
            Collider[] colliders = Physics.OverlapSphere(targetPosition, 0.3f); // Vérifie la case cible

            bool isBlocked = false;

            foreach (Collider collider in colliders)
            {
                PieceController piece = collider.GetComponent<PieceController>();
                if (piece != null)
                {
                    if (piece.isPlayerWhite == this.isPlayerWhite)
                    {
                        isBlocked = true; // Si la case est occupée par une pièce alliée, le mouvement est bloqué
                    }
                    else
                    {
                        // La case est occupée par une pièce ennemie, mais ce n'est pas un problème pour se déplacer
                    }
                }
            }

            if (!isBlocked)
            {
                return true; // Si une case adjacente est libre, le roi peut encore bouger
            }
        }

        return false; // Le roi ne peut plus bouger, fin de la partie
    }

    // Méthode pour terminer la partie en cas de défaite
    void EndGame(string message)
    {
        Debug.Log(message);
        // Ajouter ici la logique pour afficher un message de fin de partie et arrêter le jeu
    }

    // Méthode pour gérer la capture du Roi
    void OnKingCaptured()
    {
        // La partie est terminée si le roi est capturé
        EndGame("Perdu ! Le roi a été capturé !");
    }

    // On suppose que le Roi est un GameObject de type PieceController, donc on l'appelle au moment de sa destruction
    private void OnDestroy()
    {
        if (isPlayerWhite == GameManager.IsWhiteTurn)
        {
            // Si le roi de l'adversaire est détruit, la partie est terminée et l'autre joueur gagne
            EndGame("Vous avez perdu ! Le roi a été capturé.");
        }
    }
}
