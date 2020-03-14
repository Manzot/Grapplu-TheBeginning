using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManageables
{
    void Initialize();
    void PostInitialize();
    void Refresh();
    void PhysicsRefresh();
}
