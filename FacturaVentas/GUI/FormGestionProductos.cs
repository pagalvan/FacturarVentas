using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class FormGestionProductos : Form
    {
        private IService<Producto> serviceProducto;
        public FormGestionProductos()
        {
            serviceProducto = new ProductoService();
            InitializeComponent();
            InicializarControles();
        }

        private void InicializarControles()
        {
            cmbEstado.SelectedIndex = 0;
        }



        private void Consultar_Click(object sender, EventArgs e)
        {
            ConsultaFiltradaProductos formConsultaP = new ConsultaFiltradaProductos();
            formConsultaP.ShowDialog();
        }


        private void LimpiarCampos()
        {
            txtReferencia.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtExistencias.Text = string.Empty;
            txtStockMinimo.Text = string.Empty;
            txtPrecioUnitario.Text = string.Empty;
            //cmbEstado.SelectedIndex = 0;
            txtReferencia.Enabled = true;
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(txtReferencia.Text))
            {
                MessageBox.Show("La referencia no puede estar vacía", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtReferencia.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                MessageBox.Show("El nombre no puede estar vacío", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtExistencias.Text))
            {
                MessageBox.Show("Las existencias no pueden estar vacías", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtExistencias.Focus();
                return false;
            }

            if (!int.TryParse(txtExistencias.Text, out int existencias) || existencias < 0)
            {
                MessageBox.Show("Las existencias deben ser un número entero positivo", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtExistencias.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtStockMinimo.Text))
            {
                MessageBox.Show("El stock mínimo no puede estar vacío", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtStockMinimo.Focus();
                return false;
            }

            if (!int.TryParse(txtStockMinimo.Text, out int stockMinimo) || stockMinimo < 0)
            {
                MessageBox.Show("El stock mínimo debe ser un número entero positivo", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtStockMinimo.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtPrecioUnitario.Text))
            {
                MessageBox.Show("El precio unitario no puede estar vacío", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPrecioUnitario.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrecioUnitario.Text, out decimal precioUnitario) || precioUnitario <= 0)
            {
                MessageBox.Show("El precio unitario debe ser un número decimal positivo", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPrecioUnitario.Focus();
                return false;
            }

            return true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos())
                    return;

                Producto producto = new Producto
                {
                    Referencia = txtReferencia.Text,
                    Nombre = txtNombre.Text,
                    Existencias = int.Parse(txtExistencias.Text),
                    StockMinimo = int.Parse(txtStockMinimo.Text),
                    PrecioUnitario = decimal.Parse(txtPrecioUnitario.Text),
                    Estado = cmbEstado.SelectedItem.ToString()
                };

                string resultado = serviceProducto.Guardar(producto);
                MessageBox.Show(resultado, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (resultado.StartsWith("Guardado"))
                {
                    LimpiarCampos();
                    //CargarProductos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
