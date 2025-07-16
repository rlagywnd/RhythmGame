using System.Collections.Generic;
using UnityEngine;

public static class WaitForSecondsCache
{
    private static Dictionary<float, WaitForSeconds> cache = new();

    public static WaitForSeconds Get(float seconds)
    {
        if (!cache.TryGetValue(seconds, out var wfs))
        {
            wfs = new WaitForSeconds(seconds);
            cache[seconds] = wfs;
        }
        return wfs;
    }
}
