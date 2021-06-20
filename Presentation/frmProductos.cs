using Core.Poco;
using Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentation
{
    public partial class frmProductos : Form
    {
        private ProductRepository productRepository;
        public frmProductos()   
        {
            InitializeComponent();
            txtSearch.AutoSize = false;
            this.MinimumSize = new Size(651, 298);
            productRepository = new ProductRepository();
            setDataSource();
        }

   
        private void frmProductos_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmProductAU frmProductAU = new frmProductAU();
            frmProductAU.ShowDialog();
            frmProductAU.update = false;
            setDataSource();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvProducts.Rows.Count == 0 || dgvProducts.CurrentCell.RowIndex < 0)
            {
                MessageBox.Show("Tabla sin datos o fila no seleccionada");
                return;
            }
            Product p = (Product)dgvProducts.CurrentRow.DataBoundItem;
            if(p == null)
            {
                return;
            }
            frmProductAU frmProductAU = new frmProductAU();
            frmProductAU.fillSpaces(p);
            frmProductAU.update = true;
            frmProductAU.productToUpdate = p;
            frmProductAU.ShowDialog();
            setDataSource();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            if (dgvProducts.Rows.Count == 0 || dgvProducts.CurrentCell.RowIndex < 0)
            {
                MessageBox.Show("Tabla sin datos o fila no seleccionada");
                return;
            }

            Product p = (Product)dgvProducts.CurrentRow.DataBoundItem;
            Console.WriteLine(p.Id);
            productRepository.Delete(p);

            setDataSource();
        }

        private void setDataSource()
        {
            try
            {
                dgvProducts.DataSource = productRepository.GetAll();
            }
            catch (EndOfStreamException i)
            {

            }
        }
    }
}
