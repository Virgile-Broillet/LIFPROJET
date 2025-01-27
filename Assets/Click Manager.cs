using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public LayerMask clickableLayers; // Masque pour spécifier les couches cliquables (pièces)

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Détection d’un clic gauche
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Effectuer un raycast avec un filtre sur les couches cliquables
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayers))
            {
                // Vérifier si l'objet cliqué a un script de mouvement
				PawnController pawnMovement = hit.collider.GetComponent<PawnController>();
				if (pawnMovement != null)
				{
					Debug.Log("Pion cliqué, appel du script de mouvement.");
					pawnMovement.OnPawnClicked(); // Appeler la méthode du pion
				}  
				else
				{
					Debug.Log("Erreur");
					Debug.Log(pawnMovement);
				}              
            }
            else
            {
                Debug.Log("Aucun objet cliquable détecté !");
            }
        }
    }
}

