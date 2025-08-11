namespace ACADTools.Services.Contracts
{
    public interface IAcadDocumentService
    {
        string GetFullPathOfCurrentDwg();
        string GetNameOfCurrentDwg();
    }
}
