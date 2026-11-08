namespace OpenRP.Framework.Shared.Extensions
{
    public static class VectorExtensions
    {
        extension(Vector3 vector)
        {
            /// <summary>
            /// The X and Y of the vector, like SampSharp 0.x's Vector3.XY.
            /// </summary>
            public Vector2 XY => new Vector2(vector.X, vector.Y);
        }
    }
}
