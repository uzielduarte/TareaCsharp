using Core.Poco;
using Infraestructure.Data;
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
        private ProductRepository productRepository;
        public bool update;
        public Product productToUpdate { get; set; }
        public frmProductAU()
        {
            InitializeComponent();
            productRepository = new ProductRepository();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string description = txtDescription.Text;
            string brand = txtBrand.Text;
            string model = txtModel.Text;
            decimal price = decimal.Parse(txtPrice.Text);
            int stock = int.Parse(txtStock.Text);
            string image = txtImage.Text;

            if (update == true)
            {
                productToUpdate.Name = name;
                productToUpdate.Description = description;
                productToUpdate.Brand = brand;
                productToUpdate.Model = model;
                productToUpdate.Price = price;
                productToUpdate.Stock = stock;
                productToUpdate.ImageURL = image;
                productRepository.Update(productToUpdate);
                this.Dispose();
                return;
            }

            Product p = new Product
            {
                Name = name,
                Description = description,
                Brand = brand,
                Model = model,
                Price = price,
                Stock = stock,
                ImageURL = image,
            };

            productRepository.Create(p);

            this.Dispose();
        }

        public void fillSpaces(Product p)
        {
            txtName.Text = p.Name;
            txtDescription.Text = p.Description;
            txtBrand.Text = p.Brand;
            txtModel.Text = p.Model;
            txtPrice.Text = p.Price.ToString();
            txtStock.Text = p.Stock.ToString();
            txtImage.Text = p.ImageURL;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
