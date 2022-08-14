
namespace PDfReaderIPS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.ComponentModel.Design;
    using System.IO;

    /* using PdfSharp.Pdf;
     using PdfSharp.Drawing;
     using PdfSharp.Pdf.IO;
     */
    using Intermech;
    using Intermech.Interfaces;
    using Intermech.Interfaces.Plugins;
    using Intermech.Interfaces.Client;
    using Intermech.Interfaces.Server;
    using Intermech.Interfaces.Document;
    using Intermech.Kernel.Search;
    using Intermech.Kernel;

    using PdfSharp.Pdf;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf.IO;
    using PdfSharp.Pdf.Actions;
    using PdfSharp.Internal;
    using PdfSharp.Fonts;
    using PdfSharp.Charting;



    public class PluginPackage : AbstractPackage
    {

        public PluginPackage()
         : base(" Интерфейс работы с .pdf")
        {

        }


        public override void Load(IServiceProvider serviceProvider)
        {
           
            ICustomServices customServices = ApplicationServices.Container.GetService(typeof(ICustomServices)) as ICustomServices;
            PdfReads pdfreads = new PdfReads();
            customServices.AddService(typeof(IPDfReaderIPS), pdfreads);
            
            base.Load(serviceProvider);
        }


        public override void Unload()
        {
            base.Unload();
        }

        private class PdfReads : LongLifeObject, IPDfReaderIPS

        {
            string path = @"C:\IPSHub";
            string pathforpdf = String.Empty;
            public void CreateStamp(IDBObject pdfobj, IUserSession session)
            {            

                using (SessionKeeper sk = new SessionKeeper())
                {
                    // IDBObject objFile = session.GetObject(pdfobj);
                    IDBAttribute attrFile = pdfobj.GetAttributeByGuid(new Guid(SystemGUIDs.attributeFile), false) as IDBAttribute;
                    IDBAttribute attrStamp = pdfobj.GetAttributeByName("Инвентарный номер (ОТД)");
                    if (attrFile == null)
                    return;
                     

                        for (int i = 0; i < attrFile.ValuesCount; i++)
                        {
                     

                            DirectoryInfo di = new DirectoryInfo(path);

                            if (!di.Exists)
                                di.Create();
                           
                            attrFile.Index = i;
                            IBlobReader iBlobReader = (attrFile as IBlobReader);
                            if (iBlobReader == null)
                            return;

                            BlobInformation bi = iBlobReader.OpenBlob(-1);
                           
                            string fname = Path.GetFileName(bi.FileName);

                            if (bi.FileName.Trim() != String.Empty)  // файл не пуст
                            {
                                string resFname = Path.Combine(di.FullName, fname);

                                using (FileStream fs = new FileStream(resFname, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                                {
                                    BlobProcReader bpc = new BlobProcReader(attrFile, 0, fs, null, null);
                                    pathforpdf = resFname;//Initalize path for pdf reader
                                    bpc.ReadData();
                                    bpc.SetAbortThreadFlag();
                                   
                                    // MessageBox.Show("Готово");
                                
                                }
                                if (attrStamp != null)
                                {
                                    CreatePdfStamp(pathforpdf, attrStamp);
                                    SetAttribute(pdfobj, session, pathforpdf);
                                    File.Delete(pathforpdf);
                                }
                                else
                                {
                                    File.Delete(pathforpdf);
                                }
                            }
                        }
                }
            }

            private void CreatePdfStamp(String path,IDBAttribute attrStamp)
            {
                PdfDocument docs = PdfReader.Open(pathforpdf);
                PdfDictionary dict = new PdfDictionary(docs);
                foreach (PdfPage page in docs.Pages)
                {
                    
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Verdana", 7, XFontStyle.BoldItalic);
                    gfx.DrawString(attrStamp.AsString, font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height),
                    XStringFormats.BottomRight);
                }
                docs.Save(pathforpdf);
                docs.Close();
       
            }
            private void SetAttribute(IDBObject objFile,IUserSession session, String path)
            {
              
                IDBFileAttribute attrFile = objFile.GetAttributeByGuid(new Guid(SystemGUIDs.attributeFile), false) as IDBFileAttribute;

                attrFile.AddValue(FileTypes.ftAuthentical);
                IBlobWriter writer = attrFile as IBlobWriter;
             
                if (writer == null) return;
                
                byte[] fileData = File.ReadAllBytes(path);
                FileInfo fInfo = new FileInfo(path);


                BlobInformation BlbInfo = new BlobInformation(fInfo.Length,
                fInfo.Length,
                fInfo.LastAccessTime,
                "Штамп "+fInfo.Name,
                ArcMethods.NotPacked,
                "",
                FileTypes.ftAuthentical,
                session.ActingUserID);
                
             
                
                
                if (writer.OpenBlob(BlbInfo, false))
                {
                 
                    writer.WriteDataBlock(fileData);
                }
              

            }

            private void RegisterMyService()
            {
                ICustomServices customServices = ApplicationServices.Container.GetService(typeof(ICustomServices)) as ICustomServices;
                PdfReads pdfreads = new PdfReads();
                customServices.AddService(typeof(IPDfReaderIPS), pdfreads);
            }


        }
      


    }

    public interface IPDfReaderIPS
    {
        void CreateStamp(IDBObject pdfobj,IUserSession session);
    }


}
