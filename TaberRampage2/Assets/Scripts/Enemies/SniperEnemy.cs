using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]

public class SniperEnemy : EnemyParentScript
{
    RaycastHit hit;
    float range = 40.0f;
    LineRenderer line;
    float laserMoveSpeed = 0.5f;

    Vector3 currentHitPoint;
    Ray ray;

    [SerializeField]
    Transform laserPoint;
    [SerializeField]
    float timeTillShot;
    [SerializeField]
    bool stunsPlayer;
    float shotClock;
    bool rayHit, lockOn;
    Vector3 mPos;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.SetVertexCount(2);
        line.SetWidth(0.25f, 0.25f);
        Reload();
        lockOn = false;
        rayHit = false;
    }

    protected override void MonsterInteraction()
    {
        if (!lockOn)
        {
            GetDirectionToFace();
            mPos = monster.transform.position;
        }

        Vector3 heading = currentHitPoint - laserPoint.position;
        Vector3 direction = heading / (heading.magnitude);

        ray = new Ray(laserPoint.position, direction);

        if (Physics.Raycast(ray, out hit, range))
        {
            if (hit.collider.gameObject.GetComponent<MonsterController>() || hit.collider.gameObject.layer == LayerMask.NameToLayer("FoundationGround"))
            {
                currentHitPoint = hit.point; //+ hit.normal;
                if (hit.collider.gameObject.GetComponent<MonsterController>())
                {
                    rayHit = true;
                }
                //print(hit.collider.name);
            }

        }

        if (!lockOn)
        {
            if (Mathf.Round(currentHitPoint.x) == Mathf.Round(mPos.x) || AnimationSetter.instance.state != MonsterState.GroundWalk || AnimationSetter.instance.state != MonsterState.RoofWalk)
            {
                currentHitPoint = Vector3.MoveTowards(currentHitPoint, mPos, laserMoveSpeed);
            }
            else
            {
                Vector3 xChange = new Vector3(mPos.x, currentHitPoint.y, currentHitPoint.z);
                currentHitPoint = Vector3.MoveTowards(currentHitPoint, xChange, laserMoveSpeed);
            }
        }

        line.SetPosition(0, laserPoint.position);
        line.SetPosition(1, currentHitPoint);       

        if (currentHitPoint == mPos || rayHit)
        {
            shotClock += Time.deltaTime;
            lockOn = true;

            if (shotClock > timeTillShot)
            {
                if (Physics.Raycast(ray, out hit, range))
                {
                    if (hit.collider.gameObject.GetComponent<MonsterController>() != null)
                    {
                        print("Monster hit by Sniper");
                        monster.TakeDamage(stunsPlayer);
                    }
                }
                Reload();
            }
        }        
    }

    void Reload()
    {
        if (GetComponent<SpriteRenderer>().flipX)
        {
            ray = new Ray(laserPoint.position, laserPoint.right);
            currentHitPoint = laserPoint.right * range;
        }
        else
        {
            ray = new Ray(laserPoint.position, -laserPoint.right);
            currentHitPoint = -laserPoint.right * range;
        }
        shotClock = 0;
        rayHit = false;
        lockOn = false;
    }
}