using System.Drawing;
using QRCoder;

namespace Magician.Common.Util
{
    public class QrCoderUtil
    {
        /// <summary>
        ///     生成二维码
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="pixel">像素点大小</param>
        /// <param name="whiteEdge">二维码白边</param>
        /// <param name="version">版本 1 ~ 40</param>
        /// <returns>位图</returns>
        public static Bitmap GetQrCode(string msg, int pixel = 7, bool whiteEdge = true, int version = -1)
        {
            var codeGenerator = new QRCodeGenerator();
            var codeData = codeGenerator.CreateQrCode(msg, QRCodeGenerator.ECCLevel.M, true, true,
                QRCodeGenerator.EciMode.Utf8, version);

            var code = new QRCode(codeData);

            return code.GetGraphic(pixel, Color.Black, Color.White, whiteEdge);
        }
    }
}