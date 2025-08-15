using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;

namespace ACADTools.Models.Importing
{
    public class RaumstempelEntity
    {
        [Name("ID_Raumstempel")]
        public string Id { get; set; }
        [Name("Handle_Raumstempel")]
        public string Handle { get; set; }
        [Name("Blockname_Raumstempel")]
        public string BlockDefinitionName { get; set; }
        public List<AttributeDefinitionEntity> BlockAttributes { get; set; }

        //[Name("Raumnummer")]
        //public int RoomNumber { get; set; }
        //[Name("Raumbezeichnung")]
        //public string RoomName { get; set; }
        //[Name("Raumfläche")]
        //public double RoomArea { get; set; }
        //[Name("Raumhöhe")]
        //public double RoomHeight { get; set; }
        //[Name("Sonstiges1")]
        //public string OtherAttribute1 { get; set; }
        //[Name("Sonstiges2")]
        //public string OtherAttribute2 { get; set; }
        //[Name("Sonstiges3")]
        //public string OtherAttribute3 { get; set; }
        //[Name("Sonstiges4")]
        //public string OtherAttribute4 { get; set; }
        //[Name("Sonstiges5")]
        //public string OtherAttribute5 { get; set; }

        ////public string RoomDescription { get; set; }
    }
}
