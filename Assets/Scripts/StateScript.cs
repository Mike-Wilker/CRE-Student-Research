using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class StateScript : MonoBehaviour
{
    public StateScript nextUState;
    public StateScript nextIState;
    public int UReward;
    public int IReward;
    public Image image;
    void Awake()
    {
        image = GetComponent<Image>();
    }
}
