using Prism.Mvvm;
using System.IO;
using System.Windows.Media.Imaging;

namespace OpiumNetStat.Model
{
    public class ProcessIPInfo : BindableBase
    {
        public ProcessIPInfo(int _pid)
        {
            PID = _pid;
        }


        private int port;
        private string protocol;
        private string remoteip;
        private int pid;
        private string connectionStatus;
        private string processName;
        private BitmapImage icon;


        public int Port
        {
            get => port;
            set => SetProperty(ref port, value);
        }
        public string Protocol
        {
            get => protocol;
            set => SetProperty(ref protocol, value);
        }

        public string RemoteIp
        {
            get => remoteip;
            set => SetProperty(ref remoteip, value);
        }

        public int PID
        {
            get => pid;
            set
            {
                if (pid != value)
                {
                    SetProperty(ref pid, value);
                    UpdateProcessDetails(pid);
                }

            }
        }


        public string ConnectionStatus
        {
            get => connectionStatus;
            set => SetProperty(ref connectionStatus, value);
        }

        public string ProcessName
        {
            get => processName;
            set => SetProperty(ref processName, value);
        }

        public BitmapImage Icon
        {
            get => icon;
            set => SetProperty(ref icon, value);
        }

        private void UpdateProcessDetails(int _pid)
        {
            try
            {
                var proc = System.Diagnostics.Process.GetProcessById(_pid);
                ProcessName = proc.ProcessName;
                string file;

                if (tryGetIconFileName(proc, out file))
                {
                    Icon = getProcessIcon(file);
                }
            }
            catch 
            {
                //process not found
            }


        }


        private bool tryGetIconFileName(System.Diagnostics.Process proc, out string filename)
        {
            try
            {
                filename = proc.MainModule.FileName;
                return true;
            }
            catch
            {
                filename = string.Empty;
                return false;
            }
        }

        private BitmapImage getProcessIcon(string fileName)
        {
            try
            {
                var bitmap = System.Drawing.Icon.ExtractAssociatedIcon(fileName).ToBitmap();
                var stream = new MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                var bmpImage = new BitmapImage();
                bmpImage.BeginInit();
                stream.Seek(0, SeekOrigin.Begin);
                bmpImage.StreamSource = stream;
                bmpImage.EndInit();

                bmpImage.Freeze();
                return bmpImage;

            }
            catch
            {
                return null;
            }
        }
    }

}
