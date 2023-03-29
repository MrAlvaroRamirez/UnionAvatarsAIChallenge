using System;
using System.IO;

namespace UnionAvatars.Utils
{
    public class ImageConverter
    {
        public static string ConvertImageToBase64(string pathToImage)
        { 
            return ConvertImageToBase64(File.ReadAllBytes(pathToImage));   
        }

        public static string ConvertImageToBase64(byte[] img)
        {
            try
            {
                string base64ImageRepresentation = Convert.ToBase64String(img);

                return base64ImageRepresentation;
            }
            catch (System.Exception)
            {
                return "";
            }
            
        }
    }
}