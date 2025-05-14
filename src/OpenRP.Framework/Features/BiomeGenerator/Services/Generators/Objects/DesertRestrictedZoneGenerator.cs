using OpenRP.Framework.Features.BiomeGenerator.Entities;
using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.BiomeGenerator.Services.Generators.Objects
{
    public class DesertRestrictedZoneGenerator : IBiomeObjectGenerator
    {
        public string ObjectType => "RestrictedZone";

        public BiomeObject Generate(Vector2 virtualPosition, Vector3 gamePosition, Vector3 gameRotation, Vector3 defaultRotation, Vector3 maxAngleRotation, Color outputColor)
        {
            int[] obj_arr_restricted_zone = {
                12957, 19602, 3363, 3364
            };

            int modelId = obj_arr_restricted_zone[Random.Shared.Next(obj_arr_restricted_zone.Length)];

            BiomeObject restrictedZoneObject = new BiomeObject(
                obj_arr_restricted_zone[Random.Shared.Next(obj_arr_restricted_zone.Length)],
                virtualPosition,
                gamePosition,
                gameRotation,
                outputColor
            );

            return restrictedZoneObject;
        }
    }
}
