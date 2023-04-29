
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Notatki
{
    /// <summary>
    /// Logika interakcji dla klasy SelectedFiles.xaml
    /// </summary>
    public partial class SelectedFiles : Window
    {
        Icon set;
        private List<string> filestoDelete;
        functions functions;
        ObservableCollection<Folder> foldery = new ObservableCollection<Folder>();
        ObservableCollection<Filee> filess = new ObservableCollection<Filee>();
        private TextBox pathContent;
        FileInfo file;
        CancellationTokenSource ts = new CancellationTokenSource();
        Thread thread;
       Folders openFolder;
      CancellationToken token= new CancellationToken();
        public SelectedFiles()
        {
           
        }
        private async void Load()
        {

            int filesCount = 0;
            int folderCount = 0;


            await Task.Run(() => {
                thread = new Thread(() =>
                {
                   

                    
               
                        try
                        {
                            foreach (string x in filestoDelete)
                            {

                                if (Directory.Exists(x))
                                {
                                    folderCount++;
                                System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                                {
                                    foldery.Add(new Folder() { ID = folderCount, Name = x.Split("\\").Last(), Path = x });
                                });
                                }
                                else
                                {
                                    filesCount++;
                                    file = new FileInfo(x);
                                    if (file.Extension == ".jpg" || file.Extension == ".png" || file.Extension == ".jfif" || file.Extension == ".jpeg")
                                    {
                                    System.Drawing.Image img=null;
                                    try
                                    {
                                        img = System.Drawing.Image.FromFile(x);
                                    }
                                    catch (System.OutOfMemoryException exe) { }
                                    catch (FileNotFoundException notFound) { }
                                        System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                                        {
                                            filess.Add(new Filee() { ID = filesCount, Name = file.Name, Path = x, Extension = file.Extension, Width = img.Width, Height = img.Height });
                                        });
                                    }
                                    else
                                    {
                                        System.Windows.Application.Current.Dispatcher.Invoke(async delegate
                                        {
                                            filess.Add(new Filee() { ID = filesCount, Name = file.Name, Path = x, Extension = file.Extension, Size = (int)((file.Length) / 1024) / 1024 });
                                        });
                                    }

                                }

                            }

                        }catch(System.InvalidOperationException) { }
                 
                      


                    
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            },token);



            

         

            
        }

        public SelectedFiles(List<string> filestoDelete, TextBox pathContent, Folders folder, System.Threading.CancellationTokenSource ts, Icon set, functions functions)
        {

            this.set = set;
          
            InitializeComponent();

            this.DataContext = this.set;
            folders.DataContext = this.set;
            ts = new CancellationTokenSource();
            token = ts.Token;
            this.openFolder = folder;
            this.pathContent = pathContent;
            this.filestoDelete = filestoDelete;
            this.functions = functions;
          
            List<string> list = new List<string>();


            files.CanUserAddRows = false;
            folders.CanUserAddRows = false;
            Load();
           
         

            files.ItemsSource = filess;
            
            folders.ItemsSource = foldery;
           
          
           

        }

        

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Filee obj = ((FrameworkElement)sender).DataContext as Filee;
            functions.Templattte_Click1(sender, e, obj.Name, pathContent); ;

        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
          
        }

        private void deleteFile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            Filee obj = ((FrameworkElement)sender).DataContext as Filee;
         
                if (MessageBox.Show("Are you sure you want to delete this file?",
                                      obj.Name,
                                      MessageBoxButton.YesNo,
                                      MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                try
                {
                    System.IO.File.Delete(obj.Path);
                    filess.Remove(obj);

                }catch(System.IO.IOException ex) {
                   
                   ;
                }
                   

                }

            }

        private void deleteFolder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Folder obj = ((FrameworkElement)sender).DataContext as Folder;

            if (MessageBox.Show("Are you sure you want to delete this folder?",
                                  obj.Name,
                                  MessageBoxButton.YesNo,
                                  MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Directory.Delete(obj.Path, true);

                    foldery.Remove(obj);

                }
                catch (System.UnauthorizedAccessException denied) {
                    MessageBox.Show("Directory or file "+obj.Path+" is is denied or is only to read");
                    if (MessageBox.Show("Directory or file "+obj.Path+" is is denied or is only to read. Do you whant change attribute?",
                                obj.Name,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {


                        ForceDeleteDirectory(obj.Path);

                        foldery.Remove(obj);

                    }
                    }


               

            }

        }
        public static void ForceDeleteDirectory(string path)
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", System.IO.SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }
        private void cycki_Closing(object sender, CancelEventArgs e)
        {
          


            filestoDelete.Clear();
        }

        private void openFolder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Folder obj = ((FrameworkElement)sender).DataContext as Folder;
            openFolder.Tss.Cancel();
            openFolder.Tss.Dispose();
            ts = new CancellationTokenSource();
            openFolder.getFolder(obj.Path, ts = new CancellationTokenSource());
        }

       async void send_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Filee obj = ((FrameworkElement)sender).DataContext as Filee;
            using (var win = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult reslut=win.ShowDialog();

                if (reslut == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(win.SelectedPath))
                {

                    Dictionary<string, string> filesDict = new Dictionary<string, string>();
                 
                        int z = obj.Path.Split("\\").Count();

                        if (Directory.Exists(obj.Path))
                        {

                            Dictionary<string, string> dict = CopyDirectory(obj.Path, pathContent.Text + "\\" +obj.Path.Split("\\")[z - 1]);
                            await functions.Copier.CopyFiles(dict, ( file, filesize, totalsize, lab) => { fileName.Text = "File: " + file; fileSize.Text = "Size " + filesize + " MB/" + totalsize + " MB"; });

                        }
                        else
                        {

                            filesDict.Add(obj.Path,win.SelectedPath+"\\"+obj.Name);

                            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                            await functions.Copier.CopyFiles(filesDict, ( file, filesize, totalsize, lab) => {  fileName.Text = "Plik: "+file; fileSize.Text = filesize + " MB/" + totalsize + " MB"; progressBar.Maximum = totalsize; progressBar.Value = filesize; });
                        }

                          ;
                    }







                   
                   

                    System.Windows.Forms.MessageBox.Show(win.SelectedPath);
                

            }
        }
        async void send_FolderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Folder obj = ((FrameworkElement)sender).DataContext as Folder;
            using (var win = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult reslut = win.ShowDialog();

                if (reslut == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(win.SelectedPath))
                {

               
                            Dictionary<string, string> filesDict = new Dictionary<string, string>();

                        int z = obj.Path.Split("\\").Count();

                        if (Directory.Exists(obj.Path))
                        {
                            string dir = win.SelectedPath + "\\" + obj.Name;
                            Directory.CreateDirectory(dir);
                            Dictionary<string, string> dict = CopyDirectory(obj.Path, dir);
                            await functions.Copier.CopyFiles(dict, (file, filesize, totalsize, lab) => { fileName.Text = "File: " + file; fileSize.Text = "Size " + filesize + " MB/" + totalsize + " MB"; });

                        }
                        else
                        {

                            filesDict.Add(obj.Path, win.SelectedPath + "\\" + obj.Name);

                            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                            await functions.Copier.CopyFiles(filesDict, (file, filesize, totalsize, lab) => { fileName.Text = file; fileSize.Text = filesize + " MB/" + totalsize + " MB"; });
                        }
                    }
                  
                  

                          ;
                }










             


            
        }
        Dictionary<string, string> dic = new Dictionary<string, string>();

        private Dictionary<string, string> CopyDirectory(string source, string target)
        {

            DirectoryInfo info;
            info = new DirectoryInfo(source);
            try
            {
                foreach (FileInfo file in info.GetFiles())
                {



                    Directory.CreateDirectory(target);
                    dic.Add(source + "\\" + file.Name, target + "\\" + file.Name);

                }
                foreach (DirectoryInfo directory in info.GetDirectories())
                {
                    string dir = target + "\\" + directory.Name;
                    Directory.CreateDirectory(dir);

                    CopyDirectory(directory.FullName, dir);


                }

            }
            catch (System.IO.DirectoryNotFoundException error)
            {

                dic.Add(source, target);
            }

            return dic;
        }

        private void stopCopy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ts.Cancel();
            ts.Dispose();

        }

        private void deleteAllFolders_MouseLeftButtonDown<T>(T sender, T e)
        {
            if (MessageBox.Show("Are you sure you want to delete this all folders?",
                               "Delete All",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<Folder> toDelete = new List<Folder>();
                foreach (Folder x in foldery)
                {
                    toDelete.Add(x);
                    System.IO.Directory.Delete(x.Path,true);
                }
                foreach (Folder x in toDelete)
                {
                    foldery.Remove(x);
                }
            }
        }

        private void deleteAllFiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all this files?",
                               "Delete All",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<Filee> toDelete= new List<Filee>();
             foreach(Filee x in filess)

                {
                    try
                    {

                        toDelete.Add(x);
                        System.IO.File.Delete(x.Path);
                    }
                    catch(System.IO.IOException process)
                    {
                       ;
                        foreach(Process pro in Win32Processes.GetProcessesLockingFile(x.Path))
                        {
                            System.Diagnostics.Debug.WriteLine(pro.ProcessName);

                        }
                        MessageBox.Show("File "+x.Path+" is being used by another prcess.");
                    }
                   
                   
                }
             foreach(Filee x in toDelete)
                {
                    filess.Remove(x);
                }
            }
        }

        private async void sendAllFiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            using (var win = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult reslut = win.ShowDialog();

                if (reslut == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(win.SelectedPath))
                {
                    foreach (Filee x in filess)
                    {


                        Dictionary<string, string> filesDict = new Dictionary<string, string>();



                        if (Directory.Exists(x.Path))
                        {

                            Dictionary<string, string> dict = CopyDirectory(x.Path, pathContent.Text + "\\" + x.Path.Split("\\").Last());
                            await functions.Copier.CopyFiles(dict, (file, filesize, totalsize, lab) => { fileName.Text = "File: " + file; fileSize.Text = "Size " + filesize + " MB/" + totalsize + " MB"; });

                        }
                        else
                        {

                            filesDict.Add(x.Path, win.SelectedPath + "\\" + x.Name);

                            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                            await functions.Copier.CopyFiles(filesDict, (file, filesize, totalsize, lab) => { fileName.Text = "Plik: " + file; fileSize.Text = filesize + " MB/" + totalsize + " MB"; });
                        }
                    }

                          ;
                }










              


            }
        }

        private async void sebdAllFolders_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ts.Cancel();
            ts.Dispose();
         

            
            using (var win = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult reslut = win.ShowDialog();

                if (reslut == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(win.SelectedPath))
                {
                    foreach (Folder x in foldery)
                    {


                        Dictionary<string, string> filesDict = new Dictionary<string, string>();



                        if (Directory.Exists(x.Path))
                        {

                            Dictionary<string, string> dict = CopyDirectory(x.Path, win.SelectedPath+"\\"+x.Name);
                            await functions.Copier.CopyFiles(dict, (file, filesize, totalsize, lab) => { fileName.Text = "File: " + file; fileSize.Text = "Size " + filesize + " MB/" + totalsize + " MB"; });

                        }
                        else
                        {

                            filesDict.Add(x.Path, win.SelectedPath + "\\" + x.Name);

                            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
                            await functions.Copier.CopyFiles(filesDict, (file, filesize, totalsize, lab) => { fileName.Text = "Plik: " + file; fileSize.Text = filesize + " MB/" + totalsize + " MB"; });
                        }
                    }

                          ;
                }













            }
        }
    }
    }
public static class Win32Processes
{
    /// <summary>
    /// Find out what process(es) have a lock on the specified file.
    /// </summary>
    /// <param name="path">Path of the file.</param>
    /// <returns>Processes locking the file</returns>
    /// <remarks>See also:
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
    /// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
    /// </remarks>
    public static List<Process> GetProcessesLockingFile(string path)
    {
        uint handle;
        string key = Guid.NewGuid().ToString();
        int res = RmStartSession(out handle, 0, key);

        if (res != 0) throw new Exception("Could not begin restart session.  Unable to determine file locker.");

        try
        {
            const int MORE_DATA = 234;
            uint pnProcInfoNeeded, pnProcInfo = 0, lpdwRebootReasons = RmRebootReasonNone;

            string[] resources = { path }; // Just checking on one resource.

            res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

            if (res != 0) throw new Exception("Could not register resource.");

            //Note: there's a race condition here -- the first call to RmGetList() returns
            //      the total number of process. However, when we call RmGetList() again to get
            //      the actual processes this number may have increased.
            res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

            if (res == MORE_DATA)
            {
                return EnumerateProcesses(pnProcInfoNeeded, handle, lpdwRebootReasons);
            }
            else if (res != 0) throw new Exception("Could not list processes locking resource. Failed to get size of result.");
        }
        finally
        {
            RmEndSession(handle);
        }

        return new List<Process>();
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct RM_UNIQUE_PROCESS
    {
        public int dwProcessId;
        public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
    }

    const int RmRebootReasonNone = 0;
    const int CCH_RM_MAX_APP_NAME = 255;
    const int CCH_RM_MAX_SVC_NAME = 63;

    public enum RM_APP_TYPE
    {
        RmUnknownApp = 0,
        RmMainWindow = 1,
        RmOtherWindow = 2,
        RmService = 3,
        RmExplorer = 4,
        RmConsole = 5,
        RmCritical = 1000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct RM_PROCESS_INFO
    {
        public RM_UNIQUE_PROCESS Process;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)] public string strAppName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)] public string strServiceShortName;

        public RM_APP_TYPE ApplicationType;
        public uint AppStatus;
        public uint TSSessionId;
        [MarshalAs(UnmanagedType.Bool)] public bool bRestartable;
    }

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames,
        uint nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, uint nServices,
        string[] rgsServiceNames);

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
    static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

    [DllImport("rstrtmgr.dll")]
    static extern int RmEndSession(uint pSessionHandle);

    [DllImport("rstrtmgr.dll")]
    static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded,
        ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
        ref uint lpdwRebootReasons);

    private static List<Process> EnumerateProcesses(uint pnProcInfoNeeded, uint handle, uint lpdwRebootReasons)
    {
        var processes = new List<Process>(10);
        // Create an array to store the process results
        var processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
        var pnProcInfo = pnProcInfoNeeded;

        // Get the list
        var res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);

        if (res != 0) throw new Exception("Could not list processes locking resource.");
        for (int i = 0; i < pnProcInfo; i++)
        {
            try
            {
                processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
            }
            catch (ArgumentException) { } // catch the error -- in case the process is no longer running
        }
        return processes;
    }
}
public class Folder
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
    public class Filee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Size { get; set; }
      

    }
    



