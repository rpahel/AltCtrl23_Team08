namespace ScrollShop.Interfaces
{
    public interface IDebug
    {
        public void AddDebugMethodsToDebugConsole();
        public void RemoveDebugMethodsFromDebugConsole();
        public void DebugPrint(string message);
    }
}