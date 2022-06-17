using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Components;
using Leopotam.Ecs;
using UnityEngine;

public class MoveInDirection : MonoBehaviour
{
    public EcsEntity Entity;
    
    [SerializeField] private float _speed;
    public Vector3 direction = Vector3.forward;

    private EcsWorld _ecsWorld;

    public void Init(EcsWorld ecsWorld)
    {
        Entity = ecsWorld.NewEntity();
        Entity.Get<UnitRef>().MoveInDirection = this;

        _ecsWorld = ecsWorld;
    }
    
    private void Awake()
    {
        direction = transform.forward;
    }

    private void Update()
    {
        this.transform.position += this.direction * Time.deltaTime * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ref var onTriggerEnter = ref _ecsWorld.NewEntity().Get<OnTriggerEnter>();
        // onTriggerEnter.ThisGameObject = gameObject;
        // onTriggerEnter.Other = other;

        if (other.GetComponent<MoveInDirection>()!= null)
        {
            Debug.Log("collided");
            direction = -direction;
        }
    }
}
