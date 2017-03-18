using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    const float DESTROYTIME = 5;
    [SerializeField]
    float speed, damage, slowPercent, slowDurration;
    [SerializeField]
    bool stunsPlayer;
    bool readyToFire;
    Vector3 target;

	// Use this for initialization
	void Awake ()
    {
        target = Vector3.zero;
        readyToFire = false;
        Destroy(this.gameObject, DESTROYTIME);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (readyToFire)
        {
            transform.position += target * speed * Time.deltaTime;
        }
	}

    public void SetTarget(Vector3 t)
    {
        target = (t - transform.position).normalized;
        readyToFire = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<MonsterController>() != null)
        {
            col.GetComponent<MonsterController>().TakeDamage(damage, stunsPlayer);
            if (slowPercent > 0)
            {
                col.GetComponent<MonsterController>().SlowPlayer(slowPercent, slowDurration);
            }
            Destroy(this.gameObject);
        }
    }
}
