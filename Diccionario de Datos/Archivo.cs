﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diccionario_de_Datos
{
    public partial class Archivo : Form
    {
        public string nombre { get; set; }
        public Boolean ACEPTAR { get; set; }
        public Archivo(string nombre)
        {
            ACEPTAR = false;
            this.nombre = nombre;
            InitializeComponent();
        }
        
        public void  creaArchivo(object sender, EventArgs e)
        {
            nombre = textBox1.Text;
            ACEPTAR = true;

            this.Close();
           
        }
        public string dameNombre()
        {
            return nombre;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
