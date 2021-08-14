using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUI : UICtrl
{
    [SerializeField] private GameJoystick moveJoystick = null;
    [SerializeField] private GameJoystick attackJoystick = null;
    [SerializeField] private CameraFollow cameraFollow = null;
    [SerializeField] private CubeManager cubeManager = null;
    [SerializeField] private Transform cubeParent = null;


}
