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
    public partial class FormPrincipal: Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void gestionProductosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormGestionProductos formProductos = new FormGestionProductos();
            formProductos.ShowDialog();
        }

        private void crearFacturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCrearFactura formFactura = new FormCrearFactura();
            formFactura.ShowDialog();
        }

        private void consultarFacturasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConsultarFacturas formConsulta = new FormConsultarFacturas();
            formConsulta.ShowDialog();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
