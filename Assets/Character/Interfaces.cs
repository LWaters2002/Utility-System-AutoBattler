using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable
{
    public GameObject gameObject { get; }
    public bool LeftClick();
    public bool RightClick();
}
