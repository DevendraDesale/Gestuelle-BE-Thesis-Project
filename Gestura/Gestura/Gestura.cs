#region Library Files
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DirectX.Capture;
using Microsoft.VisualBasic;
using System.Xml;
using NUnit.Framework;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading;
using DirectShowLib;
using SnapShot;
using System.Runtime.InteropServices;
#endregion

namespace Gestura
{
    public partial class Gestura : Form
    {
        #region Variable and Initializations
        public static int index;
        private Capture capture = null;
        private Filters filters = new Filters();
        IntPtr m_ip = IntPtr.Zero;
        private Capture1[] cam=new Capture1[20];
        public static int prog_iterations = 0;
        public static int[,] cntr = new int[50, 3];
         public static int th_counter = 0, th_counter2 = 0, th_counter3 = 0; 
        private static ImgProc[] obj = new ImgProc[40];
        private static Thread[] MyThreads = new Thread[20];
        private static Thread[] Set_Threads = new Thread[3];
        public static Thread capture_thread;
        public static Thread[,] img_process = new Thread[20,3];
        const int VIDEODEVICE = 0; // zero based index of video capture device to use
        const int VIDEOWIDTH = 640; // Depends on video device caps
        const int VIDEOHEIGHT = 480; // Depends on video device caps
        const int VIDEOBITSPERPIXEL = 24; // BitsPerPixel values determined by device
        const long FRAMERATE = 8330;
        public static int clicks = 0;
        public static Boolean start_gesture = false;
        public byte[] rgbValues;
        public String num, temp, temp2;
        public int width = 128;
        public int height = 128;
        public static Image[] image = new Image[20];
        public Bitmap original;
        public Bitmap inputBitmap;
        public Bitmap bit_img;
        private DataProcess dataProcess = new DataProcess();
        public int cnt_scan = 0;
        #endregion

        #region Constructor
        public Gestura()
        {
            InitializeComponent();

            // Start with the first video devices
            // Don't do this in the Release build in case the
            // first devices cause problems.
            #if DEBUG
                capture = new Capture(filters.VideoInputDevices[0], filters.AudioInputDevices[0]);
                capture.CaptureComplete += new EventHandler(OnCaptureComplete);
            #endif

            // Update the main menu
            try 
            {
                updateMenu(); 
            }
            catch { }

            /* start preview automatically */
            try
            {
                cam[prog_iterations] = new Capture1(VIDEODEVICE, VIDEOWIDTH, VIDEOHEIGHT, VIDEOBITSPERPIXEL, panel1, FRAMERATE);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to enable/disable preview. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }
        #endregion

        #region UI Click And Load Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gestura_Load(object sender, EventArgs e)
        {
            label1.Text = "Gestuelle is Scanning...";
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
           for (int i = 0; i < 40; i++)
                obj[i] = new ImgProc();

            clicks = 0;
            timer3.Start();    
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            timer3.Start();
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            SpeechSynthesizer reader = new SpeechSynthesizer();
            reader.Speak("See You Later.");
            reader.Dispose();
            timer3.Stop();

            if (cam[prog_iterations] != null)
                    cam[prog_iterations].Dispose();
            Application.Exit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capture != null)
                capture.Stop();
            SpeechSynthesizer reader = new SpeechSynthesizer();
            reader.Speak("See You Later.");
            reader.Dispose();

            Application.Exit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuPreview_Click(object sender, EventArgs e)
        {
            try
            {
               cam[prog_iterations] = new Capture1(VIDEODEVICE, VIDEOWIDTH, VIDEOHEIGHT, VIDEOBITSPERPIXEL, panel1,FRAMERATE);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to enable/disable preview. Please submit a bug report.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        /// <summary>
        /// This module opens the respective applications as per the user's choice.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (windowsMediaPlayerToolStripMenuItem.Selected)
                index = 0;
            else if (windowsLiveGalleryToolStripMenuItem.Selected)
                index = 1;
            else if (microsoftPowerPointToolStripMenuItem.Selected)
                index = 2;
            else if (notepadToolStripMenuItem.Selected)
                index = 3;
            else if (preziToolStripMenuItem.Selected)
                index = 4;

            switch (index)
            {
                case 0: Process.Start("C:\\Program Files\\Windows Media Player\\wmplayer.exe");
                        Process[] processes = Process.GetProcessesByName("C:\\Program Files\\Windows Live\\Photo Gallery\\WLXPhotoGallery.exe");
                        break;

                case 1: Process.Start("C:\\Program Files\\Windows Live\\Photo Gallery\\WLXPhotoGallery.exe");
                        break;

                case 2: Process.Start("C:\\Program Files\\Microsoft Office\\Office12\\POWERPNT.exe");
                        break;

                case 3: Process.Start("notepad.exe");
                        break;

                case 4: Process.Start(@"C:\Users\Admin\Desktop\Project\dhgr-mrie5p7dshcq\dhgr-mrie5p7dshcq-089_150140_297856\prezi.exe");
                        break;
            }
        }
                        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void devicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Get current devices and dispose of capture object
                // because the video and audio device can only be changed
                // by creating a new Capture object.
                Filter videoDevice = null;
                Filter audioDevice = null;
                if (capture != null)
                {
                    videoDevice = capture.VideoDevice;
                    audioDevice = capture.AudioDevice;
                    capture.Dispose();
                    capture = null;
                }

                // Get new video device
                MenuItem m = sender as MenuItem;
                videoDevice = (m.Index > 0 ? filters.VideoInputDevices[m.Index - 1] : null);

                // Create capture object
                if ((videoDevice != null) || (audioDevice != null))
                {
                    capture = new Capture(videoDevice, audioDevice);
                    capture.CaptureComplete += new EventHandler(OnCaptureComplete);
                }

                // Update the menu
                updateMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Video device not supported.\n\n" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void developerModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capture != null)
                capture.Stop();
            Markov.MainForm dev_mode = new Markov.MainForm();
            dev_mode.Show();
            this.Hide();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watchTuorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capture != null)
                capture.Stop();

            Tutorial tut_form = new Tutorial();
            tut_form.Show();
            this.Hide();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (capture != null)
                capture.Stop();

            Instructions instr_form = new Instructions();
            instr_form.Show();
            this.Hide();
        }
        #endregion       

        #region Camera Initialization and Menu Update Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCaptureComplete(object sender, EventArgs e)
        {
            // Demonstrate the Capture.CaptureComplete event.
            Debug.WriteLine("Capture complete.");
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateMenu()
        {
            ToolStripMenuItem m;
            Filter f;
            Control oldPreviewWindow = null;

            // Load video devices
            Filter videoDevice = null;
            if (capture != null)
                videoDevice = capture.VideoDevice;
            mnuVideoDevices.DropDownItems.Clear();

            m = new ToolStripMenuItem("(None)", null, new EventHandler(devicesToolStripMenuItem_Click));
            m.Checked = (videoDevice == null);
            mnuVideoDevices.DropDownItems.Add(m);
            for (int c = 0; c < filters.VideoInputDevices.Count; c++)
            {
                f = filters.VideoInputDevices[c];
                m = new ToolStripMenuItem(f.Name, null, new EventHandler(devicesToolStripMenuItem_Click));
                m.Checked = (videoDevice == f);
                mnuVideoDevices.DropDownItems.Add(m);
            }
            mnuVideoDevices.Enabled = (filters.VideoInputDevices.Count > 0);

            // Check Preview menu option
            mnuPreview.Checked = (oldPreviewWindow != null);
            mnuPreview.Enabled = (capture != null);

            // Reenable preview if it was enabled before
            if (capture != null)
                capture.PreviewWindow = oldPreviewWindow;
        }
        #endregion

        #region Timer Functions
        /// <summary>
        /// 
        /// </summary>
        public void begin_timer3()
        {
            for (int i = 0; i < 40; i++)
                obj[i] = new ImgProc();
            timer3.Start();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer3_Tick(object sender, EventArgs e)
        {
            String num, temp2, temp;
            for (int i = 0; i < 5; i++)
            {
                Cursor.Current = Cursors.WaitCursor;

                // Release any previous buffer
                if (m_ip != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(m_ip);
                    m_ip = IntPtr.Zero;
                }

                // capture image
                m_ip = cam[prog_iterations].Click();

                Bitmap b = new Bitmap(cam[prog_iterations].Width, cam[prog_iterations].Height, cam[prog_iterations].Stride, PixelFormat.Format24bppRgb, m_ip);
                Bitmap c = new Bitmap(b, 128, 128);

                // If the image is upsidedown
                c.RotateFlip(RotateFlipType.RotateNoneFlipY);
                cnt_scan++;
                label2.Text = cnt_scan.ToString();
                temp2 = ".jpg";
                temp = "D:\\Images\\frame";
                num = clicks.ToString();
                temp2 = num + temp2;
                temp = temp + temp2;
                c.Save(temp);
                b.Dispose();
                c.Dispose();
                GC.Collect();
                if (clicks % 4 == 0)
                {
                    Step_A(clicks);
                    Step_B(clicks);
                    Step_C(clicks);
                }
                if (clicks <= 4)
                    Thread.Sleep(200);
                else
                    Thread.Sleep(125);

                clicks++;
                if (clicks >= 30)
                    break;
            }
            Cursor.Current = Cursors.Default;

            if (clicks >= 28)
            {
                timer3.Stop();
                pattern_recognition();
                cam[prog_iterations].Dispose();
                this.Dispose();
                this.Close();
                prog_iterations++;
                Gestura new_form = new Gestura();
                new_form.Show();
            }
        }
        #endregion

        #region Pattern Matching
        /// <summary>
        /// 
        /// </summary>
        public void pattern_recognition()
        {
            ImgProc main_obj = new ImgProc();
            MarkovModelCalculation hmm_obj = new MarkovModelCalculation();
            int count = 39;

            System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\images5\\Centroid.txt");
            for (int i = 4; i < count; i++)
                file.WriteLine("Frame {0}: {1}, {2}, Quad: {3}", i, cntr[i, 0], cntr[i, 1], cntr[i, 2]);
            file.Close();

            /* Logic to calculate the state sequence from the captured image frames */
            System.IO.StreamWriter file2 = new System.IO.StreamWriter("D:\\images5\\State_seq.txt");
            System.IO.StreamWriter file4 = new System.IO.StreamWriter("D:\\images5\\testing_ab.txt");
            int beta, a, b, Q;
            double alpha;
            int[] O = new int[10];
            int[] O2 = new int[5];

            for (int k = 0; k < 10; k++)
            {
                O[k] = 1;
                if (k % 2 == 0) 
                    O2[k/2] = 1;
            }
            /* for O */
            for (int i = 0, j = 0; i <=24; i += 4, j++)
            {
                b = cntr[i + 4, 0] - cntr[i, 0];
                a = cntr[i + 4, 1] - cntr[i, 1];
                
                if (b == 0)
                    alpha = 90;
                else
                {
                    alpha = Math.Atan2((Math.Abs(a)),(Math.Abs(b)));
                    alpha = alpha * 180 / 3.14159;
                }
                file4.WriteLine("Frame {0}-{1}: a = {2} b: {3} alpha: {4}", i, i + 4, a, b,alpha);
                beta = alpha > 45 ? ((90 - alpha > alpha - 45) ? 45 : 90) : ((alpha > 45 - alpha) ? 45 : 0);

                if (a >= 0 && b >= 0) //initially 1,2,4,3
                    Q = 4;
                else if (a >= 0 && b < 0)
                    Q = 3;
                else if (a < 0 && b >= 0)
                    Q = 1;
                else
                    Q = 2;


                switch (beta)
                {
                    case 0: if (Q == 1 || Q == 4)
                            O[j] = 1;
                        else
                            O[j] = 5;
                        break;

                    case 45: if (Q == 1)
                            O[j] = 8;
                        else if (Q == 2)
                            O[j] = 6;
                        else if (Q == 3)
                            O[j] = 4;
                        else
                            O[j] = 2;
                        break;

                    case 90: if (Q == 1 || Q == 2)
                            O[j] = 7;
                        else
                            O[j] = 3;
                        break;
                }
                file2.WriteLine("Frame {0}-{1}: State Value = {2} Beta: {3}   Alpha: {4}", i, i + 4, O[j], beta, alpha);
            }
            file2.Close();
            file4.Close();

            /* Viterbi Algorithm */
            System.IO.StreamWriter file3 = new System.IO.StreamWriter("D:\\images5\\Viterbi_Results.txt");
            double prob = 0.0;
            int label = 0;
            int[][] labels = new int[8][];
            int[] final_obs_seq=new int[13];
            for(int k=0,j=0;k<O.Length;k++,j++)
            {
                if (k <= 5)
                {
                    final_obs_seq[j] = O[k];
                    final_obs_seq[++j] = O[k];
                }
                else if (k == 6)
                    final_obs_seq[j] = O[k];
                else
                    break;
            }

            for (int j = 0; j < 8; j++)
            {
                double p = hmm_obj.Evaluate(final_obs_seq, j + 1);
                
                // And select the one which produces the highest probability
                if (p > prob)
                {
                    label = j;
                    prob = p;
                }
            }
            
            using (StreamWriter xfile = File.AppendText("D:\\images5\\Testing_Results.txt"))
            {
                int s;
                for (s = 0; s < (final_obs_seq.Length) - 1; s++)
                {
                    xfile.Write((final_obs_seq[s]-1) + "-");
                }
                xfile.Write((final_obs_seq[s]-1));
                xfile.WriteLine("");
                xfile.Close();
            }   
           
            int[] result2 = labels[label];
            
            for (int i = 0; i < (final_obs_seq.Length); i++)
            {
                file3.Write(final_obs_seq[i] + " ");
            }
            file3.WriteLine("\nLabel: " + (label+1) + "   Probability: " + prob);
            file3.Close();
            label += 1;
            System.IO.StreamWriter file5 = new System.IO.StreamWriter("D:\\images5\\O_Count.txt");
            for(int new_i=0;new_i<O.Length;new_i++)
                file5.WriteLine("O[{0}]: {1}", new_i, O[new_i]);
            file5.Close();
            
                if(label==1)//if (O[8] == 1)
                {
                    MessageBox.Show("Left !!");
                    switch (index)
                    {
                        case 0: WMP_Control wmp = new WMP_Control();
                                wmp.ControlMediaPlayer("PreviousSong");
                                break;
                        case 1: WLPG_Control wlpg = new WLPG_Control();
                                wlpg.ControlPictureViewer("LeftImage");
                                break;
                        case 2: Powerpoint pp = new Powerpoint();
                                pp.ControlPowerpoint("Save");
                                break;
                        case 3: Notepad n = new Notepad();
                                n.ControlNotepad("Save");
                                break;
                        case 4: Prezi p = new Prezi();
                                p.ControlPrezi("Left");
                                break;
                    }
                }
                else if (label == 2)
                {
                    MessageBox.Show("Right !!");
                    switch (index)
                    {
                        case 0: WMP_Control wmp = new WMP_Control();
                                wmp.ControlMediaPlayer("NextSong");
                                break;
                        case 1: WLPG_Control wlpg = new WLPG_Control();
                                wlpg.ControlPictureViewer("RightImage");
                                break;
                        case 2: Powerpoint pp = new Powerpoint();
                                pp.ControlPowerpoint("Open");
                                break;
                        case 3: Notepad n = new Notepad();
                                n.ControlNotepad("Open");
                                break;
                        case 4: Prezi p = new Prezi();
                                p.ControlPrezi("Right");
                                break;
                    }
                }
                else if (label == 3)//else if (O[8] == 7)
                {
                    MessageBox.Show("Up !!");
                    switch (index)
                    {
                        case 0: WMP_Control wmp = new WMP_Control();
                                wmp.ControlMediaPlayer("Increase_Vol");
                                break;
                        case 1: WLPG_Control wlpg = new WLPG_Control();
                                wlpg.ControlPictureViewer("Print");
                                break;
                        case 2: Powerpoint pp = new Powerpoint();
                                pp.ControlPowerpoint("Print");
                                break;
                        case 3: Notepad n = new Notepad();
                                n.ControlNotepad("Print");
                                break;
                    }
                }
                else if (label == 4)//else if (O[8] == 3)
                {
                    MessageBox.Show("Down !!");
                    switch (index)
                    {
                        case 0: WMP_Control wmp = new WMP_Control();
                            wmp.ControlMediaPlayer("Decrease_Vol");
                            break;
                        case 1: WLPG_Control wlpg = new WLPG_Control();
                            wlpg.ControlPictureViewer("Slideshow");
                            break;
                        case 2: Powerpoint pp = new Powerpoint();
                            pp.ControlPowerpoint("Add_Slide");
                            break;
                        case 3: Notepad n = new Notepad();
                            n.ControlNotepad("Selectall");
                            break;
                    }
                }
                else if (label == 5)//else if (O[8] == 3)
                {
                    MessageBox.Show("Semi-Circle Clockwise !!");
                    switch (index)
                    {
                        case 0: WMP_Control wmp = new WMP_Control();
                            wmp.ControlMediaPlayer("Repeat");
                            break;
                        case 1: WLPG_Control wlpg = new WLPG_Control();
                            wlpg.ControlPictureViewer("Zoom_In");
                            break;
                        case 2: Powerpoint pp = new Powerpoint();
                            pp.ControlPowerpoint("Slideshow_Next");
                            break;
                        case 3: Notepad n = new Notepad();
                            n.ControlNotepad("Find");
                            break;
                    }
                }
                else if (label == 6)
                {
                    MessageBox.Show("Semi-Circle Anticlockwise  !!");
                    switch (index)
                    {
                        case 0: WMP_Control wmp = new WMP_Control();
                            wmp.ControlMediaPlayer("Shuffle");
                            break;
                        case 1: WLPG_Control wlpg = new WLPG_Control();
                            wlpg.ControlPictureViewer("Zoom_Out");
                            break;
                        case 2: Powerpoint pp = new Powerpoint();
                            pp.ControlPowerpoint("Slideshow_Previous");
                            break;
                        case 3: Notepad n = new Notepad();
                            n.ControlNotepad("Replace");
                            break;
                    }
                }
                else if (label == 7)
                {
                    MessageBox.Show("Circle Clockwise !!");
                    switch (index)
                    {
                        case 0: WMP_Control wmp = new WMP_Control();
                            wmp.ControlMediaPlayer("Play/Pause");
                            break;
                        case 1: WLPG_Control wlpg = new WLPG_Control();
                            wlpg.ControlPictureViewer("Rotate_Clock");
                            break;
                        case 2: Powerpoint pp = new Powerpoint();
                            pp.ControlPowerpoint("Slideshow_Begin");
                            break;
                        case 3: Notepad n = new Notepad();
                            n.ControlNotepad("Highlight");
                            break;
                    }
                }
                else if (label == 8)
                {
                    MessageBox.Show("Circle Anti-Clockwise !!");
                    switch (index)
                    {
                        case 0: WMP_Control wmp = new WMP_Control();
                            wmp.ControlMediaPlayer("Stop");
                            break;
                        case 1: WLPG_Control wlpg = new WLPG_Control();
                            wlpg.ControlPictureViewer("Rotate_Anti");
                            break;
                        case 2: Powerpoint pp = new Powerpoint();
                            pp.ControlPowerpoint("Slideshow_End");
                            break;
                        case 3: Notepad n = new Notepad();
                            n.ControlNotepad("Time_Date");
                            break;
                    }
                }
                else
                    MessageBox.Show("Processing Completed. Gesture Not Recognized !");
        }
        #endregion

        #region Image Processing Class
        /* ************************* Class Image Processing ************************************88 */
        public void Step_A(int i)
        {
            String temp = "D:\\Images\\frame";
            String num;
            num = i.ToString();
            String temp2 = ".jpg";
            temp2 = num + temp2;
            temp = temp + temp2;
            Image image;

            image = Image.FromFile(temp);
            original = new Bitmap(image, new Size(width, height));
    
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
            System.Drawing.Imaging.BitmapData bmpData = original.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, original.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            int scan0 = ptr.ToInt32();

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * original.Height;
            rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            rgbValues = RGBtoHSV(original, rgbValues, scan0, bmpData.Stride);
            // Set every third value to 255. A 24bpp bitmap will look red.  
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            // Unlock the bits.
            original.UnlockBits(bmpData);
            temp2 = ".bmp";
            temp = "D:\\images2\\frame";
            temp2 = num + temp2;
            temp = temp + temp2;
            original.Save(temp);
            original.Dispose();
        }

        public void Step_B(int i)
        {
            String temp = "D:\\images2\\frame";
            String num = i.ToString();
            String temp2 = ".bmp";
            temp2 = num + temp2;
            temp = temp + temp2;
            inputBitmap = new Bitmap(temp);
            Bitmap outBitmap = LoG12x12(inputBitmap);
            temp2 = ".bmp";
            temp = "D:\\images3\\frame";
            temp2 = num + temp2;
            temp = temp + temp2;
            outBitmap.Save(temp);
            outBitmap.Dispose();
            inputBitmap.Dispose();
            GC.Collect();
        }

        public void Step_C(int i)
        {
            String temp = "D:\\images3\\frame";
            String num = i.ToString();
            String temp2 = ".bmp";
            temp2 = num + temp2;
            temp = temp + temp2;
            bit_img = new Bitmap(temp);

            Bitmap original = new Bitmap(bit_img, new Size(width, height));

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
            System.Drawing.Imaging.BitmapData bmpData = original.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, original.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            int scan0 = ptr.ToInt32();

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * original.Height;
            //if (i == 0)
            rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            centroid(original, ref rgbValues, scan0, bmpData.Stride, ref cntr[i, 0], ref cntr[i, 1], ref cntr[i, 2]);
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            original.UnlockBits(bmpData);

            temp2 = ".bmp";
            temp = "D:\\images5\\frame";
            temp2 = num + temp2;
            temp = temp + temp2;
            original.Save(temp);
            original.Dispose();
            GC.Collect();
        }

        public byte[] RGBtoHSV(Bitmap original, byte[] rgbvalues, int scan0, int stride)
        {
            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    //get the pixel from the original image
                    //double originalColorR = rgbvalues[original.Width * i + j * 4];
                    double originalColorR = rgbvalues[(j * stride) + (i * 4) + 2];
                    double originalColorG = rgbvalues[(j * stride) + (i * 4) + 1];
                    double originalColorB = rgbvalues[(j * stride) + (i * 4)];

                    double max = Math.Max(originalColorR, originalColorG);
                    max = Math.Max(originalColorB, max);
                    double min = Math.Min(originalColorR, originalColorG);
                    min = Math.Min(originalColorB, min);

                    double s = (max == 0) ? 0.0 : (1.0 - (min / max));
                    double v = max;
                    double h;

                    double r1 = (max - originalColorR) / (max - min);
                    double g1 = (max - originalColorG) / (max - min);
                    double b1 = (max - originalColorB) / (max - min);

                    if (originalColorR == max && originalColorG == min)
                        h = 5 + b1;
                    else if (originalColorR == max && originalColorG >= min)
                        h = 1 - g1;
                    else if (originalColorG == max && originalColorB == min)
                        h = r1 + 1;
                    else if (originalColorG == max && originalColorB >= min)
                        h = 3 - b1;
                    else if (originalColorR == max)
                        h = 3 + g1;
                    else
                        h = 5 - r1;

                    h = h * 60;

                    /* Skin Detection */
                    if ((h >= 0 && h <= 50) && (s >= 0.32 && s <= 0.57))
                    {
                        h = 255;
                        s = 255;
                        v = 255;
                    }
                    else
                    {
                        h = s = v = 0;
                    }

                    //create the color object
                    Color newColor = Color.FromArgb((int)(Math.Round(h, MidpointRounding.AwayFromZero)), (int)(Math.Round(s, MidpointRounding.AwayFromZero)), (int)(Math.Round(v, MidpointRounding.AwayFromZero)));
                    rgbvalues[(j * stride) + (i * 4) + 2] = newColor.R;
                    rgbvalues[(j * stride) + (i * 4) + 1] = newColor.G;
                    rgbvalues[(j * stride) + (i * 4)] = newColor.B;
                }
            }
            return rgbvalues;
        }

        public static Bitmap LoG12x12(Bitmap SrcImage)
        {
            double[,] MASK = new double[12, 12] {
								{-0.000699762,	-0.000817119,	-0.000899703,	-0.000929447,	-0.000917118,	-0.000896245,	-0.000896245,	-0.000917118,	-0.000929447,	-0.000899703,	-0.000817119,	-0.000699762},
								{-0.000817119,	-0.000914231,	-0.000917118,	-0.000813449,	-0.000655442,	-0.000538547,	-0.000538547,	-0.000655442,	-0.000813449,	-0.000917118,	-0.000914231,	-0.000817119},
								{-0.000899703,	-0.000917118,	-0.000745635,	-0.000389918,	0.0000268,	0.000309618,	0.000309618,	0.0000268,	-0.000389918,	-0.000745635,	-0.000917118,	-0.000899703},
								{-0.000929447,	-0.000813449,	-0.000389918,	0.000309618,	0.001069552,	0.00156934,	0.00156934,	0.001069552,	0.000309618,	-0.000389918,	-0.000813449,	-0.000929447},
								{-0.000917118,	-0.000655442,	0.0000268,	0.001069552,	0.002167033,	0.002878738,	0.002878738,	0.002167033,	0.001069552,	0.0000268,	-0.000655442,	-0.000917118},
								{-0.000896245,	-0.000538547,	0.000309618,	0.00156934,	0.002878738,	0.003722998,	0.003722998,	0.002878738,	0.00156934,	0.000309618,	-0.000538547,	-0.000896245},
								{-0.000896245,	-0.000538547,	0.000309618,	0.00156934,	0.002878738,	0.003722998,	0.003722998,	0.002878738,	0.00156934,	0.000309618,	-0.000538547,	-0.000896245},
								{-0.000917118,	-0.000655442,	0.0000268,	0.001069552,	0.002167033,	0.002878738,	0.002878738,	0.002167033,	0.001069552,	0.0000268,	-0.000655442,	-0.000917118},
								{-0.000929447,	-0.000813449,	-0.000389918,	0.000309618,	0.001069552,	0.00156934,	0.00156934,	0.001069552,	0.000309618,	-0.000389918,	-0.000813449,	-0.000929447},
								{-0.000899703,	-0.000917118,	-0.000745635,	-0.000389918,	0.0000268,	0.000309618,	0.000309618,	0.0000268,	-0.000389918,	-0.000745635,	-0.000917118,	-0.000899703},
								{-0.000817119,	-0.000914231,	-0.000917118,	-0.000813449,	-0.000655442,	-0.000538547,	-0.000538547,	-0.000655442,	-0.000813449,	-0.000917118,	-0.000914231,	-0.000817119},
								{-0.000699762,	-0.000817119,	-0.000899703,	-0.000929447,	-0.000917118,	-0.000896245,	-0.000896245,	-0.000917118,	-0.000929447,	-0.000899703,	-0.000817119,	-0.000699762}
							};

            double nTemp = 0.0;
            double c = 0;

            int mdl, size;
            size = 12;
            mdl = size / 2;

            double min, max;
            min = max = 0.0;

            double sum = 0.0;
            double mean;
            double d = 0.0;
            double s = 0.0;
            int n = 0;

            Bitmap bitmap = new Bitmap(SrcImage.Width + mdl, SrcImage.Height + mdl);
            int l, k;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData srcData = SrcImage.LockBits(new Rectangle(0, 0, SrcImage.Width, SrcImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            // compile with: /unsafe
            unsafe
            {
                int offset = 3;

                for (int colm = 0; colm < srcData.Height - size; colm++)
                {
                    byte* ptr = (byte*)srcData.Scan0 + (colm * srcData.Stride);
                    byte* bitmapPtr = (byte*)bitmapData.Scan0 + (colm * bitmapData.Stride);

                    for (int row = 0; row < srcData.Width - size; row++)
                    {
                        nTemp = 0.0;

                        min = double.MaxValue;
                        max = double.MinValue;

                        for (k = 0; k < size; k++)
                        {
                            for (l = 0; l < size; l++)
                            {
                                byte* tempPtr = (byte*)srcData.Scan0 + ((colm + l) * srcData.Stride);
                                c = (tempPtr[((row + k) * offset)] + tempPtr[((row + k) * offset) + 1] + tempPtr[((row + k) * offset) + 2]) / 3;

                                nTemp += (double)c * MASK[k, l];

                            }
                        }

                        sum += nTemp;
                        n++;
                    }
                }
                mean = ((double)sum / n);
                d = 0.0;

                for (int i = 0; i < srcData.Height - size; i++)
                {
                    byte* ptr = (byte*)srcData.Scan0 + (i * srcData.Stride);
                    byte* tptr = (byte*)bitmapData.Scan0 + (i * bitmapData.Stride);

                    for (int j = 0; j < srcData.Width - size; j++)
                    {
                        nTemp = 0.0;

                        min = double.MaxValue;
                        max = double.MinValue;

                        for (k = 0; k < size; k++)
                        {
                            for (l = 0; l < size; l++)
                            {
                                byte* tempPtr = (byte*)srcData.Scan0 + ((i + l) * srcData.Stride);
                                c = (tempPtr[((j + k) * offset)] + tempPtr[((j + k) * offset) + 1] + tempPtr[((j + k) * offset) + 2]) / 3;

                                nTemp += (double)c * MASK[k, l];

                            }
                        }

                        s = (mean - nTemp);
                        d += (s * s);
                    }
                }


                d = d / (n - 1);
                d = Math.Sqrt(d);
                d = d * 2;

                for (int colm = mdl; colm < srcData.Height - mdl; colm++)
                {
                    byte* ptr = (byte*)srcData.Scan0 + (colm * srcData.Stride);
                    byte* bitmapPtr = (byte*)bitmapData.Scan0 + (colm * bitmapData.Stride);

                    for (int row = mdl; row < srcData.Width - mdl; row++)
                    {
                        nTemp = 0.0;

                        min = double.MaxValue;
                        max = double.MinValue;

                        for (k = (mdl * -1); k < mdl; k++)
                        {
                            for (l = (mdl * -1); l < mdl; l++)
                            {
                                byte* tempPtr = (byte*)srcData.Scan0 + ((colm + l) * srcData.Stride);
                                c = (tempPtr[((row + k) * offset)] + tempPtr[((row + k) * offset) + 1] + tempPtr[((row + k) * offset) + 2]) / 3;

                                nTemp += (double)c * MASK[mdl + k, mdl + l];

                            }
                        }

                        if (nTemp > d)
                        {
                            bitmapPtr[row * offset] = bitmapPtr[row * offset + 1] = bitmapPtr[row * offset + 2] = 255;
                        }
                        else
                            bitmapPtr[row * offset] = bitmapPtr[row * offset + 1] = bitmapPtr[row * offset + 2] = 0;

                    }
                }
            }

            bitmap.UnlockBits(bitmapData);
            SrcImage.UnlockBits(srcData);

            return bitmap;
        }

        public void centroid(Bitmap image, ref byte[] rgbvalues, int scan0, int stride, ref int x, ref int y, ref int quad)
        {
            int white_cnt, wc, best_i = 0, best_j = 0, best_cnt = 0;
            Color newColor2;
            int i, j, k, p, r, s;
            double originalColorR, originalColorG, originalColorB;

            for (i = 0; i + 45 < image.Width; i += 5)
                for (j = 0; j + 60 < image.Height; j += 5)
                {
                    white_cnt = wc = 0;
                    for (k = i; k < i + 45; k++)
                        for (p = j; p < j + 60; p++)
                        {
                            //get the pixel from the original image
                            originalColorR = rgbvalues[(p * stride) + (k * 4) + 2];
                            originalColorG = rgbvalues[(p * stride) + (k * 4) + 1];
                            originalColorB = rgbvalues[(p * stride) + (k * 4)];
                            if (originalColorR == 255 && originalColorG == 255 && originalColorB == 255)
                                white_cnt++;
                        }
                    if (white_cnt >= 100)
                    {
                        for (r = i; r < i + 45; r++)
                            for (s = j; s > j - 20 && s > 0; s--)
                            {
                                originalColorR = rgbvalues[(s * stride) + (r * 4) + 2];
                                originalColorG = rgbvalues[(s * stride) + (r * 4) + 1];
                                originalColorB = rgbvalues[(s * stride) + (r * 4)];
                                if (originalColorR == 255 && originalColorG == 255 && originalColorB == 255)
                                    wc++;
                            }

                        if (wc <= 40)
                        {
                            if (best_cnt == 0)
                                best_cnt = white_cnt;
                            if (best_cnt <= white_cnt)
                            {
                                best_i = i;
                                best_j = j;
                                best_cnt = white_cnt;
                            }
                        }
                    }
                }

            newColor2 = Color.FromArgb(255, 0, 0);
            for (k = best_i; k < best_i + 45; k++)
            {
                try
                {
                    rgbvalues[(best_j * stride) + (k * 4) + 2] = newColor2.R;
                    rgbvalues[(best_j * stride) + (k * 4) + 1] = newColor2.G;
                    rgbvalues[(best_j * stride) + (k * 4)] = newColor2.B;

                    rgbvalues[((best_j + 60) * stride) + (k * 4) + 2] = newColor2.R;
                    rgbvalues[((best_j + 60) * stride) + (k * 4) + 1] = newColor2.G;
                    rgbvalues[((best_j + 60) * stride) + (k * 4)] = newColor2.B;
                }
                catch (Exception er)
                { }
            }
            for (p = best_j; p < best_j + 60; p++)
            {
                try
                {
                    /*image.SetPixel(best_i, p, newColor2);
                    image.SetPixel(best_i + 90, p, newColor2);*/
                    rgbvalues[(p * stride) + (best_i * 4) + 2] = 255;
                    rgbvalues[(p * stride) + (best_i * 4) + 1] = 0;
                    rgbvalues[(p * stride) + (best_i * 4)] = 0;

                    rgbvalues[(p * stride) + ((best_i + 45) * 4) + 2] = 255;
                    rgbvalues[(p * stride) + ((best_i + 45) * 4) + 1] = 0;
                    rgbvalues[(p * stride) + ((best_i + 45) * 4)] = 0;
                }
                catch (Exception er)
                {
                    
                }
            }

            x = best_i + 22;
            y = best_j + 30;

            /* To find the quadrant*/
            decimal alpha = Math.Abs((image.Height - y) / (image.Width - x));
            if (x <= image.Width / 2) // quad 1,2,8,7
            {
                if (y <= image.Height / 2) //quad 1 or 2
                {
                    if (alpha < 1)
                        quad = 1;
                    else
                        quad = 2;
                }
                else  //quad 7 or 8
                {
                    if (alpha < 1)
                        quad = 8;
                    else
                        quad = 7;
                }
            }
            else    // quad 3,4,5,6
            {
                if (y <= image.Height / 2) //quad 3 or 4
                {
                    if (alpha < 1)
                        quad = 4;
                    else
                        quad = 3;
                }
                else  //quad 5 or 6
                {
                    if (alpha < 1)
                        quad = 5;
                    else
                        quad = 6;
                }
            }
            //return rgbValues;
        }

        private void Gestura_Resize(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipTitle = "Minimized to Tray";
            notifyIcon1.BalloonTipText = "Double-click the icon to restore Gestuelle.";

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }
        #endregion

        #region Delete The Images
        private void flushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delete_files();
        }

        public void delete_files()
        {
            string[] filePaths = Directory.GetFiles(@"D:\Images\");
            foreach (string filePath in filePaths)
                File.Delete(filePath);

            filePaths = Directory.GetFiles(@"D:\images3\");
            foreach (string filePath in filePaths)
                File.Delete(filePath);

            filePaths = Directory.GetFiles(@"D:\images2\");
            foreach (string filePath in filePaths)
                File.Delete(filePath);

            filePaths = Directory.GetFiles(@"D:\images3\");
            foreach (string filePath in filePaths)
                File.Delete(filePath);

            filePaths = Directory.GetFiles(@"D:\images5\");
            foreach (string filePath in filePaths)
                File.Delete(filePath);
        }
        #endregion
    }
}