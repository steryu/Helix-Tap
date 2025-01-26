using UnityEngine;

/* Generating a helix wit a cumstom amount of rings and distance */

public class HelixManager : MonoBehaviour
{
    public GameObject[] rings;
    public int nbOfRings;
    public float ringDistance = 5f;
    float yPosition;

    private bool isObstaclesOn = false;

    private void Start()
    {
        transform.rotation = Quaternion.identity;
        yPosition = 0;
        InstantiateTower();
    }

    void InstantiateTower()
    {
        for (int i = 0; i < nbOfRings; i++)
        {
            if (i == 0)
                GenerateTower(0); // first ring
            else
            {
                GenerateTower(Random.Range(1, rings.Length - 1)); // middle rings
            }
        }
        GenerateTower(rings.Length - 1); // last ring
    }

    void GenerateTower(int index)
    {
        Vector3 ringPos = new(transform.position.x, yPosition, transform.position.z);
        GameObject newRing = Instantiate(rings[index], ringPos, Quaternion.identity);
        yPosition -= ringDistance;
        newRing.transform.parent = transform;
        newRing.GetComponent<Ring>().SetObstacles(isObstaclesOn);
    }
    
    public void Regenerate()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        isObstaclesOn = !isObstaclesOn;
        Start();
    }
}