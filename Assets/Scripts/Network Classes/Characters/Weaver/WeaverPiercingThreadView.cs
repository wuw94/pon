﻿using UnityEngine;

public class WeaverPiercingThreadView : ClientCalculatedView
{
    public override void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x / 1.03f, transform.localScale.y, transform.localScale.z);
    }
}