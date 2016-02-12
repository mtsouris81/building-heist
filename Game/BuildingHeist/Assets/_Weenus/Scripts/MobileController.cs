using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GiveUp.Core;

public class MobileController : MonoBehaviour {

    public ScreenButton ButtonMoveUp = null;
    public ScreenButton ButtonLookUp = null;
    public ScreenButton ShootButton = null;
    public ScreenButton JumpButton = null;
    public RectTransform ButtonsContainer = null;



    public float maxTurnMultiplier = 0.75f;
    public float minTurnMultiplier = 0.15f;
    public float lowDistanceRange = 40;
    public float highDistanceRange = 105;


	private float ButtonDeadZoneRadius = 5f;


	Text _buttonText = null;
    Vector2? movementVector = null;
    Vector2? lookVector = null;
    Vector3 moveButtonCenter;
    Vector3 lookButtonCenter;
	RectTransform _moveRect;
	RectTransform _lookRect;
	Touch leftTouch;
	Touch rightTouch;
	Vector3 positionOverMoveButton;
	Vector3 positionOverLookButton;

    public MobileControllerMode Mode = MobileControllerMode.Desktop;

    public enum MobileControllerMode
    {
        Desktop,
        TouchDevice,
        DesktopOnly
    }

	public void Start () {


        if (!Application.isEditor)
        {
#if UNITY_STANDALONE
            // force to desktop only
            this.Mode = MobileControllerMode.DesktopOnly;
#else
            // force to mobile
            this.Mode = MobileControllerMode.TouchDevice;
#endif
        }

        ButtonMoveUp.MouseDownCallback = delegate()
        {
            movementVector = new Vector2(0, 1);
        };

        ButtonLookUp.MouseDownCallback = delegate()
        {
            lookVector = new Vector2(0, 1);
        };

        ButtonMoveUp.MouseUpCallback = CancelMove;
        ButtonLookUp.MouseUpCallback = CancelLook;


        ShootButton.MouseDownCallback = delegate()
        {
            PlayerUtility.Current.SendMessage("Shoot");
        };


        JumpButton.MouseDownCallback = delegate()
        {
            PlayerUtility.Hero.Jump();
        };

		_moveRect = ButtonMoveUp.GetComponent<RectTransform>();
		_lookRect = ButtonLookUp.GetComponent<RectTransform>();
		_buttonText = ButtonMoveUp.GetComponentInChildren<Text> ();

        if (this.Mode == MobileControllerMode.DesktopOnly)
        {
           // var mouseLook = PlayerUtility.Hero.GetComponent<WeenusSoft.MouseLook>();
            //if (mouseLook != null)
            //{
            //    mouseLook.SuspendMouse = false;
            //    //this.gameObject.SetActive(false);
            //    ButtonsContainer.gameObject.SetActive(false);
            //}
        }
	}


	public void Update () 
	{

        if (this.Mode == MobileControllerMode.DesktopOnly)
        {
            //this.gameObject.SetActive(false);
            return;
        }
		
		positionOverMoveButton = Vector3.zero;
		positionOverLookButton = Vector3.zero;

        if (Mode == MobileControllerMode.TouchDevice)
        {
            if (Input.touchCount > 0)
            {
                GetTouches();
                if (leftTouch.position != Vector2.zero)
                {
                    positionOverMoveButton = new Vector3(leftTouch.position.x, Screen.height - leftTouch.position.y);
                }

                if (rightTouch.position != Vector2.zero)
                {
                    positionOverLookButton = new Vector3(rightTouch.position.x, Screen.height - rightTouch.position.y);
                }
            }
        }

        if (Mode == MobileControllerMode.Desktop) 
		{
			positionOverMoveButton = new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
			positionOverLookButton = new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		}

		if (positionOverMoveButton == Vector3.zero && positionOverLookButton == Vector3.zero) 
		{
			return;
		}

		if (movementVector.HasValue && positionOverMoveButton != Vector3.zero)
		{
			DetermineMoveInput();
        }

		if (lookVector.HasValue && positionOverLookButton != Vector3.zero)
		{
			DetermineLookInput();
        }
	}

	

	public void DetermineMoveInput()
    {
        moveButtonCenter = new Vector3(_moveRect.position.x, Screen.height - _moveRect.position.y);
		Vector3 normalizedVec = Vector3.Normalize(positionOverMoveButton - moveButtonCenter);
		movementVector = new Vector2(normalizedVec.x, -normalizedVec.y);

        bool isRunning = DeterminePlayerRunning(positionOverMoveButton);
        PlayerUtility.Hero.Controller.SetRunning(isRunning);
		PlayerUtility.Hero.Controller.SetMovementOverride(movementVector.Value);
	}

    private bool DeterminePlayerRunning(Vector3 positionOverMoveButton)
    {
        float height = Screen.height;
        float threshold = height * 0.3f;
        float y = positionOverMoveButton.y;

        if (y < 0)
            y = -y;

        return y < threshold;
    }
	public void DetermineLookInput()
    {
        lookButtonCenter = new Vector3(_lookRect.position.x, Screen.height - _lookRect.position.y);

        float x = _lookRect.rect.xMin + _lookRect.rect.width / 2;
        float y = _lookRect.rect.yMin + _lookRect.rect.height / 2;

        float distance = Vector3.Distance(positionOverLookButton, lookButtonCenter);

        float realRange = highDistanceRange - lowDistanceRange;

        distance -= lowDistanceRange;
        distance = Mathf.Clamp(distance, 0.01f, realRange);

        float rangeRatio = distance / realRange;


        float finalTurnMultiplier = Mathf.Lerp(minTurnMultiplier, maxTurnMultiplier, rangeRatio);




		Vector3 normalizedVec = Vector3.Normalize(positionOverLookButton - lookButtonCenter);
		lookVector = new Vector2(normalizedVec.x, -normalizedVec.y);
        PlayerUtility.Current.SendMessage("SetLookOverride", (lookVector.Value * finalTurnMultiplier));
	}

	
	public void GetTouches()
	{
		int widthHalf = Screen.width / 2;
				
		if (Input.touchCount == 1) 
		{
			var t = Input.GetTouch(0);
			if (t.position.x > widthHalf)
			{
				rightTouch = t;
			}
			else
			{
				leftTouch = t;
			}
		}
		
		if (Input.touchCount > 1) 
		{
			// get the two touches that are closest to the buttons and assign left and right
			// ignore all other touches
			float lowestDistance = float.MaxValue;
			moveButtonCenter = new Vector3(_moveRect.anchoredPosition.x, Screen.height - _moveRect.anchoredPosition.y);
			foreach(var t in Input.touches)
			{
				float dist = Vector2.Distance(moveButtonCenter, t.position);
				if (dist < lowestDistance && dist > ButtonDeadZoneRadius)
				{
					lowestDistance = dist;
					leftTouch = t;
				}
			}


			lowestDistance = float.MaxValue;
			lookButtonCenter = new Vector3(Screen.width + _lookRect.anchoredPosition.x, Screen.height - _lookRect.anchoredPosition.y);
			foreach(var t in Input.touches)
			{
				float dist = Vector2.Distance(lookButtonCenter, t.position);
				if (dist < lowestDistance && dist > ButtonDeadZoneRadius)
				{
					lowestDistance = dist;
					rightTouch = t;
				}
			}
		}
	}
	public void CancelMove()
	{
		movementVector = null;
	}
	public void CancelLook()
	{
		lookVector = null;
	}

    public void CancelInputs()
    {
        CancelMove();
        CancelLook();
    }
}
