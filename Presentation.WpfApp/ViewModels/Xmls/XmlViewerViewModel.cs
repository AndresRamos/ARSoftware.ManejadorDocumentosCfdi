using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace Presentation.WpfApp.ViewModels.Xmls
{
    public sealed class XmlViewerViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private string _contenido;

        public XmlViewerViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Visor XML";
        }

        public string Contenido
        {
            get => _contenido;
            private set
            {
                if (value == _contenido)
                {
                    return;
                }

                _contenido = value;
                NotifyOfPropertyChange(() => Contenido);
            }
        }

        public void Inicializar(string contenido)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(contenido);
            var stringBuilder = new StringBuilder();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
            var xmlWriter = XmlWriter.Create(stringBuilder, settings);
            xmlDocument.Save(xmlWriter);
            xmlWriter.Flush();
            xmlWriter.Close();
            Contenido = stringBuilder.ToString();
        }

        public async Task GuardarArchivoAsync()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "archivo.xml";
                saveFileDialog.Filter = "XML (.xml)|*.xml|TXT (.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, Contenido);
                    Process.Start(saveFileDialog.FileName);
                }
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }
    }
}
