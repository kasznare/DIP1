using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsFormsApp1 {
    static class Program {
        /// <summary>
        /// The main entry myPoint for the application.
        /// </summary>
        static Stopwatch st = new Stopwatch();
        [STAThread]
        static void Main() {
            st.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new SimulationForm());
        }
    }
}
