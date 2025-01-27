using UnityEngine;

public class PawnController : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de déplacement du pion
    private bool isSelected = false; // État de sélection du pion
    private Vector3 targetPosition; // Position de destination du pion
    private bool isMoving = false; // Si le pion est en train de se déplacer

    private void Update()
    {
        // Si le pion est sélectionné, on permet de déplacer le pion avec la souris
        if (isSelected && !isMoving)
        {
            HandleMouseClick();
        }

        // Si le pion est en mouvement, on le déplace vers la position cible
        if (isMoving)
        {
            MovePawn();
        }
    }

    // Gère le mouvement avec la souris : cliquer pour se déplacer
    void HandleMouseClick()
    {
        // Si le clic gauche est effectué
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Lancer un raycast depuis la position de la souris
            if (Physics.Raycast(ray, out hit))
            {
                // Vérifie si on clique sur un sol ou une case où le pion peut se déplacer
                if (hit.collider.CompareTag("Tiles")) // Assurez-vous que le sol ou la case est taggé "Tiles"
                {
                    targetPosition = hit.point; // Mettre à jour la position cible
                    isMoving = true; // Commencer le déplacement
                }
            }
        }
    }

    // Déplacer le pion vers la position cible
    void MovePawn()
    {
        // Déplace le pion vers la position cible en douceur
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Si le pion a atteint la position cible, arrêter le mouvement
        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }

    // Cette méthode est appelée lorsque le pion est cliqué
    public void OnPawnClicked()
    {
        if (!isSelected)
        {
            isSelected = true;
            Debug.Log(gameObject.name + " a été sélectionné.");
        }
        else
        {
            isSelected = false;
            Debug.Log(gameObject.name + " a été désélectionné.");
        }
    }

    // Méthode pour désélectionner le pion
    public void DeselectPawn()
    {
        isSelected = false;
        Debug.Log(gameObject.name + " a été désélectionné.");
    }
}
