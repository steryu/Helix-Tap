using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Instantiating a ring with a number of triangles that minimize when destroyed */

public class Ring : MonoBehaviour
{
	public GameObject trianglePrefab;
	public GameObject SolidtrianglePrefab;
	public Fruit fruit;
	public bool isFirstRing;
	public bool isLastRing;

	[Header("Triangle Settings")]
	public float minimizeDuration = 0.5f;
	public float delayBetweenParts = 0.1f;
	public int numberOfTriangles = 8;
	public float radius = 0;
    private readonly float yOffset = 5.4f;
	private readonly float yRotationOffset = -20f;
	private GameObject lastPrefab = null;

	[Header("Obstacle Settings")]
    [Range(0, 1)] public float obstacleChance = 0.01f;
	private bool isObstaclesOn = true;

    [Header("PickUps")]
	public GameObject coinPrefab;
	public GameObject powerUpPrefab;
	[Range(0, 1)] public float coinSpawnRate = 0.4f;
	[Range(0, 1)] public float powerUpSpawnRate = 0.05f;
	private readonly Vector3[] pickUpPositions = new Vector3[]
	{
		new(-2.75f, 0, -3.22f),
		new(0, 0, -3.25f),
		new(2.81f, 0, -3.3f)
	};

    [Header("Bounce")]
    public bool onFirstHit;

    void Start()
	{
		ArrangeTrianglesInCircle();
    }

	void ArrangeTrianglesInCircle()
	{
		float angleStep = 360f / numberOfTriangles;

		for (int i = 0; i < numberOfTriangles; i++)
		{
			float angle = i * angleStep;
			float radians = angle * Mathf.Deg2Rad;

			float x = Mathf.Cos(radians) * radius;
			float z = Mathf.Sin(radians) * radius;

			Vector3 position = new(x, yOffset, z);
			Quaternion rotation = Quaternion.Euler(0, -angle + yRotationOffset, 0);

			if (trianglePrefab)
			{
				GameObject prefab = trianglePrefab;
				if (isObstaclesOn && Random.value < obstacleChance && SolidtrianglePrefab != null)
				{
					if (lastPrefab != SolidtrianglePrefab)
						prefab = SolidtrianglePrefab;
				}
				GameObject triangle = Instantiate(prefab, position, rotation);
				triangle.transform.SetParent(transform, false);  // set the parent to organize hierarchy
				lastPrefab = prefab;
			}
		}
		SetPickUps();
	}

	public void DestroyRing()
	{
		StartCoroutine(Minimize());
	}
	private IEnumerator Minimize()
	{
		foreach (Transform part in transform)
		{
			StartCoroutine(MinimizePart(part));
			yield return new WaitForSeconds(delayBetweenParts);
		}
		yield return new WaitForSeconds(minimizeDuration);
		Destroy(gameObject);
	}

	private IEnumerator MinimizePart(Transform part)
	{
		Vector3 originalScale = part.transform.localScale;
		Vector3 targetScale = new(0, originalScale.y, 0);
		float elaspedTime = 0;

		while (elaspedTime < minimizeDuration)
		{
			part.transform.localScale = Vector3.Lerp(originalScale, targetScale, elaspedTime / minimizeDuration);
			elaspedTime += Time.deltaTime;
			yield return null;
		}
		part.transform.localScale = targetScale;
	}

    private void SetPickUps()
    {
        if (Random.value < powerUpSpawnRate && powerUpPrefab != null)
        {
            int randomIndex = Random.Range(0, pickUpPositions.Length);
            Vector3 selectedPosition = pickUpPositions[randomIndex];
            Vector3 powerUpPostion = new(selectedPosition.x, transform.position.y + 6f, selectedPosition.z);
            GameObject powerUpInstance = Instantiate(powerUpPrefab, powerUpPostion, powerUpPrefab.transform.rotation);
            powerUpInstance.transform.parent = transform;
        }
        else if (Random.value < coinSpawnRate && coinPrefab != null)
        {
            int randomIndex = Random.Range(0, pickUpPositions.Length);
            Vector3 selectedPosition = pickUpPositions[randomIndex];
            Vector3 coinPostion = new(selectedPosition.x, transform.position.y + 6f, selectedPosition.z);
            GameObject coinInstance = Instantiate(coinPrefab, coinPostion, coinPrefab.transform.rotation);
            coinInstance.transform.parent = transform;
        }
    }

	public void SetObstacles(bool value)
	{
		isObstaclesOn = value;
	}

	public bool GetIsObstaclesOn()
	{
		return isObstaclesOn;
	}
}
