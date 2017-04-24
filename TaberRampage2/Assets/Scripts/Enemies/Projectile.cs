using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    const float DESTROYTIME = 5;
    [SerializeField]
    float speed, damage, slowPercent, slowDurration;
    [SerializeField]
    bool stunsPlayer;
    Vector3 target;

	// Use this for initialization
	void Awake ()
    {
        target = Vector3.right;
        Destroy(this.gameObject, DESTROYTIME);
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(target * speed * Time.smoothDeltaTime);
	}

    public void SetTarget(Vector3 t)
    {
        target = t;
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
