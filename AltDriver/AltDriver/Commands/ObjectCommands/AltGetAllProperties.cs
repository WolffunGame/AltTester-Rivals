using System.Collections.Generic;
using System.Threading.Tasks;

namespace Altom.AltDriver.Commands
{
    public class AltGetAllProperties : AltBaseCommand
    {
        AltGetAllPropertiesParams cmdParams;

        public AltGetAllProperties(IDriverCommunication commHandler, AltComponent altComponent, AltObject altObject, AltPropertiesSelections altPropertiesSelections = AltPropertiesSelections.ALLPROPERTIES) : base(commHandler)
        {
            cmdParams = new AltGetAllPropertiesParams(altObject.id, altComponent, altPropertiesSelections);

        }
        public Task<List<AltProperty>> Execute()
        {
            CommHandler.Send(cmdParams);
            return CommHandler.Recvall<List<AltProperty>>(cmdParams);
        }
    }
}