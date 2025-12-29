using System.Collections.Generic;
using UnityEngine;

public class AttackSlotsController : MonoBehaviour
{
    public List<AttackSlot> attackSlots = new List<AttackSlot>();

    public int WhichSlotsFree()
    {
        foreach (var slot in attackSlots)
        {
            if (!slot.isFulled)
            {
                return attackSlots.IndexOf(slot);
            }
        }

        return -1;
    }
}
