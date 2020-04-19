using System;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using Image = System.Drawing.Image;

namespace Diploma2.Utilities {
    public class ImageSaver {
        public string runPath = "";
        public static string LocalImagePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DIP2\Screens\";
        public static string ImageName = "Diploma-" + DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss") + ".png";
        public static void SaveControlImage(Control ctr) {
            //try {
            //    ImageName = "Diploma-" + DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss") + ".png";
            //    var imagePath = LocalImagePath + ImageName;

            //    Image bmp = new Bitmap(ctr.Width, ctr.Height);
            //    var gg = Graphics.FromImage(bmp);
            //    var rect = ctr.RectangleToScreen(ctr.ClientRectangle);
            //    gg.CopyFromScreen(rect.Location, System.Drawing.Point.Empty, ctr.Size);

            //    bmp.Save(imagePath);
            //}
            //catch (Exception w) {
            //    Logger.WriteLog("Onpaint exception: " + w);
            //}
        }
        private void CreateRunFolderAndInitPath() {

            runPath = $@"C:\Users\{Environment.UserName}\Documents\DIP2\Screens\{DateTime.Now:yy-MM-dd-hh-ss-tt}\";
            try {
                Directory.CreateDirectory(runPath);
            }
            catch (Exception e) {
                Logger.WriteLog("Directory can not be created, it already exists");
            }
        }
    }


}