using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
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

            Selection selection = uidoc.Selection;
            Reference pickRef = selection.PickObject(ObjectType.LinkedElement);

            MessageBox.Show($"Selected element: {doc.GetElement(pickRef.LinkedElementId)}");

            //Element pickElement = doc.GetElement(pickRef);
            //BoundingBoxXYZ boxXYZ = pickElement.;


            //BoundingBoxXYZ boxXYZ = new BoundingBoxXYZ();

            return Result.Succeeded;
        }
    }
}