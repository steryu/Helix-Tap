using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum Fruit
{
    Solid,
    Watermelon,
    Orange,
    Gomu // powerUp
}

public class Player : MonoBehaviour
{
    Rigidbody rb;
    private Renderer ballRenderer;
    private Score score;
    private bool isGameEnd = false;

    public ScreenOverlayController overlayController;
    public Fruit fruitColor;

    [Header("Ball Settings")]
    public float bounceF;
    public float fallForce;
    public float gravityStrength = 15f;
    public float gravityStrengthPoweredUp = 20f;
    public float accelerationRate = 1f;
    public float accelerationPointsMultiplier = 1.4f;

    private float accelerationFactor;
    private float startTime = 0f;
    private readonly float torqueForce = 10f;

    [Header("Fruits")]
    public GameObject watermelonChild;
    public GameObject orangeChild;
    public GameObject gomuChild;

    [Header("PowerUp")]
    public float obstacleScoreMuliplier = 1f;
    public float powerUpTime = 2f;

    private Fruit previousFruitColor = Fruit.Solid;
    private bool isPoweredUp = false;

    [Header("Cooldown")]
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private readonly float zebounceCooldown = 0.05f;

    private float lowestBouncePositionY = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();
        if (!GameObject.Find("Canvas").TryGetComponent<Score>(out score))
        {
            Debug.Log("no Score GameObject");
        }
        fruitColor = (Fruit)Random.Range(1, System.Enum.GetValues(typeof(Fruit)).Length - 1);
        ApplyColor();
    }

    private void FixedUpdate()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.fixedDeltaTime;
            if (cooldownTimer <= 0f)
                isOnCooldown = false;

        }
        if (rb.isKinematic == true)
        {
            if (startTime == 0f)
            {
                startTime = Time.time;
            }
            float accelerationTime = Time.time - startTime;
            float strength;
            if (isPoweredUp)
                strength = gravityStrengthPoweredUp;
            else
                strength = gravityStrength;
            accelerationFactor = 1f + accelerationRate * accelerationTime;
            Vector3 gravity = strength * accelerationFactor * Time.deltaTime * Vector3.down;
            transform.position += gravity;
        }
        else if (rb.isKinematic == false) // reseting the acceleration
        {
            accelerationFactor = 1f;
            startTime = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp"))
        {
            PickUp pickup = other.GetComponent<PickUp>();
            if (pickup.Collect(isPoweredUp) == 1)
                PowerUp();
        }
        else if (other.transform.parent != null && other.transform.parent.GetComponent<Ring>())
        {
            Ring ring = other.transform.parent.gameObject.GetComponent<Ring>();
            if (ring.GetIsObstaclesOn() == true)
                obstacleScoreMuliplier = 1.5f;
            if (other.transform.gameObject.TryGetComponent<RingPart>(out var part))
            {
               if (part.isSolid && fruitColor != Fruit.Gomu) 
               {
                    BounceOnRing(ring, other);
                    return;
               }
            }
            if (fruitColor == ring.fruit || (fruitColor == Fruit.Gomu && ring.fruit != Fruit.Solid))
            {
                FallThroughRing(ring);
            }
            else if (fruitColor != ring.fruit || ring.fruit == Fruit.Solid)
            {
                BounceOnRing(ring, other);
            }
        }
        SetCooldown();
    }

    private void FallThroughRing(Ring ring)
    {
        rb.isKinematic = true;
        ring.DestroyRing();
        if (!isOnCooldown)
            score.UpdateScore(accelerationFactor * accelerationPointsMultiplier * obstacleScoreMuliplier);
    }

    private void BounceOnRing(Ring ring, Collider other)
    {
        rb.isKinematic = false;
        if (ring.onFirstHit == false)
        {
            lowestBouncePositionY = transform.position.y;
            rb.velocity = new(rb.velocity.x, bounceF * Time.deltaTime, rb.velocity.z);
            ring.onFirstHit = true;
        }
        rb.velocity = new(rb.velocity.x, bounceF * Time.deltaTime, rb.velocity.z);
        if (transform.position.y < lowestBouncePositionY)
            transform.position = new Vector3(transform.position.x, lowestBouncePositionY, transform.position.z);

        ApplyRotation();
        SplashSettings fruit = GetComponentInChildren<SplashSettings>();
        if (fruit != null)
            fruit.CreateSplash(other);
        if (other == ring.isLastRing)
        {
            if (isGameEnd)
                return;
            overlayController.GameEnd();
            score.EndScore();
            isGameEnd = true;
        }
    }

    private void ApplyRotation()
    {
        Vector3 torque = new(
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce)
            );
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    public void ApplyColor()
    {
        if (ballRenderer != null)
        {
            switch (fruitColor)
            {
                case Fruit.Watermelon:
                    watermelonChild.SetActive(true);
                    orangeChild.SetActive(false);
                    gomuChild.SetActive(false);
                    break;
                case Fruit.Orange:
                    watermelonChild.SetActive(false);
                    orangeChild.SetActive(true);
                    gomuChild.SetActive(false);
                    break;
                case Fruit.Gomu:
                    watermelonChild.SetActive(false);
                    orangeChild.SetActive(false);
                    gomuChild.SetActive(true);
                    break;
            }
        }
    }
    public void ToggleColor()
    {
        if (fruitColor == Fruit.Gomu)
            return;
        if (fruitColor == Fruit.Watermelon)
        {
            fruitColor = Fruit.Orange;
        }
        else
        {
            fruitColor = Fruit.Watermelon;
        }
        ApplyColor();
    }

    public void SetFruitColor(Fruit newColor)
    {
        fruitColor = newColor;
        ApplyColor();
    }

    private void PowerUp()
    {
        if (isPoweredUp)
            return ;
        isPoweredUp = true;
        previousFruitColor = fruitColor;
        SetFruitColor(Fruit.Gomu);

        StartCoroutine(ChangeToPreviousFruitColor(previousFruitColor));
    }

    private IEnumerator ChangeToPreviousFruitColor(Fruit previousFruitColor)
    {
        yield return new WaitForSeconds(powerUpTime);

        SetFruitColor(previousFruitColor);
        isPoweredUp = false;
    }

    public void MoveHorizontally(int direction)
    {
        Vector3 newPos = new(transform.position.x + (2.8f * direction), transform.position.y, transform.position.z);
        if (newPos.x > -3 && newPos.x < 3)
        {
            transform.position = newPos;
        }
    }
    private void SetCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = zebounceCooldown;
    }
}
