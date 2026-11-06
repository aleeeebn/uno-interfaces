using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class FormConexion : Form
    {
        private const int Puerto = 5000;

        private readonly TextBox txtIp = new TextBox
        {
            Text = "127.0.0.1",
            Width = 150
        };

        private readonly TextBox txtMensaje = new TextBox
        {
            Width = 260
        };

        private readonly Button btnServidor = new Button
        {
            Text = "Crear servidor",
            AutoSize = true
        };

        private readonly Button btnConectar = new Button
        {
            Text = "Conectar",
            AutoSize = true
        };

        private readonly Button btnEnviar = new Button
        {
            Text = "Enviar",
            Enabled = false
        };

        private readonly ListBox registro = new ListBox
        {
            Dock = DockStyle.Fill
        };

        private ServidorTcp servidor;
        private ClienteTcp cliente;

        public FormConexion()
        {
            Text = "Prueba cliente-servidor UNO";
            ClientSize = new Size(650, 400);
            var superior = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 45
            };

            superior.Controls.AddRange(new Control[]
            {
                btnServidor, txtIp, btnConectar
            });

            var inferior = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 45
            };

            inferior.Controls.AddRange(new Control[]
            {
                txtMensaje, btnEnviar
            });

            Controls.Add(registro);
            Controls.Add(superior);
            Controls.Add(inferior);

            btnServidor.Click += CrearServidor;
            btnConectar.Click += Conectar;
            btnEnviar.Click += Enviar;

            FormClosed += (s, e) =>
            {
                cliente?.Dispose();
                servidor?.Dispose();
            };
        }

        private void EnUi(Action accion)
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
                return;

            try
            {
                BeginInvoke(new Action(() =>
                {
                    if (!IsDisposed && !Disposing)
                        accion();
                }));
            }
            catch (InvalidOperationException)
            {

            }
        }

        private void Anotar(string texto)
        {
            EnUi(() =>
            {
                registro.Items.Add(texto);
                registro.TopIndex = registro.Items.Count - 1;
            });
        }

        private async void CrearServidor(object sender, EventArgs e)
        {
            btnServidor.Enabled = false;
            servidor = new ServidorTcp(Puerto);

            servidor.ClienteConectado +=
                id => Anotar("Se conectó el cliente " + id);

            servidor.ClienteDesconectado +=
                id => Anotar("Se desconectó el cliente " + id);

            servidor.Error += error => Anotar("Error: " + error);

            servidor.MensajeRecibido += (id, mensaje) =>
            {
                EnUi(async () =>
                {
                    try
                    {
                        await servidor.EnviarATodosAsync(
                            "Cliente " + id + ": " + mensaje);
                    }
                    catch (Exception ex)
                    {
                        Anotar("Error de envío: " + ex.Message);
                    }
                });
            };

            try
            {
                var ejecucion = servidor.IniciarAsync();
                Anotar("Servidor escuchando en el puerto " + Puerto);
                await ejecucion;
            }
            catch (Exception ex)
            {
                Anotar("No se pudo mantener el servidor: " + ex.Message);
            }
            finally
            {
                servidor.Dispose();
                EnUi(() => btnServidor.Enabled = true);
            }
        }

        private async void Conectar(object sender, EventArgs e)
        {
            btnConectar.Enabled = false;

            cliente = new ClienteTcp();
            cliente.MensajeRecibido += Anotar;
            cliente.Error += error => Anotar("Error: " + error);

            cliente.Desconectado += () =>
            {
                Anotar("Conexión cerrada.");

                EnUi(() =>
                {
                    btnEnviar.Enabled = false;
                    btnConectar.Enabled = true;
                });
            };

            try
            {
                await cliente.ConectarAsync(txtIp.Text.Trim(), Puerto);
                btnEnviar.Enabled = true;
                Anotar("Conectado al servidor.");
            }
            catch (Exception ex)
            {
                cliente.Dispose();
                btnConectar.Enabled = true;
                Anotar("No se pudo conectar: " + ex.Message);
            }
        }

        private async void Enviar(object sender, EventArgs e)
        {
            string mensaje = txtMensaje.Text.Trim();

            if (mensaje.Length == 0)
                return;

            try
            {
                await cliente.EnviarAsync(mensaje);
                txtMensaje.Clear();
            }
            catch (Exception ex)
            {
                Anotar("No se pudo enviar: " + ex.Message);
            }
        }
    }
}