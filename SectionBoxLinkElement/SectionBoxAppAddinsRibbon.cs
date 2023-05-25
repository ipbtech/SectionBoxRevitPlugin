using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace SectionBoxLinkElement
{
    internal class SectionBoxLinkElementAppAddin : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyName = Assembly.GetExecutingAssembly().Location;
            string commandNamespace = "SectionBoxLinkElement.";

            RibbonPanel addinPanel = application.CreateRibbonPanel(0, "ipb.tools");
            PushButtonData sectionBoxBtnData = new PushButtonData("SectionBoxByLinkedElement", "Рамка выбора \nпо элементу связи", 
                assemblyName, commandNamespace + "SectionBoxLinkElement");

            PushButton sectionBoxButton = addinPanel.AddItem(sectionBoxBtnData) as PushButton;
            BitmapImage logo = new BitmapImage(new Uri("pack://application:,,,/SectionBoxLinkElement_ipb;component/Media/logo_sectionbox.ico"));
            sectionBoxButton.LargeImage = logo;
            sectionBoxButton.ToolTip = "Обрезка 3D вида по элементу из связанной модели";

            ContextualHelp contextHelpLink = new ContextualHelp(ContextualHelpType.Url, "https://github.com/ipbtech/SectionBoxRevitPlugin");
            sectionBoxButton.SetContextualHelp(contextHelpLink);

            return Result.Succeeded;
        }
    }
}
