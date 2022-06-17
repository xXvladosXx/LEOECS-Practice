using System;
using System.Threading;
using Components;
using Leopotam.Ecs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Systems
{
    public struct TransformParallelJob<TIn, TOut> : IJobParallelFor
        where TIn : struct
        where TOut : struct
    {
        private NativeArray<TIn> _input;
        public NativeArray<TOut> Output;
        private readonly Func<TIn, TOut> _transform;
        
        public TransformParallelJob(NativeArray<TIn> input, NativeArray<TOut> output, Func<TIn, TOut> transform)
        {
            this._input = input;
            this.Output = output;
            this._transform = transform;
        }

        void IJobParallelFor.Execute(int index)
        {
            Output[index] = _transform(_input[index]);
        }
    }
    
    public class DoSomethingWithTriggerSystem : IEcsRunSystem
    {
        private EcsFilter<OnTriggerEnter> _filter;
        private JobHandle Dependency;

        public void Run()
        {
            using var inputDirections = new NativeList<Vector3>(Allocator.TempJob);

            foreach (var i in _filter)
            {
                ref var onTriggerEnter = ref _filter.Get1(i);
                var first = onTriggerEnter.ThisGameObject.GetComponent<MoveInDirection>();
                var second = onTriggerEnter.Other.GetComponent<MoveInDirection>();

                if (first && second)
                {
                    Debug.Log("Collider ecs");
                    inputDirections.Add(first.Entity.Get<UnitRef>().MoveInDirection.direction);
                }
                
                // _filter.GetEntity(i).Del<OnTriggerEnter>();
            }

            using var outputDirections = new NativeArray<Vector3>(inputDirections.Length, Allocator.TempJob);

            var job = new TransformParallelJob<Vector3, Vector3>(inputDirections.AsArray(), outputDirections, vec => -vec);
            //var jobHandle = job.Schedule(outputDirections.Length, 64);
            //jobHandle.Complete();
            
            job.Run(outputDirections.Length);

            var j = 0;
            foreach (var i in _filter)
            {
                ref var onTriggerEnter = ref _filter.Get1(i);
                var first = onTriggerEnter.ThisGameObject.GetComponent<MoveInDirection>();

                first.Entity.Get<UnitRef>().MoveInDirection.direction = job.Output[j];
                j++;
                
                _filter.GetEntity(i).Del<OnTriggerEnter>();
            }
        }
        
        
    }
}