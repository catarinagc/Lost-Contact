using UnityEngine;
using System.Collections;

public class SlowMoveRoom : MonoBehaviour
{
    private Player player;

    [SerializeField] GameManager timerManager;
    [SerializeField] GameObject speedInfoUI;

    [SerializeField] private float penaltyCooldown = 2f;

    private bool canPenalty = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            speedInfoUI.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!player.getIsCrouching() && canPenalty)
            {
                Debug.Log("TOO FAST");

                timerManager.ErrorPenalty();

                StartCoroutine(PenaltyCooldown());
            }
        }
    }

    private IEnumerator PenaltyCooldown()
    {
        canPenalty = false;

        yield return new WaitForSeconds(penaltyCooldown);

        canPenalty = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            speedInfoUI.SetActive(false);
        }
    }
}