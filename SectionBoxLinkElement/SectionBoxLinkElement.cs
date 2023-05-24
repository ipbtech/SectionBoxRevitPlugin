using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

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

            try
            {
                IList<Reference> pickRefs = selection.PickObjects(ObjectType.LinkedElement);

                if (pickRefs.Count != 0)
                {
                    Views3DSelectionWindow window = new Views3DSelectionWindow(doc, uidoc, pickRefs);
                    try
                    {
                        System.Windows.Window wndRevit = WindowHandle.GettingRevitWindow(commandData);
                        window.Owner = wndRevit;
                    }
                    catch (NullReferenceException) { }

                    window.ShowDialog();
                    window.Close();
                    return Result.Succeeded;
                }
                else
                {
                    MsgShow.Info(TaskDialogIcon.TaskDialogIconError, "Ошибка", "Вы не выбрали ни один элемент ",
                    "Выполнние будет команды отменено", "");
                    return Result.Failed;
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Failed;
            }
        }
    }
}