using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

// Tạo phương thức Test:

public class Commands
{
    [CommandMethod("TEST")]
    public void Test()
    {
        var doc = AcAp.DocumentManager.MdiActiveDocument;
        var db = doc.Database;
        var ed = doc.Editor;

        using (var tr = db.TransactionManager.StartTransaction())
        {
            ed.WriteMessage("Hello World");
            tr.Commit();
        }
    }
}