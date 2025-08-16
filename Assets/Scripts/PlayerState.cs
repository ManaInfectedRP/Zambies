using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentCalories;
    [HideInInspector] public float currentHydrationPercent;

    [Header("--|Player Health|--")]
    public float maxHealth;

    [Header("--|Player Calories|--")]
    public float maxCalories;

    [Header("--|Calories Distance|--")]
    public float caloriesLostPerDistance = 1;
    public float caloriesLostPerDistanceTraveled = 5;

    float distanceTraveled = 0;
    Vector3 lastPosition;

    [Header("--|Player Hydration|--")]

    public float maxHydrationPercent;
    public float hydrationLostTimer = 2;
    public bool isHydrationActive = true;

    [Header("--|Variables|--")]
    public GameObject playerModel;

    private void Start()
    {
        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydrationPercent = maxHydrationPercent;

        StartCoroutine(DecreaseHydration());
    }

    private IEnumerator DecreaseHydration()
    {
        while (isHydrationActive)
        {
            currentHydrationPercent -= 1;
            yield return new WaitForSeconds(hydrationLostTimer);
        }
    }

    void Update()
    {
        distanceTraveled += Vector3.Distance(playerModel.transform.position, lastPosition);
        lastPosition = playerModel.transform.position;

        if (distanceTraveled >= caloriesLostPerDistanceTraveled)
        {
            distanceTraveled = 0;
            currentCalories -= caloriesLostPerDistance;
        }




        #region Debug
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentHealth -= 10;
        }
        #endregion
    }

    public void setHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public void setCalories(float newCalories)
    {
        currentCalories = newCalories;

    }

    public void setHydration(float newHydration)
    {
        currentHydrationPercent = newHydration;
    }
}