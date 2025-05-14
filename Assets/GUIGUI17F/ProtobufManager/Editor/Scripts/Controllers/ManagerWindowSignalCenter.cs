namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// holder for global signals
    /// </summary>
    public class ManagerWindowSignalCenter
    {
        /// <summary>
        /// argument0: previousTypeName, argument1: newTypeName
        /// </summary>
        public static readonly Signal<string,string> TypeChangedSignal = new Signal<string, string>();
    }
}