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
    public partial class FormCrearFactura : Form
    {
        private IService<Producto> serviceProducto;
        private IService<Factura> serviceFactura;
        private IProductoService productoService;
        private IFacturaService facturaService;
        private List<DetalleFactura> detallesFactura;
        private Producto productoSeleccionado;
        private decimal valorTotal;
        public FormCrearFactura()
        {
            ProductoService productoSvc = new ProductoService();
            FacturaService facturaSvc = new FacturaService();
            serviceProducto = productoSvc;
            serviceFactura = new FacturaService();
            facturaService = facturaSvc;
            detallesFactura = new List<DetalleFactura>();
            productoService = productoSvc;
            InitializeComponent();

            valorTotal = 0;
            InicializarControles();
        }

        private void InicializarControles()
        {
            DateTime fechaUltimaFactura = ObtenerFechaUltimaFactura();
            dtpFechaFactura.MinDate = fechaUltimaFactura;
            dtpFechaFactura.Value = DateTime.Today;

            // Configurar DataGridView
            dgvDetallesFactura.AutoGenerateColumns = false;
            dgvDetallesFactura.Columns.Add("ReferenciaProducto", "Referencia");
            dgvDetallesFactura.Columns.Add("NombreProducto", "Nombre");
            dgvDetallesFactura.Columns.Add("Cantidad", "Cantidad");
            dgvDetallesFactura.Columns.Add("PrecioUnitario", "Precio Unitario");
            dgvDetallesFactura.Columns.Add("ValorItemVendido", "Valor Total");

            dgvDetallesFactura.Columns["ReferenciaProducto"].DataPropertyName = "ReferenciaProducto";
            dgvDetallesFactura.Columns["NombreProducto"].DataPropertyName = "NombreProducto";
            dgvDetallesFactura.Columns["Cantidad"].DataPropertyName = "Cantidad";
            dgvDetallesFactura.Columns["PrecioUnitario"].DataPropertyName = "PrecioUnitario";
            dgvDetallesFactura.Columns["ValorItemVendido"].DataPropertyName = "ValorItemVendido";

            // Inicializar etiquetas
            lblNombreProducto.Text = "";
            lblPrecioUnitario.Text = "";
            lblExistencias.Text = "";
            lblValorTotal.Text = "$ 0.00";

            ActualizarDataGridView();
        }

        private void ActualizarDataGridView()
        {
            dgvDetallesFactura.DataSource = null;
            dgvDetallesFactura.DataSource = detallesFactura;
            CalcularValorTotal();
        }

        private void CalcularValorTotal()
        {
            valorTotal = 0;
            foreach (var detalle in detallesFactura)
            {
                valorTotal += detalle.ValorItemVendido;
            }
            lblValorTotal.Text = $"$ {valorTotal:N2}";
        }

        private DateTime ObtenerFechaUltimaFactura()
        {
            try
            {
                var facturas = serviceFactura.Consultar();
                if (facturas.Count > 0)
                {
                    return facturas.Max(f => f.Fecha);
                }
                return DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private void LimpiarDatosProducto()
        {
            txtReferenciaProducto.Text = string.Empty;
            lblNombreProducto.Text = "";
            lblPrecioUnitario.Text = "";
            lblExistencias.Text = "";
            txtCantidad.Text = string.Empty;
            productoSeleccionado = null;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string referencia = txtReferenciaProducto.Text.Trim();
                if (string.IsNullOrEmpty(referencia))
                {
                    MessageBox.Show("Ingrese una referencia de producto", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                productoSeleccionado =productoService.BuscarPorReferencia(referencia);
                if (productoSeleccionado == null)
                {
                    MessageBox.Show("No se encontró un producto con esa referencia", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarDatosProducto();
                    return;
                }

                lblNombreProducto.Text = productoSeleccionado.Nombre;
                lblPrecioUnitario.Text = $"$ {productoSeleccionado.PrecioUnitario:N2}";
                lblExistencias.Text = productoSeleccionado.Existencias.ToString();
                txtCantidad.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (productoSeleccionado == null)
                {
                    MessageBox.Show("Debe buscar un producto primero", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtCantidad.Text))
                {
                    MessageBox.Show("Ingrese la cantidad a vender", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCantidad.Focus();
                    return;
                }

                if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
                {
                    MessageBox.Show("La cantidad debe ser un número entero positivo", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCantidad.Focus();
                    return;
                }

                string validacion = facturaService.ValidarDisponibilidadProducto(productoSeleccionado.Referencia, cantidad);
                if (validacion != "OK")
                {
                    MessageBox.Show(validacion, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verificar si el producto ya está en la lista
                var detalleExistente = detallesFactura.FirstOrDefault(d => d.ReferenciaProducto == productoSeleccionado.Referencia);
                if (detalleExistente != null)
                {
                    DialogResult respuesta = MessageBox.Show("Este producto ya está en la factura. ¿Desea actualizar la cantidad?", "Producto duplicado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (respuesta == DialogResult.Yes)
                    {
                        // Validar que haya suficiente existencia para la nueva cantidad total
                        int nuevaCantidadTotal = detalleExistente.Cantidad + cantidad;
                        validacion = facturaService.ValidarDisponibilidadProducto(productoSeleccionado.Referencia, nuevaCantidadTotal);
                        if (validacion != "OK")
                        {
                            MessageBox.Show(validacion, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        detalleExistente.Cantidad = nuevaCantidadTotal;
                        detalleExistente.CalcularValorItemVendido();
                    }
                }
                else
                {
                    // Crear nuevo detalle
                    DetalleFactura detalle = new DetalleFactura
                    {
                        ReferenciaProducto = productoSeleccionado.Referencia,
                        NombreProducto = productoSeleccionado.Nombre,
                        Cantidad = cantidad,
                        PrecioUnitario = productoSeleccionado.PrecioUnitario
                    };
                    detalle.CalcularValorItemVendido();
                    detallesFactura.Add(detalle);
                }

                ActualizarDataGridView();
                LimpiarDatosProducto();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarFactura_Click(object sender, EventArgs e)
        {
            try
            {
                if (detallesFactura.Count == 0)
                {
                    MessageBox.Show("Debe agregar al menos un producto a la factura", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Factura factura = new Factura
                {
                    Fecha = dtpFechaFactura.Value,
                    ValorTotal = valorTotal,
                    Detalles = detallesFactura
                };

                string resultado = facturaService.Guardar(factura);
                MessageBox.Show(resultado, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (resultado.Contains("correctamente"))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar factura: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void dgvDetallesFactura_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string referencia = dgvDetallesFactura.Rows[e.RowIndex].Cells["ReferenciaProducto"].Value.ToString();
                var detalle = detallesFactura.FirstOrDefault(d => d.ReferenciaProducto == referencia);

                if (detalle != null)
                {
                    DialogResult respuesta = MessageBox.Show($"¿Desea eliminar el producto {detalle.NombreProducto} de la factura?", "Eliminar producto", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (respuesta == DialogResult.Yes)
                    {
                        detallesFactura.Remove(detalle);
                        ActualizarDataGridView();
                    }
                }
            }
        }

    }
}
