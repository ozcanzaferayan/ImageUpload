using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace ImageUpload
{
    class Images
    {
        string ImageFileName = null;
        byte[] imageBytes = null;

        SqlConnection imageConnection = null;
        SqlCommand imageCommand = null;
        SqlDataReader imageReader = null;

        public Images()
        {
            imageConnection = new SqlConnection(@"Server= .\; Integrated Security= true; Database= ImageStorage");
            imageConnection.Open();
            imageReader = imageCommand.ExecuteReader();

        }
        public Bitmap GetImage()
        {
            MemoryStream ms = new MemoryStream(imageBytes);
            Bitmap bmap = new Bitmap(ms);
            return bmap;
        }
        public String GetFileName()
        {
            return ImageFileName;
        }
        public bool GetRow()
        {
            if (imageReader.Read())
            {
                ImageFileName = (string)imageReader.GetValue(0);
                imageBytes = (byte[])imageReader.GetValue(1);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
