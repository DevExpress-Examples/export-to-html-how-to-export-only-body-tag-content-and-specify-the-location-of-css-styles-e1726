using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
#region #usings
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Export.Html;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Office.Services;
#endregion #usings



namespace ExportOnlyBodyContent {
    public partial class Form1 : XtraForm {
        CssPropertiesExportType cssExportType;
        ExportRootTag htmlExportType;

        public Form1() {
            InitializeComponent();
            InitComboHtmlExportType();
            InitComboCssExportType();

        }
        #region Initializing
        private void InitComboHtmlExportType() {
            ComboBoxItemCollection collExportHtml = edtExportHtmlType.Properties.Items;
            collExportHtml.BeginUpdate();
            collExportHtml.Clear();
            collExportHtml.Add(ExportRootTag.Body);
            collExportHtml.Add(ExportRootTag.Html);
            collExportHtml.EndUpdate();
            edtExportHtmlType.SelectedIndex = 0;
            htmlExportType = ExportRootTag.Body;
        }
        private void InitComboCssExportType() {
            ComboBoxItemCollection collCssStyle = edtCssStyleType.Properties.Items;
            collCssStyle.BeginUpdate();
            collCssStyle.Clear();
            collCssStyle.Add(CssPropertiesExportType.Style);
            collCssStyle.Add(CssPropertiesExportType.Link);
            collCssStyle.Add(CssPropertiesExportType.Inline);
            collCssStyle.EndUpdate();
            edtCssStyleType.SelectedIndex = 0;
            cssExportType = CssPropertiesExportType.Style;
        }
        protected internal void OnLoadDocumentClick(object sender, EventArgs e) {
            this.richEditControl1.LoadDocument();
        }
        private void richEditControl_DocumentLoaded(object sender, EventArgs e) {
            try {
                string fileName = richEditControl1.Options.DocumentSaveOptions.CurrentFileName;
                if(!String.IsNullOrEmpty(fileName)) {
                    using(StreamReader reader = new StreamReader(fileName)) {
                        this.memoEdit1.Text = reader.ReadToEnd();
                    }
                }
            }
            catch {
            }
        }

        private void richEditControl_EmptyDocumentCreated(object sender, EventArgs e) {
            this.memoEdit1.Text = String.Empty;
        }
#endregion Initializing

        #region Adjusting
        private string GetFileName(string filter) {
            using(SaveFileDialog saveFileDialog = new SaveFileDialog()) {
                saveFileDialog.Filter = filter;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.CheckFileExists = false;
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.DereferenceLinks = true;
                saveFileDialog.ValidateNames = true;
                if(saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    return saveFileDialog.FileName;
            }
            return String.Empty;
        }
        private void SaveFile(string fileName, string value) {
            using(FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                using(StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(value);
                }
            }
        }

        private void edtExportHtmlType_SelectedValueChanged(object sender, EventArgs e) {
            this.htmlExportType = (ExportRootTag)edtExportHtmlType.EditValue;

        }
        private void edtCssStyleType_SelectedValueChanged(object sender, EventArgs e) {
            this.cssExportType = (CssPropertiesExportType)edtCssStyleType.EditValue;
        }
        #endregion

        private void OnExportHtmlClick(object sender, EventArgs e) {
            string fileName = GetFileName("HyperText Markup Language Format|*.html");
            if(String.IsNullOrEmpty(fileName))
                return;
            IUriProviderService svc = (IUriProviderService) richEditControl1.GetService(typeof(IUriProviderService));
            svc.RegisterProvider(new MyUriProvider(Path.GetDirectoryName(fileName)));
            string stringHtml = String.Empty;
            ExportHtml(out stringHtml, null, fileName);
            this.memoEdit2.Text = stringHtml;
            SaveFile(fileName, stringHtml);

        }
        #region #exporting
    private void ExportHtml(out string stringHtml, HtmlExporter exporter, string fileName) {
        stringHtml = String.Empty;
        HtmlDocumentExporterOptions options = new HtmlDocumentExporterOptions();
        options.ExportRootTag = htmlExportType;
        options.CssPropertiesExportType = cssExportType;
        options.TargetUri = Path.GetFileNameWithoutExtension(fileName);
        exporter = new HtmlExporter(this.richEditControl1.Model, options);
        stringHtml = exporter.Export();
    }
        #endregion #exporting
    }
}