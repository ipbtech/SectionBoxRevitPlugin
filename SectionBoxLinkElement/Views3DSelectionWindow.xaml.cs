using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.TextFormatting;

namespace SectionBoxLinkElement
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class Views3DSelectionWindow : Window
    {
        Document doc;
        UIDocument uidoc;
        bool checkCreate3DView;
        IList<Reference> pickRefs;
        public Views3DSelectionWindow(Document _doc, UIDocument _uidoc, IList<Reference> _pickRefs)
        {
            InitializeComponent();

            doc = _doc;
            uidoc = _uidoc;
            pickRefs = _pickRefs;

            List<string> nameViews3D = new List<string>() { "Текущий активный вид" };

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
                IList<Element> selectedElems = SomeMethods.GetSelectedElementIds(doc, pickRefs);
                View3D boundBoxView = SomeMethods.GetView3D(doc, view3DName);
                List<XYZ> boxPoints = SomeMethods.GetMainPoints(boundBoxView, selectedElems);
                BoundingBoxXYZ boundBox = SomeMethods.BoundBoxXYZ(boxPoints[0], boxPoints[1]);

                using (Transaction trans = new Transaction(doc, "SectionBoxLinkElement"))
                {
                    trans.Start();
                    boundBoxView.SetSectionBox(boundBox);
                    trans.Commit();
                }
                uidoc.ActiveView = boundBoxView;
                Close();
            }
            catch (NullReferenceException)
            {
                MsgShow.Info(TaskDialogIcon.TaskDialogIconError, "Ошибка", "Ошибка",
                    "Текущий активный вид не является 3D видом \nили на выбранном виде отсутствуют (скрыты) целевые элементы", "");
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
                    if (viewName == "Текущий активный вид")
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
                using (Transaction trans = new Transaction(doc, "ProcessingView"))
                {
                    trans.Start();
                    view3D.IsSectionBoxActive = false;
                    trans.Commit();
                }
                return view3D;
            }

            public static IList<Element> GetSelectedElementIds(Document doc, IList<Reference> refList)
            {
                IList<Element> elements = new List<Element>();
                foreach (Reference reference in refList)
                {
                    ElementId rvtlinkElemId = reference.ElementId;
                    RevitLinkInstance element = doc.GetElement(rvtlinkElemId) as RevitLinkInstance;
                    Document linkDoc = element.GetLinkDocument();
                    ElementId linkElemId = reference.LinkedElementId;

                    elements.Add(linkDoc.GetElement(linkElemId));
                }
                return elements;
            }
            public static List<XYZ> GetMainPoints(View3D view, IList<Element> elements)
            {
                #region Иницииализация переменных
                double Min_X = new double();
                double Min_Y = new double();
                double Min_Z = new double();

                double Max_X = new double();
                double Max_Y = new double();
                double Max_Z = new double();

                double kof = 100/304.8;

                List<double> Min_XList = new List<double>();
                List<double> Min_YList = new List<double>();
                List<double> Min_ZList = new List<double>();

                List<double> Max_XList = new List<double>();
                List<double> Max_YList = new List<double>();
                List<double> Max_ZList = new List<double>();
                #endregion

                foreach (Element el in elements)
                {
                    BoundingBoxXYZ box = el.get_BoundingBox(view);

                    #region Получение максимальных значений координат
                    Max_X = box.Max.X;
                    Max_Y = box.Max.Y;
                    Max_Z = box.Max.Z;
                    #endregion

                    #region Получение минимальных значений координат
                    Min_X = box.Min.X;
                    Min_Y = box.Min.Y;
                    Min_Z = box.Min.Z;
                    #endregion

                    #region Добавление в список максимальных значений координат
                    Max_XList.Add(Max_X);
                    Max_YList.Add(Max_Y);
                    Max_ZList.Add(Max_Z);
                    #endregion

                    #region Добавление в список минимальных значений координат
                    Min_XList.Add(Min_X);
                    Min_YList.Add(Min_Y);
                    Min_ZList.Add(Min_Z);
                    #endregion
                }
                #region Вычисление максимальных и мимнимальных значений XYZ
                double X_Max = Max_XList.Max() + kof;
                double X_Min = Min_XList.Min() - kof;
                double Y_Max = Max_YList.Max() + kof;
                double Y_Min = Min_YList.Min() - kof;
                double Z_Max = Max_ZList.Max() + kof;
                double Z_Min = Min_ZList.Min() - kof;
                #endregion

                XYZ Max = new XYZ(X_Max, Y_Max, Z_Max);
                XYZ Min = new XYZ(X_Min, Y_Min, Z_Min);
                return new List<XYZ>() { Max, Min };
            }
            public static BoundingBoxXYZ BoundBoxXYZ(XYZ max, XYZ min)
            {
                BoundingBoxXYZ box = new BoundingBoxXYZ();
                box.Max = max;
                box.Min = min;
                return box;
            }
        }
    }
}

