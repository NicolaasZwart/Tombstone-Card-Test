using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDestroyer : MonoBehaviour
{
    void OnEnable()
    {
        HandBuilder.DestroyOldCards += KillCard;
    }

    void KillCard()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        HandBuilder.DestroyOldCards -= KillCard;
    }
}
