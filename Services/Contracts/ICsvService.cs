using ACADTools.Models;
using System.Collections.Generic;

namespace ACADTools.Services.Contracts
{
    public interface ICsvService
    {
        void CreateAndSaveCsv(string saveFilePath, List<RaumpolygonEntity> polygons, List<RaumstempelEntity> raumstempeln, List<TextRaumstempelEntity> texts);
    }
}
