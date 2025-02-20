using SampSharp.Entities.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.WorldWeather.Services
{
    public interface IWorldWeatherService
    {
        /// <summary>
        /// Returns a weather id for the given x, y position.
        /// </summary>
        int GetWeatherAt(Vector3 position);

        /// <summary>
        /// Predicts the weather at a given position and future time.
        /// </summary>
        int PredictWeather(Vector3 position, DateTime futureTime);

        /// <summary>
        /// Gets the current wind speed at a given x, y position.
        /// </summary>
        double GetWindSpeedAt(Vector3 position);

        /// <summary>
        /// Gets the current temperature at the given x, y, z position.
        /// </summary>
        double GetTemperatureAt(Vector3 position);

        bool IsPositionHumid(Vector3 position);
        bool IsPositionArid(Vector3 position);
    }

}
