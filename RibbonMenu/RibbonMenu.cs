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
        private const string LicenseButtonId = "SBTOOLS_BTN_LICENSE"; // Đổi id cho nút About thành License
        private const string FindButtonId = "SBTOOLS_BTN_FIND";
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
                // 1. PANEL LICENSE (CHỈ CHỨA NÚT ABOUT, ĐẶT Ở GÓC TRÁI NHẤT)
                // ==========================================
                RibbonPanelSource sourceLicense = new RibbonPanelSource
                {
                    Title = "LICENSE"
                };
                RibbonPanel panelLicense = new RibbonPanel
                {
                    Source = sourceLicense
                };

                RibbonToolTip aboutToolTip = new RibbonToolTip
                {
                    Title = "About SBPowerCAD & License",
                    Content = "Displays product version, release date, and copyright protection details.",
                    ExpandedContent = "Shortcut Command: ABOUTSBTOOLS\nChecks software registration and license status.",
                    Image = LoadImageFromResource("about.png")
                };

                RibbonButton btnAbout = new RibbonButton
                {
                    Id = LicenseButtonId,
                    Text = "About",
                    ShowText = true,
                    ShowImage = true,
                    Size = RibbonItemSize.Large,
                    IsEnabled = true,
                    CommandParameter = "ABOUTSBTOOLS ",
                    CommandHandler = Handler,
                    ToolTip = aboutToolTip
                };

                try
                {
                    btnAbout.Image = LoadImageFromResource("about_16.png");
                    btnAbout.LargeImage = LoadImageFromResource("about.png");
                }
                catch { }

                sourceLicense.Items.Add(btnAbout);
                newTab.Panels.Add(panelLicense); // Thêm Panel LICENSE đầu tiên

                // ==========================================
                // 2. PANEL INFORMATION (CHỨA NÚT FIND)
                // ==========================================
                RibbonPanelSource sourceInfo = new RibbonPanelSource
                {
                    Title = "INFORMATION"
                };
                RibbonPanel panelInfo = new RibbonPanel
                {
                    Source = sourceInfo
                };

                RibbonToolTip findToolTip = new RibbonToolTip
                {
                    Title = "Command Finder & Smartsheet",
                    Content = "Opens the official Smartsheet command database and feature request portal.",
                    ExpandedContent = "Shortcut Command: SBFINDER\nAllows users to search all available commands, view update dates, and submit new feature requests.",
                    Image = LoadImageFromResource("find.png")
                };

                RibbonButton btnFind = new RibbonButton
                {
                    Id = FindButtonId,
                    Text = "FIND",
                    ShowText = true,
                    ShowImage = true,
                    Size = RibbonItemSize.Large,
                    IsEnabled = true,
                    CommandParameter = "SBFINDER ",
                    CommandHandler = Handler,
                    ToolTip = findToolTip
                };

                try
                {
                    btnFind.Image = LoadImageFromResource("find_16.png");
                    btnFind.LargeImage = LoadImageFromResource("find.png");
                }
                catch { }

                sourceInfo.Items.Add(btnFind);
                newTab.Panels.Add(panelInfo); // Thêm Panel INFORMATION thứ hai

                // ==========================================
                // 3. PANEL SBTOOL (MANAGER)
                // ==========================================
                RibbonPanelSource source1 = new RibbonPanelSource
                {
                    Title = "SBTOOL"
                };
                RibbonPanel panel1 = new RibbonPanel
                {
                    Source = source1
                };

                RibbonToolTip sbToolTip = new RibbonToolTip
                {
                    Title = "SBTOOL Manager",
                    Content = "Opens the central management dashboard for all SBTools drafting and automation utilities.",
                    ExpandedContent = "Shortcut Command: SBTOOL\nStreamlines layer, block, and layout management workflows.",
                    Image = LoadImageFromResource("sbtool.png")
                };

                RibbonButton btnSbTool = new RibbonButton
                {
                    Id = SbButtonId,
                    Text = "Manager",
                    ShowText = true,
                    ShowImage = true,
                    Size = RibbonItemSize.Large,
                    IsEnabled = true,
                    CommandParameter = "SBTOOL ",
                    CommandHandler = Handler,
                    ToolTip = sbToolTip
                };

                try
                {
                    btnSbTool.Image = LoadImageFromResource("sbtool_16.png");
                    btnSbTool.LargeImage = LoadImageFromResource("sbtool.png");
                }
                catch { }

                source1.Items.Add(btnSbTool);
                newTab.Panels.Add(panel1);

                // ==========================================
                // 4. PANEL MODIFY (MIRROR BLOCK)
                // ==========================================
                RibbonPanelSource source2 = new RibbonPanelSource
                {
                    Title = "MODIFY"
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
                    Image = LoadImageFromResource("mirror-image.png")
                };

                RibbonButton btnMirror = new RibbonButton
                {
                    Id = MirrorButtonId,
                    Text = "Mirror Block",
                    ShowText = true,
                    ShowImage = true,
                    Size = RibbonItemSize.Large,
                    IsEnabled = true,
                    CommandParameter = "MMI ",
                    CommandHandler = Handler,
                    ToolTip = mirrorToolTip
                };

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

                Log("\n[SBTools] Ribbon created successfully with separate LICENSE panel.");
            }
            catch (System.Exception ex)
            {
                Log("\n[SBTools Ribbon ERROR]: " + ex.Message);
            }
        }

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
                        bitmap.Freeze();
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