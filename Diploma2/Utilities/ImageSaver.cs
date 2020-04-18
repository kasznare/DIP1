using System;
using System.Drawing;
using System.Windows.Controls;
using WindowsFormsApp1;
using Image = System.Drawing.Image;

namespace Diploma2.Utilities {
    public class ImageSaver {

        public static string LocalImagePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\DIP1\Screens\";
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
    }


}