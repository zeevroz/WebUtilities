using iText.Kernel.Pdf;
using System.IO;

namespace WebUtilities
{
    public static class Library
    {        

        public static bool InsertItems(PdfDocument dst, InsertParams[] items)
        {
            if (items.Length > 0)
            {
                foreach (InsertParams item in items)
                {
                    InsertDocument(dst, item.file);
                }

                return true;
            }

            return false;
        }

        public static void InsertDocument(PdfDocument dst, string file)
        {
            PdfDocument insertDoc = new PdfDocument(new PdfReader(file));

            for (int j = 1; j <= insertDoc.GetNumberOfPages(); j++)
            {
                insertDoc.CopyPagesTo(j, j, dst);
            }

            insertDoc.Close();
        }

        public static string SwapSource(string file)
        {
            if (!System.IO.File.Exists(file))
            {
                return string.Empty;
            }

            string copy = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + "_Copy" + Path.GetExtension(file));

            if (System.IO.File.Exists(copy))
            {
                System.IO.File.Delete(copy);
            }

            System.IO.File.Copy(file, copy);
        
            return copy;
        }

    }
}