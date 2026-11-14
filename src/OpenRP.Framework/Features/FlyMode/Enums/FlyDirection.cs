using System;

namespace OpenRP.Framework.Features.FlyMode.Enums
{
    /// <summary>
    /// The directions a flying player is holding down. Diagonals are two directions combined.
    /// </summary>
    [Flags]
    public enum FlyDirection
    {
        None = 0,
        Forward = 1,
        Back = 2,
        Left = 4,
        Right = 8
    }
}
