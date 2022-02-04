using System;
using UnityEngine;

public sealed class GlobalState {
    private GlobalState() {
        throw new InvalidOperationException();
    }

    public static int GroundLayer = 7;
    public static LayerMask GroundLayerMask = 1 << GroundLayer;
}