using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: ExtensionApplication(typeof(RibbonMenu.RibbonMenuInitializer))]

namespace RibbonMenu
{
    public class RibbonMenuInitializer : IExtensionApplication
    {
        private const string TabId = "SBTOOLS_MAIN_TAB";
        private const string SbButtonId = "SBTOOLS_BTN_MANAGER";
        private const string MirrorButtonId = "SBTOOLS_BTN_MIRROR";

        private static readonly RibbonButtonCommandHandler Handler = new RibbonButtonCommandHandler();

        public void Initialize()
        {
            try
            {
                ComponentManager.ItemInitialized += OnItemInitialized;
                AcadApp.DocumentManager.DocumentCreated += OnDocumentCreated;

                CreateRibbonTab();
                Log("\n[SBTools] Ribbon initialized.");
            }
            catch (System.Exception ex)
            {
                Log("\n[SBTools Initialize ERROR]: " + ex.Message);
            }
        }

        public void Terminate()
        {
            ComponentManager.ItemInitialized -= OnItemInitialized;
            AcadApp.DocumentManager.DocumentCreated -= OnDocumentCreated;
        }

        private void OnItemInitialized(object sender, RibbonItemEventArgs e)
        {
            if (ComponentManager.Ribbon != null)
            {
                CreateRibbonTab();
                ComponentManager.ItemInitialized -= OnItemInitialized;
            }
        }

        private void OnDocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            CreateRibbonTab();
        }

        [CommandMethod("SBTEST")]
        public void TestRibbon()
        {
            CreateRibbonTab();
            Log("\n[SBTools] Ribbon checked.");
        }

        public static void CreateRibbonTab()
        {
            try
            {
                RibbonControl ribbon = ComponentManager.Ribbon;
                if (ribbon == null) return;

                RibbonTab oldTab = null;
                foreach (RibbonTab tab in ribbon.Tabs)
                {
                    if (tab.Id == TabId)
                    {
                        oldTab = tab;
                        break;
                    }
                }

                if (oldTab != null)
                {
                    ribbon.Tabs.Remove(oldTab);
                }

                // TẠO TAB SBTOOLS
                RibbonTab newTab = new RibbonTab
                {
                    Title = "SBTools",
                    Id = TabId
                };

                // ==========================================
                // PANEL 1 - SB TOOLS MANAGER
                // ==========================================
                RibbonPanelSource source1 = new RibbonPanelSource
                {
                    Title = "SB Tools Manager"
                };
                RibbonPanel panel1 = new RibbonPanel
                {
                    Source = source1
                };

                RibbonToolTip sbToolTip = new RibbonToolTip
                {
                    Title = "SB Tools Manager",
                    Content = "Opens the central management dashboard for all SBTools drafting and automation utilities.",
                    ExpandedContent = "Shortcut Command: SBTOOL\nStreamlines layer, block, and layout management workflows.",
                    Image = LoadImageFromResource("sbtool.png") // Thêm icon vào Tooltip
                };

                RibbonButton btnSbTool = new RibbonButton
                {
                    Id = SbButtonId,
                    Text = "SB Tools",
                    ShowText = true,
                    ShowImage = true,
                    Size = RibbonItemSize.Large,
                    IsEnabled = true,
                    CommandParameter = "SBTOOL ",
                    CommandHandler = Handler,
                    ToolTip = sbToolTip
                };

                // Nạp icon 16x16 và 32x32 cho SB Tools
                try
                {
                    btnSbTool.Image = LoadImageFromResource("sbtool_16.png");
                    btnSbTool.LargeImage = LoadImageFromResource("sbtool.png");
                }
                catch { }

                source1.Items.Add(btnSbTool);
                newTab.Panels.Add(panel1);

                // ==========================================
                // PANEL 2 - MIRROR BLOCK
                // ==========================================
                RibbonPanelSource source2 = new RibbonPanelSource
                {
                    Title = "Mirror Block"
                };
                RibbonPanel panel2 = new RibbonPanel
                {
                    Source = source2
                };

                RibbonToolTip mirrorToolTip = new RibbonToolTip
                {
                    Title = "Mirror Block (MMI)",
                    Content = "Intelligently mirrors blocks, attributes, text, and dynamic references while keeping text readable.",
                    ExpandedContent = "Shortcut Command: MMI\nAutomates block-state transformations and prevents flipped text issues.",
                    Image = LoadImageFromResource("mirror-image.png") // Thêm icon vào Tooltip
                };

                RibbonButton btnMirror = new RibbonButton
                {
                    Id = MirrorButtonId,
                    Text = "Mirror",
                    ShowText = true,
                    ShowImage = true,
                    Size = RibbonItemSize.Large,
                    IsEnabled = true,
                    CommandParameter = "MMI ",
                    CommandHandler = Handler,
                    ToolTip = mirrorToolTip
                };

                // Nạp icon 16x16 và 32x32 cho Mirror Block
                try
                {
                    btnMirror.Image = LoadImageFromResource("mirror_16.png");
                    btnMirror.LargeImage = LoadImageFromResource("mirror-image.png");
                }
                catch { }

                source2.Items.Add(btnMirror);
                newTab.Panels.Add(panel2);

                ribbon.Tabs.Add(newTab);
                newTab.IsActive = true;

                Log("\n[SBTools] Ribbon created successfully with Icons & Tooltips.");
            }
            catch (System.Exception ex)
            {
                Log("\n[SBTools Ribbon ERROR]: " + ex.Message);
            }
        }

        // Hàm nạp ảnh trực tiếp qua Stream (Bỏ qua lỗi pack URI của AutoCAD)
        private static ImageSource LoadImageFromResource(string fileName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string[] resourceNames = assembly.GetManifestResourceNames();
                string matchedResourceName = Array.Find(resourceNames, r => r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrEmpty(matchedResourceName)) return null;

                using (Stream stream = assembly.GetManifestResourceStream(matchedResourceName))
                {
                    if (stream != null)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze(); // Tối ưu hóa render WPF
                        return bitmap;
                    }
                }
            }
            catch { }
            return null;
        }

        private static void Log(string message)
        {
            try
            {
                AcadApp.DocumentManager
                    .MdiActiveDocument?
                    .Editor.WriteMessage(message);
            }
            catch { }
        }
    }

    public class RibbonButtonCommandHandler : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                string command = parameter as string;
                if (string.IsNullOrEmpty(command) && parameter is RibbonButton btn)
                {
                    command = btn.CommandParameter as string;
                }

                if (string.IsNullOrWhiteSpace(command)) return;

                Document doc = AcadApp.DocumentManager.MdiActiveDocument;
                if (doc != null)
                {
                    doc.SendStringToExecute(command, true, false, true);
                }
            }
            catch (System.Exception ex)
            {
                AcadApp.ShowAlertDialog("SBTools Command ERROR:\n" + ex.Message);
            }
        }
    }
}