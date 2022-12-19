using System;
using System.Threading;
using System.Threading.Tasks;
using Altom.AltDriver.Commands;

namespace Altom.AltDriver
{
    public class AltObject
    {
        public string name;
        public int id;
        public int x;
        public int y;
        public int z;
        public int mobileY;
        public string type;
        public bool enabled;
        public float worldX;
        public float worldY;
        public float worldZ;
        public int idCamera;
        public int transformParentId;
        public int transformId;
        [Newtonsoft.Json.JsonIgnore]
        public IDriverCommunication CommHandler;

        public AltObject(string name, int id = 0, int x = 0, int y = 0, int z = 0, int mobileY = 0, string type = "", bool enabled = true, float worldX = 0, float worldY = 0, float worldZ = 0, int idCamera = 0, int transformParentId = 0, int transformId = 0)
        {
            this.name = name;
            this.id = id;
            this.x = x;
            this.y = y;
            this.z = z;
            this.mobileY = mobileY;
            this.type = type;
            this.enabled = enabled;
            this.worldX = worldX;
            this.worldY = worldY;
            this.worldZ = worldZ;
            this.idCamera = idCamera;
            this.transformParentId = transformParentId;
            this.transformId = transformId;
        }

        public async Task<AltObject> GetParent()
        {
            var altObject = await new AltFindObject(CommHandler, By.PATH, "//*[@id=" + this.id + "]/..", By.NAME, "", true).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        [Obsolete("getParent is deprecated, please use GetParent instead.")]
        public async Task<AltObject> getParent()
        {
            var altObject =await new AltFindObject(CommHandler, By.PATH, "//*[@id=" + this.id + "]/..", By.NAME, "", true).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }
        public AltVector2 GetScreenPosition()
        {
            return new AltVector2(x, y);
        }

        [Obsolete("getScreenPosition is deprecated, please use GetScreenPosition instead.")]
        public AltVector2 getScreenPosition()
        {
            return new AltVector2(x, y);
        }
        public AltVector3 GetWorldPosition()
        {
            return new AltVector3(worldX, worldY, worldZ);
        }

        [Obsolete("getWorldPosition is deprecated, please use GetWorldPosition instead.")]
        public AltVector3 getWorldPosition()
        {
            return new AltVector3(worldX, worldY, worldZ);
        }
        public async Task<T> GetComponentProperty<T>(string componentName, string propertyName, string assemblyName, int maxDepth = 2)
        {
            var propertyValue =await new AltGetComponentProperty<T>(CommHandler, componentName, propertyName, assemblyName, maxDepth, this).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return propertyValue;
        }

        public async void SetComponentProperty(string componentName, string propertyName, object value, string assemblyName)
        {
           await new AltSetComponentProperty(CommHandler, componentName, propertyName, value, assemblyName, this).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
        }

        public async Task<T> CallComponentMethod<T>(string componentName, string methodName, string assemblyName, object[] parameters, string[] typeOfParameters = null)
        {
            var result =await new AltCallComponentMethod<T>(CommHandler, componentName, methodName, parameters, typeOfParameters, assemblyName, this).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return result;
        }

        public async Task<string> GetText()
        {
            var text =await new AltGetText(CommHandler, this).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return text;
        }

        public async Task<AltObject> SetText(string text, bool submit = false)
        {
            var altObject =await new AltSetText(CommHandler, this, text, submit).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        /// <summary>
        /// Click current object
        /// </summary>
        /// <param name="count">Number of times to click</param>
        /// <param name="interval">Interval between clicks in seconds</param>
        /// <param name="wait">Wait for command to finish</param>
        /// <returns>The clicked object</returns>
        public async Task<AltObject> Click(int count = 1, float interval = 0.1f, bool wait = true)
        {
            var altObject =await new AltClickElement(CommHandler, this, count, interval, wait).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        public async Task<AltObject> PointerUpFromObject()
        {
            var altObject =await new AltPointerUpFromObject(CommHandler, this).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        public async Task<AltObject> PointerDownFromObject()
        {
            var altObject =await new AltPointerDownFromObject(CommHandler, this).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        public async Task<AltObject> PointerEnterObject()
        {
            var altObject =await new AltPointerEnterObject(CommHandler, this).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        public async Task<AltObject> PointerExitObject()
        {
            var altObject =await new AltPointerExitObject(CommHandler, this).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        /// <summary>
        /// Tap current object
        /// </summary>
        /// <param name="count">Number of taps</param>
        /// <param name="interval">Interval in seconds</param>
        /// <param name="wait">Wait for command to finish</param>
        /// <returns>The tapped object</returns>
        public async Task<AltObject> Tap(int count = 1, float interval = 0.1f, bool wait = true)
        {
            var altObject =await new AltTapElement(CommHandler, this, count, interval, wait).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        public async Task<System.Collections.Generic.List<AltComponent>> GetAllComponents()
        {
            var altObject =await new AltGetAllComponents(CommHandler, this).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        public async Task<System.Collections.Generic.List<AltProperty>> GetAllProperties(AltComponent altComponent, AltPropertiesSelections altPropertiesSelections = AltPropertiesSelections.ALLPROPERTIES)
        {
            var altObject =await new AltGetAllProperties(CommHandler, altComponent, this, altPropertiesSelections).Execute();
           await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        public async Task<System.Collections.Generic.List<AltProperty>> GetAllFields(AltComponent altComponent, AltFieldsSelections altFieldsSelections = AltFieldsSelections.ALLFIELDS)
        {
            var altObject =await new AltGetAllFields(CommHandler, altComponent, this, altFieldsSelections).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }

        public async Task<System.Collections.Generic.List<string>> GetAllMethods(AltComponent altComponent, AltMethodSelection methodSelection = AltMethodSelection.ALLMETHODS)
        {
            var altObject =await new AltGetAllMethods(CommHandler, altComponent, methodSelection).Execute();
            await CommHandler.SleepFor(CommHandler.GetDelayAfterCommand());
            return altObject;
        }
    }
}
