using UnityEngine;

/* Touch movement for PC and Android. Tap, swipe left, right and up */

public class TouchMovement : MonoBehaviour
{
	Player player;
	public bool isTitleScreen = true;
	public float swipeThreshold = 70;

	private Vector2 startPosition;

    [Header("Swipe Down Overlay")]
    public GameObject swipeDown;

	void Start()
	{
		player = GetComponent<Player>();
	}

	// checking for a single touch or swipe based on the distance between button down and up
	private void Update()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
		{
			startPosition = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp(0))
		{
			Vector2 endPosition = Input.mousePosition;
			Vector2 swipeDirection = endPosition - startPosition;
			DetectSwipe(swipeDirection);
		}
#elif UNITY_ANDROID
		if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startPosition = touch.position;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                Vector2 endPosition = touch.position;
                Vector2 swipeDirection = endPosition - startPosition;
                DetectSwipe(swipeDirection);
            }
        }
#endif
    }

    private void DetectSwipe(Vector2 swipeDirection)
	{
		if (swipeDirection.magnitude > swipeThreshold && !isTitleScreen)
		{
			if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
			{
				if (swipeDirection.x > 0) // swipe to the left or right
				{
					player.MoveHorizontally(1);
				}
				else
				{
					player.MoveHorizontally(-1);
				}
			}
			else 
			{
				if (swipeDirection.y < 0) // swipe down
				{
					Ring ring = GameObject.FindWithTag("Ring").GetComponent<Ring>();
					if (ring != null && ring.isFirstRing)
					{
						ring.DestroyRing();
						swipeDown.SetActive(false);
					}
				}
			}
		}
		else // single tap
		{
			player.ToggleColor();
		}
	}
}
