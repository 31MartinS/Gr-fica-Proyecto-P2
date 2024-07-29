//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Proyecto2P
//{
//    public class VolumeControl
//    {
//        // Importa la función waveOutSetVolume de la API de Windows
//        [DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

//        // Establece el volumen. El rango de volumen es de 0x0000 a 0xFFFF (0 a 100%)
//        public static void SetVolume(int volume)
//        {
//            uint newVolume = (uint)((volume & 0xFFFF) | (volume << 16));
//            waveOutSetVolume(IntPtr.Zero, newVolume);
//        }
//    }
//}
