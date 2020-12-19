using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace PixiEditor.Parser
{
    public sealed class CurrentAssemblyDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            typeName = typeName.Replace("Models.DataHolders", "Parser");
            typeName = typeName.Replace("Models.Layers", "Parser");
            return Type.GetType(string.Format("{0}, {1}", typeName, Assembly.GetExecutingAssembly().FullName));
        }
    }
}