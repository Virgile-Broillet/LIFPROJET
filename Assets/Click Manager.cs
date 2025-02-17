using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public LayerMask clickableLayers; // Masque pour spécifier les couches cliquables (pièces)

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Détection d’un clic gauche
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Effectuer un raycast avec un filtre sur les couches cliquables
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayers))
            {
                Debug.Log("Raycast hit detected: " + hit.collider.gameObject.name);

                // Vérifier si l'objet cliqué a un composant de mouvement spécifique (pion, tour, etc.)
                if (hit.collider.TryGetComponent(out PawnController pawn))
                {
                    Debug.Log("Pion cliqué, appel du script de mouvement.");
                    pawn.OnPawnClicked(); // Appeler la méthode du pion
                }
                else if (hit.collider.TryGetComponent(out RookController rook))
                {
                    Debug.Log("Tour cliquée, appel du script de mouvement.");
                    rook.OnRookClicked(); // Appeler la méthode de la tour
                }
                else if (hit.collider.TryGetComponent(out BishopController bishop))
                {
                    Debug.Log("Fou cliqué, appel du script de mouvement.");
                    bishop.OnBishopClicked(); // Appeler la méthode du fou
                }
                else if (hit.collider.TryGetComponent(out KnightController knight))
                {
                    Debug.Log("Cavalier cliqué, appel du script de mouvement.");
                    knight.OnKnightClicked(); // Appeler la méthode du cavalier
                }
                else if (hit.collider.TryGetComponent(out KingController king))
                {
                    Debug.Log("Roi cliqué, appel du script de mouvement.");
                    king.OnKingClicked();
                }
                else
                {
                    Debug.Log("Aucune pièce ou pièce inconnue détectée.");
                }
            }
            else
            {
                Debug.Log("Aucun objet cliquable détecté !");
            }
        }
    }
}
