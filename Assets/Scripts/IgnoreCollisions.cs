using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IgnoreCollisions
{
    public static void SetItemsCollisions(this List<Collider> items, bool isActive)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Collider firstCollider = items[i];

            for (int j = i + 1; j < items.Count; j++)
            {
                Collider secondCollider = items[j];

                if (firstCollider == null || secondCollider == null)
                    continue;

                Physics.IgnoreCollision(
                    firstCollider,
                    secondCollider,
                    !isActive
                );
            }
        }
    }
}
