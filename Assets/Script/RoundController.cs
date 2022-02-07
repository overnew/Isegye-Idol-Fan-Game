using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundController :MonoBehaviour
{
    private int roundCounter = 0;
    private Text roundText;

    void Awake()
    {
        roundText = GameObject.Find("roundNumber").GetComponent<Text>();

        roundCounter = 0; 
        roundText.text = roundCounter.ToString();
    }

    public void ChangeRound()
    {
        ++roundCounter;
        roundText.text = roundCounter.ToString();
        StartCoroutine(RoundChangeEffectCoroutine());
    }

    private IEnumerator RoundChangeEffectCoroutine()
    {
        int originSize = roundText.fontSize;
        int largeSize = 25 + originSize;

        roundText.fontSize = largeSize;
        while (roundText.fontSize > originSize)
        {
            roundText.fontSize = roundText.fontSize - 1;
            yield return new WaitForSeconds(0.02f);
        }
    }

    internal int GetRoundCounter() { return this.roundCounter; }
}
