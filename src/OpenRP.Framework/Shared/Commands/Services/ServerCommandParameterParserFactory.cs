using OpenRP.Framework.Shared.Commands.Helpers;
using SampSharp.Entities.SAMP.Commands;
using System.Reflection;

namespace OpenRP.Framework.Shared.Commands.Services
{
    /// <summary>
    /// SampSharp's parsers, except that floats are read the same on every server culture (see <see cref="InvariantFloatParser"/>).
    /// </summary>
    public class ServerCommandParameterParserFactory : ICommandParameterParserFactory
    {
        private readonly DefaultCommandParameterParserFactory _defaultFactory = new DefaultCommandParameterParserFactory();

        public ICommandParameterParser CreateParser(ParameterInfo[] parameters, int index)
        {
            if (parameters[index].ParameterType == typeof(float))
            {
                return new InvariantFloatParser();
            }
            return _defaultFactory.CreateParser(parameters, index);
        }
    }
}
