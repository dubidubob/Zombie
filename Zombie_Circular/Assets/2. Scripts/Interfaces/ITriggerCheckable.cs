using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerCheckable
{ 
    bool IsTowerFront { get; set; }
    bool IsZombieFront { get; set; }
    bool IsZombieBack { get; set; }

    void SetTowerFront(bool isTowerFront);
    void SetZombieFront(bool isZombieFront);
    void SetZombieBack(bool isZombieBack);

}