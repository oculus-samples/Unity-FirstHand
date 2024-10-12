// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Interface for PropertyBehaviours
    /// </summary>
    public interface IProperty
    {
        event Action WhenChanged;
    }

    public interface IProperty<T> : IProperty
    {
        T Value { get; set; }
    }
}
