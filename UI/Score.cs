using System.Collections;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
	public TextMeshProUGUI scoreTextMesh;
	public TextMeshProUGUI plusPointTextMesh;
	public float displayDuration = 1f;
    private int score = 0;

	[Header("Points")]
	public int ringValue = 5;
	public int coinValue = 6;
	public int PowerUpValue = 15;

	private void Start()
	{
		scoreTextMesh.SetText(score.ToString());
	}

	public void UpdateScore(float multiplier)
	{
        score += (int)(ringValue * multiplier);
		scoreTextMesh.SetText(score.ToString());
	}

	public void AddPoints(bool PowerUp)
	{
		int value;

        if (PowerUp)
			value = PowerUpValue;
		else
			value = coinValue;
		plusPointTextMesh.SetText("+" + value.ToString());
		plusPointTextMesh.gameObject.SetActive(true);
		score += value;
		UpdateScore(1);
		StartCoroutine(HideTextAfterDelay(displayDuration));
    }

	public void EndScore()
    {
        scoreTextMesh.fontSize = 100;
        if (UnityEngine.ColorUtility.TryParseHtmlString("#FF66B7", out UnityEngine.Color color))
        {
            scoreTextMesh.color = color;
        }
    }

	IEnumerator HideTextAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		plusPointTextMesh.gameObject.SetActive(false);
	}
}
