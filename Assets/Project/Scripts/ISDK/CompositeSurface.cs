// Copyright (c) Meta Platforms, Inc. and affiliates.

using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using System.Collections.Generic;
using UnityEngine;

public class CompositeSurface : MonoBehaviour, ISurface, IBounds, ISerializationCallbackReceiver
{
    [SerializeField, Interface(typeof(ISurface))]
    List<MonoBehaviour> _surfaces;
    public List<ISurface> Surfaces { get; private set; }

    public Bounds Bounds
    {
        get
        {
            Bounds result = default;
            bool valid = false;
            Surfaces.ForEach(surface =>
            {
                if (surface is IBounds bounds)
                {
                    if (valid)
                    {
                        result.Encapsulate(bounds.Bounds);
                    }
                    else
                    {
                        result = bounds.Bounds;
                        valid = true;
                    }
                }
            });
            return result;
        }
    }

    public Transform Transform => transform;

    public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance = 0)
    {
        SurfaceHit? result = null;

        for (int i = 0; i < Surfaces.Count; i++)
        {
            if (Surfaces[i].ClosestSurfacePoint(point, out var surfaceHit, maxDistance) &&
                (!result.HasValue || result.Value.Distance > surfaceHit.Distance))
            {
                result = surfaceHit;
            }
        }

        hit = result.GetValueOrDefault();
        return result.HasValue;
    }

    public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0)
    {
        SurfaceHit? result = null;

        for (int i = 0; i < Surfaces.Count; i++)
        {
            if (Surfaces[i].Raycast(ray, out var surfaceHit, maxDistance) &&
                (!result.HasValue || result.Value.Distance > surfaceHit.Distance))
            {
                result = surfaceHit;
            }
        }

        hit = result.GetValueOrDefault();
        return result.HasValue;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    void ISerializationCallbackReceiver.OnAfterDeserialize() => Surfaces = _surfaces.ConvertAll(x => x as ISurface);
}
