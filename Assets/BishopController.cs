using UnityEngine;

public class BishopController : PieceController
{
    public float moveSpeed = 5f; // Vitesse de déplacement
    private bool isSelected = false; // État de sélection
    private Vector3 targetPosition; // Position de destination
    private bool isMoving = false; // Si le fou est en train de se déplacer
    private PieceController pieceToCapture = null; // Pièce à capturer

    private void Update()
    {
        // Si le fou est sélectionné, on permet de le déplacer
        if (isSelected && !isMoving)
        {
            HandleMouseClick();
        }

        // Si le fou est en mouvement, on le déplace
        if (isMoving)
        {
            MoveBishop();
        }
    }

    // Gère le clic souris pour déplacer le fou
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

                    // Vérifie si le mouvement est valide pour le fou
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

        // Le fou se déplace en diagonale : |X - targetX| == |Z - targetZ|
        if (Mathf.Abs(targetX - currentX) == Mathf.Abs(targetZ - currentZ))
        {
            //verifie si le chemin ets dégagé
            if (IsPathClear(currentPosition, hitPosition))
            {
                return true;
            }
            Debug.Log("La case de destination ou une case sur le parcours est occupée par une pièce.");
            return false;
        }

        Debug.Log("Déplacement impossible.");
        return false; // Le mouvement n'est pas valide
    }

    // Vérifie si le chemin entre la position actuelle et la position cible est dégagé
    bool IsPathClear(Vector3 currentPosition, Vector3 targetPosition)
    {
        int currentX = Mathf.RoundToInt(currentPosition.x);
        int currentZ = Mathf.RoundToInt(currentPosition.z);
        int targetX = Mathf.RoundToInt(targetPosition.x);
        int targetZ = Mathf.RoundToInt(targetPosition.z);

        // Vérifie que le déplacement est en diagonale
        if (Mathf.Abs(targetX - currentX) != Mathf.Abs(targetZ - currentZ))
        {
            return false; // Mouvement non diagonal, donc invalide
        }

        // Détermine la direction du déplacement (+1 ou -1)
        int dx = (targetX > currentX) ? 1 : -1;
        int dz = (targetZ > currentZ) ? 1 : -1;

        // On commence juste après la position actuelle
        int x = currentX + dx;
        int z = currentZ + dz;

        while (x != targetX || z != targetZ)
        {
            Vector3 checkPosition = new Vector3(x, currentPosition.y, z);
            Collider[] colliders = Physics.OverlapSphere(checkPosition, 0.3f); // Augmente le rayon

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out PieceController piece))
                {
                    return false;
                }
            }

            x += dx;
            z += dz;
        }

        // Vérifie la dernière case (cible) pour une capture éventuelle
        Collider[] finalColliders = Physics.OverlapSphere(targetPosition, 0.3f);
        foreach (Collider collider in finalColliders)
        {
            PieceController piece = collider.GetComponent<PieceController>();
            if (piece != null && piece.isPlayerWhite == this.isPlayerWhite)
            {
                return false; // Impossible d'atterrir sur une pièce alliée
            }
        }

        return true; // Déplacement valide
    }


    // Déplace le fou vers la position cible
    void MoveBishop()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            Debug.Log(gameObject.name + " a atteint sa destination.");

            // Vérifie s'il y a une pièce ennemie à capturer
            Collider[] colliders = Physics.OverlapSphere(targetPosition, 0.3f);
            foreach (Collider collider in colliders)
            {
                PieceController piece = collider.GetComponent<PieceController>();
                if (piece != null && piece.isPlayerWhite != this.isPlayerWhite)
                {
                    pieceToCapture = piece;
                    Debug.Log("Pièce ennemie détectée pour capture : " + piece.gameObject.name);
                    Destroy(piece.gameObject); // Capture la pièce
                    break;
                }
            }

            DeselectPiece();
            GameManager.SwitchTurn();
        }
    }

    // Méthode appelée lors du clic sur le fou
    public void OnBishopClicked()
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

    // Méthode pour désélectionner le fou
    public void DeselectPiece()
    {
        isSelected = false;
        Debug.Log(gameObject.name + " a été désélectionné.");
    }
}
