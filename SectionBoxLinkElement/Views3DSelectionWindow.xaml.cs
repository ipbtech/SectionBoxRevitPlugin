using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SectionBoxLinkElement
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class Views3DSelectionWindow : Window
    {
        Document doc;
        bool checkCreate3DView;
        public Views3DSelectionWindow(Document _doc)
        {
            InitializeComponent();

            doc = _doc;

            List<string> nameViews3D = new List<string>() { "Текущий активный 3D вид" };

            IList<View3D> views3D = new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>().
                Where(view => view.IsTemplate == false).ToList();
            foreach (var view3D in views3D)
            {
                nameViews3D.Add(view3D.Name);
            }
            foreach (string name in nameViews3D)
            {
                Views3DList.Items.Add(name);
            }
        }
        private void Create3DView_checked(object sender, RoutedEventArgs e)
        {
            checkCreate3DView = true;
        }
        private void Create3DView_unchecked(object sender, RoutedEventArgs e)
        {
            checkCreate3DView = false;
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            #region Получение 3D вида для SectionBox
            string viewName = string.Empty;
            if (checkCreate3DView == true)
            {
                //Создание дефолтного 3D вида и получение его имени во viewName
            }
            else
            {
                viewName = Views3DList.SelectedItem.ToString();
                if (viewName == "Текущий активный 3D вид")
                {
                    if (doc.ActiveView.GetType() == typeof(View3D))
                    {
                        viewName = doc.ActiveView.Name;
                    }
                    else
                    {
                        MsgShow.Info(TaskDialogIcon.TaskDialogIconError, "Ошибка", "Текущий активный вид не является 3D видом",
                            "Выберите существующий 3D вид или создайте новый", "");
                    }
                }
            }
            #endregion
            // Получение View3D по его имени
            #region Получение BoundingBox по выбранным элементам, установка Max и Min
            // Получение нового BoundingBox на View3D
            #endregion
            // Присвоение SectionBox к View3D
            // Открытие вида View3D
        }
    }
}

