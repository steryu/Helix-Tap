using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Creating a splash with smaller splashed that go up and outward */

public class SplashSettings : MonoBehaviour
{
    public GameObject splashPrefab;
	public GameObject smallSplashPrefab;
    private readonly List<GameObject> splashes = new List<GameObject>();
	private readonly int maxSplashes = 10;

    public void CreateSplash(Collider other)
	{
		Vector3 splashPosition = new(transform.position.x, other.transform.position.y - 0.5f, transform.position.z);
		Quaternion rotation = Quaternion.Euler(90, 0, 0);
		GameObject newSplash = Instantiate(splashPrefab, splashPosition, rotation);
		newSplash.transform.parent = other.transform;
        splashes.Add(newSplash);

        if (splashes.Count > maxSplashes) // remove splashes when there are too many on screen
        {
            Destroy(splashes[0]);
            splashes.RemoveAt(0);
        }

        GameObject splashParent = new("SplashParent"); // to organize all the splashes in a gameobject
		if (smallSplashPrefab != null)
		{
            Vector3 smallSplashPosition = splashPosition + Random.insideUnitSphere * 0.5f;
            GameObject SmallSplashes = Instantiate(smallSplashPrefab, smallSplashPosition, Quaternion.identity, splashParent.transform);
            Destroy(SmallSplashes, 1f);
		}
		Destroy(splashParent, 1f);
	}
}
