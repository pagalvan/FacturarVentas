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
    public partial class ConsultaFiltradaProductos : Form
    {
        private IService<Producto> serviceProducto;
        private IProductoService productoService;
        private Producto productoSeleccionado;
        public ConsultaFiltradaProductos()
        {
            ProductoService productoSvc = new ProductoService();
            serviceProducto = productoSvc;
            productoService = productoSvc;

            InitializeComponent();
            InicializarControles();
        }

        private void InicializarControles()
        {
            // Inicializar ComboBox de Filtro por Estado
            cmbFiltroEstado.Items.Add("Todos");
            cmbFiltroEstado.Items.Add("activo");
            cmbFiltroEstado.Items.Add("inactivo");
            cmbFiltroEstado.SelectedIndex = 0;

            // Configurar DataGridView
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.Columns.Add("Id", "ID");
            dgvProductos.Columns.Add("Referencia", "Referencia");
            dgvProductos.Columns.Add("Nombre", "Nombre");
            dgvProductos.Columns.Add("Existencias", "Existencias");
            dgvProductos.Columns.Add("StockMinimo", "Stock Mínimo");
            dgvProductos.Columns.Add("PrecioUnitario", "Precio Unitario");
            dgvProductos.Columns.Add("Estado", "Estado");

            dgvProductos.Columns["Id"].DataPropertyName = "Id";
            dgvProductos.Columns["Referencia"].DataPropertyName = "Referencia";
            dgvProductos.Columns["Nombre"].DataPropertyName = "Nombre";
            dgvProductos.Columns["Existencias"].DataPropertyName = "Existencias";
            dgvProductos.Columns["StockMinimo"].DataPropertyName = "StockMinimo";
            dgvProductos.Columns["PrecioUnitario"].DataPropertyName = "PrecioUnitario";
            dgvProductos.Columns["Estado"].DataPropertyName = "Estado";

            // Cargar productos
            CargarProductos();
        }

        private void CargarProductos()
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = serviceProducto.Consultar();
        }

        private void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            cmbFiltroEstado.SelectedIndex = 0;
            txtFiltroNombre.Text = string.Empty;
            CargarProductos();
        }


        private void LimpiarCampos()
        {
            txtNombre.Text = string.Empty;
            txtExistencias.Text = string.Empty;
            txtStockMinimo.Text = string.Empty;
            txtPrecioUnitario.Text = string.Empty;
            cmbEstado.SelectedIndex = 0;
            productoSeleccionado = null;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (productoSeleccionado == null)
                {
                    MessageBox.Show("Debe seleccionar un producto para eliminar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult respuesta = MessageBox.Show($"¿Está seguro de eliminar el producto {productoSeleccionado.Nombre}?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (respuesta == DialogResult.Yes)
                {
                    string resultado = serviceProducto.Eliminar(productoSeleccionado.Id);
                    MessageBox.Show(resultado, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (resultado.StartsWith("Eliminado"))
                    {
                        LimpiarCampos();
                        CargarProductos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dgvProductos.Rows[e.RowIndex].Cells["Id"].Value);
                productoSeleccionado = serviceProducto.BuscarId(id);

                if (productoSeleccionado != null)
                {
                    txtNombre.Text = productoSeleccionado.Nombre;
                    txtExistencias.Text = productoSeleccionado.Existencias.ToString();
                    txtStockMinimo.Text = productoSeleccionado.StockMinimo.ToString();
                    txtPrecioUnitario.Text = productoSeleccionado.PrecioUnitario.ToString();
                    cmbEstado.SelectedItem = productoSeleccionado.Estado;
                }
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            try
            {
                string estado = cmbFiltroEstado.SelectedItem.ToString();
                string nombre = txtFiltroNombre.Text;

                if (estado == "Todos" && string.IsNullOrEmpty(nombre))
                {
                    CargarProductos();
                    return;
                }

                if (estado != "Todos" && !string.IsNullOrEmpty(nombre))
                {
                    // Filtrar por estado y nombre
                    var productosFiltrados = productoService.ConsultarPorEstado(estado)
                        .Where(p => p.Nombre.ToLower().Contains(nombre.ToLower()))
                        .ToList();
                    dgvProductos.DataSource = productosFiltrados;
                }
                else if (estado != "Todos")
                {
                    // Filtrar solo por estado
                    dgvProductos.DataSource = productoService.ConsultarPorEstado(estado);
                }
                else if (!string.IsNullOrEmpty(nombre))
                {
                    // Filtrar solo por nombre
                    dgvProductos.DataSource = productoService.ConsultarPorNombre(nombre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al filtrar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (productoSeleccionado == null)
                {
                    MessageBox.Show("Debe seleccionar un producto para modificar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                productoSeleccionado.Nombre = txtNombre.Text;
                productoSeleccionado.Existencias = int.Parse(txtExistencias.Text);
                productoSeleccionado.StockMinimo = int.Parse(txtStockMinimo.Text);
                productoSeleccionado.PrecioUnitario = decimal.Parse(txtPrecioUnitario.Text);
                productoSeleccionado.Estado = cmbEstado.SelectedItem.ToString();

                string resultado = serviceProducto.Modificar(productoSeleccionado);
                MessageBox.Show(resultado, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (resultado.StartsWith("Modificado"))
                {
                    LimpiarCampos();
                    CargarProductos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
