using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Altom.AltDriver.Commands
{
    public class AltGetAllFields : AltBaseCommand
    {
        AltGetAllFieldsParams cmdParams;
        public AltGetAllFields(IDriverCommunication commHandler, AltComponent altComponent, AltObject altObject, AltFieldsSelections altFieldsSelections = AltFieldsSelections.ALLFIELDS) : base(commHandler)
        {
            cmdParams = new AltGetAllFieldsParams(altObject.id, altComponent, altFieldsSelections);
        }
        public Task<List<AltProperty>> Execute()
        {
            CommHandler.Send(cmdParams);
            return CommHandler.Recvall<List<AltProperty>>(cmdParams);
        }
    }
}