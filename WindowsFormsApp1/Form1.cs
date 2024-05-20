using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string BASE_URL = @"https://epdf.mylstech.com/api/";
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnConvert_Click(object sender, EventArgs e)
        {
            //1. Declare Variable
            string saveInvoiceName = "Inv-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf",
                    pdfFileName, xmlFilename;
            byte[] pdfBytes = null,xmlBytes= null, responseContentBytes=null;

            HttpResponseMessage response = null;
            MultipartFormDataContent form = new MultipartFormDataContent();

            //2. Convert File to Bytes Array
            pdfFileName =txtPDFFileName.Text;
            xmlFilename = txtXMLFilename.Text;
            if(string.IsNullOrEmpty(pdfFileName)  )
            {
                return;
            }
            using(FileStream stream = File.Open(pdfFileName,FileMode.Open) )
            {
                pdfBytes= new byte[stream.Length];
                await stream.ReadAsync(pdfBytes, 0, (int)stream.Length);
            }
            var pdffileContent = new ByteArrayContent(pdfBytes);
            pdffileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            form.Add(pdffileContent, "pdfFile", Path.GetFileName(pdfFileName));   //3. Construct Mulform Data



            if (!string.IsNullOrEmpty(xmlFilename) && checkBox1.Checked)
            {
                using (FileStream stream = File.Open(xmlFilename, FileMode.Open))
                {
                    xmlBytes = new byte[stream.Length];
                    await stream.ReadAsync(xmlBytes, 0, (int)stream.Length);
                }
                var xmlfileContent = new ByteArrayContent(xmlBytes);
                xmlfileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");
                form.Add(xmlfileContent, "xmlFile", Path.GetFileName(xmlFilename));   //3. Construct Mulform Data
            }
           
             
            
            // here it is important that second parameter matches with name given in API.

            //MultipartFormDataContent form = new MultipartFormDataContent()
            //{
            //    {pdffileContent,"pdfFile",Path.GetFileName(pdfFileName)  }     ,
            //    {xmlfileContent,"xmlFile",Path.GetFileName(pdfFileName) }

            //};

            



            //4. Send To Service
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.ConnectionClose = true;
                httpClient.BaseAddress = new Uri(BASE_URL);// "https://localhost:44314/api/");
                response = await httpClient.PostAsync($"ZatcaService/ConvertPDFtoPDFA3", form);
                response.EnsureSuccessStatusCode();
                responseContentBytes = await response.Content.ReadAsByteArrayAsync();
            };
            if (responseContentBytes == null)
            {
                MessageBox.Show("Error File Creataion");
                return;
            }
            //5. Save the Response

            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf";
                saveFileDialog1.FileName = saveInvoiceName;// "InvoicePDFA3.pdf";
                DialogResult resultdiag = saveFileDialog1.ShowDialog();

                if (resultdiag == DialogResult.OK)
                {
                    File.WriteAllBytes(saveFileDialog1.FileName, responseContentBytes);
                    MessageBox.Show("File Saved Successfuly at " + saveFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog()
            {
                InitialDirectory = @"C:\Users\Linga\Desktop\PDFA\",
                Title = "Browse PDF Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "pdf",
                Filter = "Pdf files (*.pdf)|*.pdf",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPDFFileName.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog()
            {
                InitialDirectory = @"C:\Users\Linga\Desktop\PDFA\",
                Title = "Browse XML Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "xml",
                Filter = "xml files (*.xml)|*.xml",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtXMLFilename.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
