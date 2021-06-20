using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentation
{
    public partial class frmProductos : Form
    {
        public frmProductos()
        {
            InitializeComponent();
            txtSearch.AutoSize = false;
            this.MinimumSize = new Size(651, 298);
            
        }

    

        private void frmProductos_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmProductAU frmProductAU = new frmProductAU();
            frmProductAU.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            frmProductAU frmProductAU = new frmProductAU();
            frmProductAU.ShowDialog();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }
    }
}
