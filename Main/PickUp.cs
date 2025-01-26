using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PickUp : MonoBehaviour
{

    public bool isCoin = false;
    public bool isPowerUp = false;
    public float powerUpTime = 2f;
    public GameObject explosionEffectprefab;

    private Score score;

    private void Start()
    {
        if (!GameObject.Find("Canvas").TryGetComponent(out score))
        {
            Debug.Log("no score script found");
        }
        if (explosionEffectprefab == null)
        {
            Debug.Log("no particle system");
            return;
        }
    }

    public int Collect(bool isPoweredUp)
    {
        GameObject Explosion = Instantiate(explosionEffectprefab, transform.position, Quaternion.identity);
        if (isCoin)
        {
            Destroy(gameObject);
            score.AddPoints(false);
            Destroy(Explosion, 1f);
            return 0;
        }
        else if (isPowerUp)
        {
            Destroy(gameObject);
            Destroy(Explosion, 1f);
            if (isPoweredUp)
            {
                score.AddPoints(true);
            }
            return 1;
        }
        return 0;
    }
}
