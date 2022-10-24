using iText.Kernel.Pdf;

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

    }
}