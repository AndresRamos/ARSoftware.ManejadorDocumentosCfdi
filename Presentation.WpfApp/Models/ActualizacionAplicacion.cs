using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Azure.Storage.Blob;

namespace Presentation.WpfApp.Models
{
    public class ActualizacionAplicacion : PropertyChangedBase
    {
        private string _mensaje;

        public bool ActualizacionDisponible { get; set; }

        public string Mensaje
        {
            get => _mensaje;
            set
            {
                if (value == _mensaje)
                {
                    return;
                }

                _mensaje = value;
                NotifyOfPropertyChange(() => Mensaje);
            }
        }

        public Version VersionActual { get; set; }

        public Version VersionNueva { get; set; }

        public async Task ChecarActualizacionDisponibleAsync()
        {
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var container = new CloudBlobContainer(new Uri("https://arsoftware.blob.core.windows.net/arsoftwaredownloads/"));

                var currentVersionBlob = container.GetBlockBlobReference(@"ManejadorDocumentosCfdi\CurrentVersion.txt");

                string currentverstion;
                using (var memoryStream = new MemoryStream())
                {
                    await currentVersionBlob.DownloadToStreamAsync(memoryStream);
                    currentverstion = Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                VersionActual = new Version(version);
                VersionNueva = new Version(currentverstion);

                var result = VersionActual.CompareTo(VersionNueva);
                if (result < 0)
                {
                    ActualizacionDisponible = true;
                    Mensaje = $"Hay una actualizacion disponible (version: {VersionNueva}). Desea descargarla?";
                }
                else if (result == 0)
                {
                    Mensaje = $"Ya esta corriendo la ultima version disponible (version: {VersionActual}).";
                }
            }
            catch (Exception e)
            {
                Mensaje = $"No se pudo conectar al servicio de actualizaciones. Error: {e.Message}";
            }
        }

        public async Task DescargarActualizacionAsync(string fileName)
        {
            var container = new CloudBlobContainer(new Uri("https://arsoftware.blob.core.windows.net/arsoftwaredownloads/"));
            var installerFile = container.GetBlockBlobReference(@"ManejadorDocumentosCfdi\Release.zip");
            await installerFile.DownloadToFileAsync(fileName, FileMode.Create);
        }
    }
}