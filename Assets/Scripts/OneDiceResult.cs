using System.Collections.Generic;
using UnityEngine;

public class OneDiceResult : MonoBehaviour
{
    [field: SerializeField]
    public List<Transform> Sides { get; private set; }

    public Rigidbody Rigidbody { get; private set; }
    public Collider Collider { get; private set; }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();

        if (Rigidbody == null)
        {
            Debug.LogError("OneDiceResult требует Rigidbody.", this);
        }

        if (Collider == null)
        {
            Debug.LogError("OneDiceResult требует Collider.", this);
        }
    }

    public int GetResult()
    {
        if (Sides == null || Sides.Count == 0)
        {
            Debug.LogError("—тороны не назначены.", this);
            return 0;
        }

        int result = -1;
        float highestDot = float.MinValue;

        for (int i = 0; i < Sides.Count; i++)
        {
            if (Sides[i].position.y > highestDot)
            {
                result = i;
                highestDot = Sides[i].position.y;
            }
        }

        return result+1;
    }
}