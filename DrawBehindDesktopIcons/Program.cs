using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawBehindDesktopIcons
{
    
    public class Program
        
    {
         public static Image img = null;
        public static Form form = new Form();
        [STAThread]
        public static void Main(string[] args)
        {

            PrintVisibleWindowHandles(5);
            // The output will look something like this. 
            // .....
            // 0x00010190 "" WorkerW
            //   ...
            //   0x000100EE "" SHELLDLL_DefView
            //     0x000100F0 "FolderView" SysListView32
            // 0x000100EC "Program Manager" Progman



            // Fetch the Progman window
            IntPtr progman = W32.FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;

            // Send 0x052C to Progman. This message directs Progman to spawn a 
            // WorkerW behind the desktop icons. If it is already there, nothing 
            // happens.
            W32.SendMessageTimeout(progman,
                                   0x052C,
                                   new IntPtr(0),
                                   IntPtr.Zero,
                                   W32.SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out result);


            PrintVisibleWindowHandles(5);
            // The output will look something like this
            // .....
            // 0x00010190 "" WorkerW
            //   ...
            //   0x000100EE "" SHELLDLL_DefView
            //     0x000100F0 "FolderView" SysListView32
            // 0x00100B8A "" WorkerW                                   <--- This is the WorkerW instance we are after!
            // 0x000100EC "Program Manager" Progman

            IntPtr workerw = IntPtr.Zero;

            // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
            // as a child. 
            // If we found that window, we take its next sibling and assign it to workerw.
            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = W32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerw = W32.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);

            // We now have the handle of the WorkerW behind the desktop icons.
            // We can use it to create a directx device to render 3d output to it, 
            // we can use the System.Drawing classes to directly draw onto it, 
            // and of course we can set it as the parent of a windows form.
            //
            // There is only one restriction. The window behind the desktop icons does
            // NOT receive any user input. So if you want to capture mouse movement, 
            // it has to be done the LowLevel way (WH_MOUSE_LL, WH_KEYBOARD_LL).


            // Demo 1: Draw graphics between icons and wallpaper

            // Get the Device Context of the WorkerW
            IntPtr dc = W32.GetDCEx(workerw, IntPtr.Zero, (W32.DeviceContextValues)0x403);
            if (dc != IntPtr.Zero)
            {
               
                W32.ReleaseDC(workerw, dc);
            }

            // Demo 2: Demo 2: Put a Windows Form behind desktop icons
            //PUT FORM MANIPULATION DATA HERE FOR YOUR BACKGROUND ENGINE!<<<<<
            
            form.Text = "Test Window";
            form.Name = "jerry";
            form.Load += new EventHandler((s, e) =>
            {
                // Move the form right next to the in demo 1 drawn rectangle
                

                form.Width = Screen.PrimaryScreen.Bounds.Width+15;
                form.Height = Screen.PrimaryScreen.Bounds.Height;
                form.Left = 0;
                form.Top = 0;
                form.ControlBox = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
                form.Name = "jerry";

                PictureBox imageControl = new PictureBox
                {
                    Width = Screen.PrimaryScreen.Bounds.Height,
                    Height = Screen.PrimaryScreen.Bounds.Height,
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Name = "pictureBox1"
                };
                Image imgd = img;
                imageControl.Image = imgd;
                form.Controls.Add(imageControl);
                
                

                // Add a randomly moving button to the form


                // Those two lines make the form a child of the WorkerW, 
                // thus putting it behind the desktop icons and out of reach 
                // for any user intput. The form will just be rendered, no 
                // keyboard or mouse input will reach it. You would have to use 
                // WH_KEYBOARD_LL and WH_MOUSE_LL hooks to capture mouse and 
                // keyboard input and redirect it to the windows form manually, 
                // but thats another story, to be told at a later time.//>>>>> DATA MANIPULATION FOR FORM ENDS HERE DONT TOUCH OTHER THINGS!
                W32.SetParent(form.Handle, workerw);
              
            });
           
            Application.Run(form);
           
            // Start the Application Loop for the Form. // DAT
           // Application.Run(form);
        }

        static void PrintVisibleWindowHandles(IntPtr hwnd, int maxLevel=-1, int level=0)
        {
            bool isVisible = W32.IsWindowVisible(hwnd);

            if (isVisible && (maxLevel==-1||level<=maxLevel))
            {
                StringBuilder className = new StringBuilder(256);
                W32.GetClassName(hwnd, className, className.Capacity);

                StringBuilder windowTitle = new StringBuilder(256);
                W32.GetWindowText(hwnd, windowTitle, className.Capacity);

                Console.WriteLine("".PadLeft(level*2)+"0x{0:X8} \"{1}\" {2}", hwnd.ToInt64(), windowTitle, className);

                level++;

                // Enumerates all child windows of the current window
                W32.EnumChildWindows(hwnd, new W32.EnumWindowsProc((childhandle, childparamhandle) =>
                {
                    PrintVisibleWindowHandles(childhandle, maxLevel, level);
                    return true;
                }), IntPtr.Zero);
            }            
        }
        static void PrintVisibleWindowHandles(int maxLevel=-1)
        {
            // Enumerates all existing top window handles. This includes open and visible windows, as well as invisible windows.
            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                PrintVisibleWindowHandles(tophandle, maxLevel);
                return true;
            }), IntPtr.Zero);
        }
        public static void SetwallpaperinDBDI(string path)
        {
            img = Image.FromFile(path.ToString()); 



        }
    }
    
}
