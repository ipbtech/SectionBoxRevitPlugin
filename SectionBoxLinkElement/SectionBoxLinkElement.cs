using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using System.Windows;

namespace SectionBoxLinkElement
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class SectionBoxLinkElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;

            Selection selection = uidoc.Selection;
            IList<Reference> pickRefs = selection.PickObjects(ObjectType.LinkedElement);

            Views3DSelectionWindow wnd = new Views3DSelectionWindow(doc);
            wnd.ShowDialog();

            wnd.Close();

            return Result.Succeeded;
        }
        public static View3D GetView3D(Document doc, string name)
        {
            View3D view3D = new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>().
                    Where(view => view.Name == name).FirstOrDefault();
            view3D.IsSectionBoxActive = false;

            return view3D;
        }
    }
}