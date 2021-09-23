using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private int health;
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float attackSpeed;

    [SerializeField] private Player unitOwner; //this will not be a serialized field once unit owners are assigned at time of prefab instantiation.

    private Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        healthText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (unitOwner.GetUnitsAreMoving())
        {
            transform.position = (Vector2)transform.position + new Vector2(direction.x * speed, direction.y * speed);
        }

        healthText.text = health.ToString();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PlayerUnit")
        {
            Debug.Log(gameObject.name + " Collided with " + collision.name);
            unitOwner.SetUnitsAreMoving(false);
            StartCoroutine("AttackEnemy", collision);
        }
    }

    private IEnumerator AttackEnemy(Collider2D collision)
    {
        UnitController enemyController = collision.GetComponent<UnitController>();
        float nextHitTime = Time.time;

        while (health > 0 && enemyController.GetHealth() > 0)
        {
            if (Time.time >= nextHitTime)
            {
                enemyController.SetHealth(enemyController.GetHealth() - damage);
                nextHitTime += attackSpeed;
            }
            yield return null;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        unitOwner.SetUnitsAreMoving(true);
        yield return null;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int newHealthValue)
    {
        health = newHealthValue;
    }

    public void SetUnitOwner(Player owner)
    {
        unitOwner = owner;
    }
}
