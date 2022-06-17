using System;
using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using Systems;
using UnityEngine;

public class ECSStartUp : MonoBehaviour
{
    [SerializeField] private MoveInDirection _moveInDirection;
    [SerializeField] private MoveInDirection[] _planes;
    
    private EcsWorld _ecsWorld;
    private EcsSystems _systems;

    private void Start()
    {
        _ecsWorld = new EcsWorld();
        _systems = new EcsSystems(_ecsWorld)
                .Add(new DoSomethingWithTriggerSystem())
            ;
        
        _systems.Init();

        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                var unit = Instantiate(_moveInDirection, new Vector3(transform.position.x+j, transform.position.y, transform.position.z+i), 
                    new Quaternion(Quaternion.identity.x,Quaternion.identity.y+i, Quaternion.identity.z, Quaternion.identity.w ));
                unit.Init(_ecsWorld);
            }
            
        }

        foreach (var plane in _planes)
        {
            plane.Init(_ecsWorld);
        }
    }


    protected virtual void Update()
    {
        _systems.Run();
    }

    protected void OnDestroy()
    {
        _systems.Destroy();
        _ecsWorld.Destroy();
    }
}
