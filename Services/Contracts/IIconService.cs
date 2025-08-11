namespace ACADTools.Services.Contracts
{
    public interface IIconService
    {
        string GetIconPath(string name);
        void CleanUp();
    }
}
