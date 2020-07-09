using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entidades;

namespace MainCorreo
{
    public partial class FrmPpal : Form
    {
        /// <summary>
        /// Atributos
        /// </summary>
        Paquete paquete;
        Correo correo;

        public FrmPpal()
        {
            InitializeComponent();
            correo = new Correo();
        }

        private void FrmPpal_FormClosing(object sender, FormClosingEventArgs e)
        {
            correo.FinEntregas();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            paquete = new Paquete(txtDireccion.Text, mtxtTrackingID.Text);
            paquete.InformaEstado += paq_InformaEstado;
            paquete.InfoException+= paquete_InfoException;

            try
            {
                correo += paquete;
            }
            catch (TrackingIdRepetidoException ex)
            {
                MessageBox.Show(ex.Message, "Paquete repetido", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }

        }

        private void paq_InformaEstado(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Paquete.DelegadoEstado d = new Paquete.DelegadoEstado(paq_InformaEstado);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                ActualizarEstados();
            }
        }

        private void paquete_InfoException(object paquete, Exception ex)
        {
            MessageBox.Show("Error en la Conexión");
        }

        private void ActualizarEstados()
        {
            foreach (Paquete item in correo.Paquetes)
            {
                switch (item.Estado)
                {
                    case Paquete.EEstado.Ingresado:
                        if (!lstEstadoIngresado.Items.Contains(item.ToString()))
                            lstEstadoIngresado.Items.Add(item.ToString());
                        break;
                    case Paquete.EEstado.EnViaje:
                        if (!lstEstadoEnViaje.Items.Contains(item.ToString()))
                        {
                            lstEstadoEnViaje.Items.Add(item.ToString());
                            lstEstadoIngresado.Items.Clear();
                        }
                        break;
                    case Paquete.EEstado.Entregado:
                        if (!lstEstadoEntregado.Items.Contains(item))
                        {
                            lstEstadoEntregado.Items.Add(item);
                            lstEstadoEnViaje.Items.Clear();
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Método Mostrar Info
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elemento"></param>
        private void MostrarInformacion<T>(IMostrar<T> elemento)
        {
            if (!(elemento == null))
            {
                string archivo = "salida";
                if (elemento is Paquete)
                {
                    rtbMostrar.Text = ((Paquete)elemento).ToString();
                    //rtbMostrar.Text = elemento.MostrarDatos(elemento);
                }
                else if (elemento is Correo)
                {
                    rtbMostrar.Text = ((Correo)elemento).MostrarDatos((Correo)elemento);
                }
                rtbMostrar.Text.Guardar(archivo);
            }

        }

        private void Mostrar_Opening(object sender, CancelEventArgs e)
        {
            this.MostrarInformacion<List<Paquete>>((IMostrar<List<Paquete>>)correo);
        }


        private void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            this.MostrarInformacion<List<Paquete>>((IMostrar<List<Paquete>>)correo);
        }

        private void mostrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MostrarInformacion<Paquete>((IMostrar<Paquete>)lstEstadoEntregado.SelectedItem);
        }
    }
}
