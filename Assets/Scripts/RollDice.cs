using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class RollDice : MonoBehaviour
{
    [field: SerializeField] public float MaxAngleX { get; private set; } = 45f;
    [field: SerializeField] public float MinAngleX { get; private set; } = -45f;
    [field: SerializeField] public float MaxAngleY { get; private set; } = 45f;
    [field: SerializeField] public float MinAngleY { get; private set; } = -45f;

    [field: SerializeField] public float MaxLinearForce { get; private set; } = 10f;
    [field: SerializeField] public float MinLinearForce { get; private set; } = 5f;

    [field: SerializeField] public float MaxAngularForce { get; private set; } = 10f;
    [field: SerializeField] public float MinAngularForce { get; private set; } = 5f;

    [field: SerializeField] public uint DiceCount { get; set; } = 5;
    [field: SerializeField] public GameObject DicePrefab { get; private set; }

    [field: SerializeField] public int Result { get; private set; }
    [field: SerializeField] public UIDocument ui { get; private set; }
    public Action EndRoll { get; set; }
    public Action StartRoll { get; set; }

    public static RollDice Instance { get; private set; }



    private const float CollisionDelay = 0.15f;
    private const float VelocityThreshold = 0.1f;
    private const float CheckInterval = 0.1f;

    private readonly List<OneDiceResult> spawnedDice = new();

    private bool isRolling;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (DicePrefab == null)
        {
            Debug.LogError("DicePrefab не назначен.", this);
            return;
        }

        if (DicePrefab.GetComponent<Rigidbody>() == null)
        {
            DicePrefab.AddComponent<Rigidbody>();
        }
    }

    public void Roll()
    {
        if (isRolling)
            return;

        StartCoroutine(RollVoid());
    }

    private IEnumerator RollVoid()
    {
        isRolling = true;
        Result = 0;

        ClearPreviousDice();

        StartRoll?.Invoke();
        SpawnDice();

        var colliders = spawnedDice.Select(x => x.GetComponent<Collider>()).ToList();
        IgnoreCollisions.SetItemsCollisions(colliders, false);

        yield return new WaitForSeconds(CollisionDelay);

        IgnoreCollisions.SetItemsCollisions(colliders, true);

        yield return WaitForDiceToStop();

        CalculateResult();

        isRolling = false;

        EndRoll?.Invoke();
    }

    private void SpawnDice()
    {
        for (int i = 0; i < DiceCount; i++)
        {
            GameObject diceObject = Instantiate(
                DicePrefab,
                transform.position,
                Random.rotation
            );

            OneDiceResult dice = diceObject.GetComponent<OneDiceResult>();

            if (dice == null)
            {
                dice = diceObject.AddComponent<OneDiceResult>();
            }

            spawnedDice.Add(dice);

            ApplyRandomForce(diceObject.GetComponent<Rigidbody>());
        }
    }

    private void ApplyRandomForce(Rigidbody rb)
    {
        // Направление
        float angleX = Random.Range(MinAngleX, MaxAngleX);
        float angleY = Random.Range(MinAngleY, MaxAngleY);

        Vector3 direction =
            Quaternion.Euler(angleY, angleX, 0f) * transform.forward;

        // Линейная сила
        float linearForce =
            Random.Range(MinLinearForce, MaxLinearForce);

        rb.AddForce(
            direction * linearForce,
            ForceMode.Impulse
        );

        // Вращение
        Vector3 torqueDirection =
            Random.insideUnitSphere.normalized;

        float angularForce =
            Random.Range(MinAngularForce, MaxAngularForce);

        rb.AddTorque(
            torqueDirection * angularForce,
            ForceMode.Impulse
        );
    }

    private IEnumerator WaitForDiceToStop()
    {
        while (true)
        {
            bool allDiceStopped = true;

            foreach (OneDiceResult dice in spawnedDice)
            {
                if (dice == null)
                    continue;

                Rigidbody rb = dice.Rigidbody;

                bool isMoving =
                    rb.linearVelocity.magnitude > VelocityThreshold ||
                    rb.angularVelocity.magnitude > VelocityThreshold;

                if (isMoving)
                {
                    allDiceStopped = false;
                    break;
                }
            }

            if (allDiceStopped)
                yield break;

            yield return new WaitForSeconds(CheckInterval);
        }
    }

    private void CalculateResult()
    {
        Result = 0;

        foreach (OneDiceResult dice in spawnedDice)
        {
            if (dice == null)
                continue;

            Result += dice.GetResult();
        }
    }

    private void ClearPreviousDice()
    {
        foreach (OneDiceResult dice in spawnedDice)
        {
            if (dice != null)
            {
                Destroy(dice.gameObject);
            }
        }

        spawnedDice.Clear();
    }
}