using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using TouchScript.Hit;

[RequireComponent(typeof(ReleaseGesture))]
[RequireComponent(typeof(PressGesture))]
[RequireComponent(typeof(FlickGesture))]
[RequireComponent(typeof(MetaGesture))]
public class MovmentInput : MonoBehaviour
{
    const float XTHRESHHOLD = 1f;
    const float YTHRESHHOLD = 1f;

    const float MAXXSPEED = 6f;
    const float MAXYSPEED = 6f;

    const float MAXDASHPOWER = 7;
    const float FLICKCOOLDOWN = 0.5f;

    float flickCooldownTimer;

    bool joystickActive, flickReady;

    GameObject player;

    int fingerpresses;

    TouchHit hitJS;

    [SerializeField]
    GameObject joystickPos;

    Vector3 pressPoint, releasePoint;

    void Awake()
    {
        player = GameObject.Find("Player");
        joystickActive = false;
        fingerpresses = 0;
        flickCooldownTimer = FLICKCOOLDOWN;
    }

    void Update()
    {
        flickCooldownTimer += Time.deltaTime;
    }

    private void OnEnable()
    {
        GetComponent<PressGesture>().Pressed += PressHandler;
        GetComponent<ReleaseGesture>().Released += ReleaseHandler;
        GetComponent<FlickGesture>().Flicked += FlickHandler;
        GetComponent<MetaGesture>().TouchBegan += TouchStartHandler;
        GetComponent<MetaGesture>().TouchMoved+= TouchMoveHandler;
        GetComponent<MetaGesture>().TouchEnded += TouchEndHandler;
    }

    private void OnDisable()
    {
        GetComponent<PressGesture>().Pressed -= PressHandler;
        GetComponent<ReleaseGesture>().Released -= ReleaseHandler;
        GetComponent<FlickGesture>().Flicked -= FlickHandler;
        GetComponent<MetaGesture>().TouchBegan -= TouchStartHandler;
        GetComponent<MetaGesture>().TouchMoved -= TouchMoveHandler;
        GetComponent<MetaGesture>().TouchEnded -= TouchEndHandler;
    }

    void TouchStartHandler(object sender, System.EventArgs e)
    {        
        if (!joystickActive && fingerpresses == 0 && AnimationSetter.instance.state != MonsterState.Stun)
        {
            MetaGesture gesture = sender as MetaGesture;
            TouchHit hit;
            gesture.GetTargetHitResult(out hit);

            Vector3 modifiedPos = new Vector3(hit.Point.x, hit.Point.y, joystickPos.transform.position.z);

            joystickPos.transform.position = modifiedPos;

            joystickActive = true;
            flickReady = false;            
        }
        fingerpresses++;
    }

    void TouchMoveHandler(object sender, System.EventArgs e)
    {
        if (joystickActive && AnimationSetter.instance.state != MonsterState.Dash)
        {
            
            if (fingerpresses == 1)
            { 
                MetaGesture gesture = sender as MetaGesture;                
                gesture.GetTargetHitResult(out hitJS);
            }

            float x = 0;
            float y = 0;

            if (Camera.main.WorldToScreenPoint(hitJS.Point).x >= Camera.main.WorldToScreenPoint(joystickPos.transform.position).x + XTHRESHHOLD || Camera.main.WorldToScreenPoint(hitJS.Point).x < Camera.main.WorldToScreenPoint(joystickPos.transform.position).x - XTHRESHHOLD)
            {
                x = (hitJS.Point.x - joystickPos.transform.position.x) * MAXXSPEED;
                if (x > MAXXSPEED)
                {
                    x = MAXXSPEED;
                }
            }

            if (Camera.main.WorldToScreenPoint(hitJS.Point).y >= Camera.main.WorldToScreenPoint(joystickPos.transform.position).y + YTHRESHHOLD || Camera.main.WorldToScreenPoint(hitJS.Point).y < Camera.main.WorldToScreenPoint(joystickPos.transform.position).y - YTHRESHHOLD)
            {
                y = (hitJS.Point.y - joystickPos.transform.position.y) * MAXYSPEED;
                if (y > MAXYSPEED)
                {
                    y = MAXYSPEED;
                }
            }

            Vector3 moveVector = new Vector3(x, y, player.transform.position.z);
            moveVector.Normalize();
            //print(moveVector);

            player.GetComponent<MonsterController>().RecieveMovmentImput(moveVector);

            if (moveVector.x != 0 || moveVector.y != 0)
            {
                AnimationSetter.instance.SetMovement(true);
            }
            else
            {
                AnimationSetter.instance.SetMovement(false);
            }
        }
        flickReady = false;
    }

    void TouchEndHandler(object sender, System.EventArgs e)
    {        
        fingerpresses--;
        if (fingerpresses == 0)
        {
            player.GetComponent<MonsterController>().RecieveMovmentImput(Vector2.zero);
            AnimationSetter.instance.SetMovement(false);
            joystickActive = false;
        }
    }

    void FlickHandler(object sender, System.EventArgs e)
    {
        if (flickReady && flickCooldownTimer > FLICKCOOLDOWN && AnimationSetter.instance.state != MonsterState.Fall && AnimationSetter.instance.state != MonsterState.Dash)
        {
            FlickGesture gesture = sender as FlickGesture;
            TouchHit hitFlick;
            gesture.GetTargetHitResult(out hitFlick);
            
            Vector3 direction = hitFlick.Point - pressPoint;
            //print(direction.normalized.x + " , " + direction.normalized.y + " : " + direction.magnitude);
            if (direction.normalized.y < 0)
            {
                player.GetComponent<MonsterController>().SetDashPower(1);
            }
            else if (direction.magnitude > MAXDASHPOWER)
            {
                player.GetComponent<MonsterController>().SetDashPower(MAXDASHPOWER);
            }            
            else
            {
                player.GetComponent<MonsterController>().SetDashPower(direction.magnitude);
            }
            //print(direction.magnitude);
            direction.Normalize();
            //print(direction);

            player.GetComponent<MonsterController>().SetSwipeDirection(direction);

            AnimationSetter.instance.state = MonsterState.Dash;

            flickCooldownTimer = 0;
        }
    }

    void PressHandler(object sender, System.EventArgs e)
    {
        //print("Press Detected");
        player.GetComponent<MonsterController>().SetMonsterTapped(true);
        player.GetComponent<MonsterController>().RecieveMovmentImput(Vector2.zero);
        AnimationSetter.instance.SetMovement(false);

        PressGesture gesture = sender as PressGesture;
        TouchHit hitPress;
        gesture.GetTargetHitResult(out hitPress);
        pressPoint = hitPress.Point;
    }

    void ReleaseHandler(object sender, System.EventArgs e)
    {
        //print("Released");
        player.GetComponent<MonsterController>().RecieveMovmentImput(Vector2.zero);
        AnimationSetter.instance.SetMovement(false);
        flickReady = true;

        ReleaseGesture gesture = sender as ReleaseGesture;
        TouchHit hitRelease;
        gesture.GetTargetHitResult(out hitRelease);
        releasePoint = hitRelease.Point;
    }

}
