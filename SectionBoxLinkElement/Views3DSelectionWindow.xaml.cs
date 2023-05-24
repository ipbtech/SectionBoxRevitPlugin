using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;

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
            string view3DName = SomeMethods.GetView3DName(doc, checkCreate3DView, Views3DList.SelectedItem.ToString());
            try
            {
                View3D boundBoxView = SomeMethods.GetView3D(doc, view3DName);
                #region Получение BoundingBox по выбранным элементам, установка Max и Min



                #endregion
                // Присвоение SectionBox к View3D
                // Открытие вида View3D
            }
            catch (NullReferenceException)
            {
                MsgShow.Info(TaskDialogIcon.TaskDialogIconError, "Ошибка", "Текущий активный вид не является 3D видом",
                    "Выберите существующий 3D вид или создайте новый", "");
            }
        }
        public static class CodeGenerator
        {
            public static int Get()
            {
                return new Random().Next(1000, 9999);
            }
        }
        public static class SomeMethods
        {
            public static string GetView3DName(Document doc, bool check, string selectedItem)
            {
                string viewName = string.Empty;
                if (check == true)
                {
                    viewName = $"SectionBoxView.SpecialId({CodeGenerator.Get()})";
                    ViewFamilyType viewType3D = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().
                        Where(v => v.ViewFamily == ViewFamily.ThreeDimensional).FirstOrDefault();

                    using (Transaction trans = new Transaction(doc, "Create 3DView"))
                    {
                        trans.Start();
                        View3D viewCreate = View3D.CreateIsometric(doc, viewType3D.Id);
                        viewCreate.Name = viewName;
                        trans.Commit();
                    }
                }
                else
                {
                    viewName = selectedItem;
                    if (viewName == "Текущий активный 3D вид")
                    {
                        if (doc.ActiveView.GetType() == typeof(View3D))
                        {
                            viewName = doc.ActiveView.Name;
                        }
                    }
                }
                return viewName;
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
}

