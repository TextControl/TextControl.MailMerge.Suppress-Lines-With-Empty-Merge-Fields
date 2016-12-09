using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TXTextControl;

namespace tx_emptylines
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // dummy data source class 
        public List<Report> reports = new List<Report>();

        public class Report
        {
            public string company { get; set; }
            public string firstname { get; set; }
            public string name { get; set; }
            public string street { get; set; }
            public string country { get; set; }
        }

        private void mergeNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create a new dummy data object
            Report report = new Report();
            report.company = "Text Control LLC";
            report.street = "123 Shannon-Willow Rd";
            report.country = "United States";

            reports.Add(report);

            // keep empty fields so they can be processed later on
            mailMerge1.RemoveEmptyFields = false;

            // merge the template with data
            serverTextControl1.Create();
            mailMerge1.MergeObjects(reports);

            // remove empty lines
            RemoveEmptyFieldLines();

            // visualize the resulting document
            byte[] data;
            serverTextControl1.Save(out data, BinaryStreamType.InternalUnicodeFormat);
            textControl1.Load(data, BinaryStreamType.InternalUnicodeFormat);
        }

        private void RemoveEmptyFieldLines()
        {
            foreach (IFormattedText obj in serverTextControl1.TextParts)
            {
                foreach (ApplicationField field in obj.ApplicationFields)
                {
                    // check, if character next to empty field is a carriage return
                    if (obj.TextChars[field.Start + field.Length].Char == '\n')
                    {
                        bool bCompleteLine =
                            (field.Start == obj.Lines.GetItem(field.Start).Start)
                            ? true : false;

                        // if yes, remove the field
                        obj.Selection.Start = field.Start;
                        obj.ApplicationFields.Remove(field, false);

                        // if the field is the only text in the line
                        // remove the CR
                        if (bCompleteLine)
                        {
                            obj.Selection.Length = 1;
                            obj.Selection.Text = "";
                        }

                        // call RemoveEmptyFieldLines recursively
                        RemoveEmptyFieldLines();
                    }
                    else
                        field.Text = "";
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TXTextControl.LoadSettings ls = new LoadSettings();
            ls.ApplicationFieldFormat = ApplicationFieldFormat.MSWord;
            textControl1.Load(mailMerge1.TemplateFile, StreamType.WordprocessingML, ls);
        }
    }
}
