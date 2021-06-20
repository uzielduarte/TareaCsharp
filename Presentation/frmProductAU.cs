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
    public partial class frmProductAU : Form
    {
        public frmProductAU()
        {
            InitializeComponent();
        }

        private void frmProductAU_Load(object sender, EventArgs e)
        {

        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            string rutaImagen = string.Empty;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Elige la ruta de la imagen";
            ofd.Filter = "Archivos de Imagen (*.jpg)(*.jpeg)|*.jpg;*.jpeg|PNG(*.png)|*.png|GIF(*.gif)|*.gif";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                rutaImagen = ofd.FileName;
            }

            txtImage.Text = rutaImagen;
        }
    }
}
