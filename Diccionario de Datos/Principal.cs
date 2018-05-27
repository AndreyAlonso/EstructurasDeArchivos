﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Linq;
/****************************************************************************************************************
* Proyecto:	    Diccionario de Datos                                                                            *
* Autor:		Héctor Andrey Hernández Alonso	                                                                *
* Creación:	    27/Febrero/2018                                                                                 *
* Clases:       Principal, Entidad, Atributo.                                                                   *
*****************************************************************************************************************/

namespace Diccionario_de_Datos
{
    public partial class Principal : Form
    {

        private int Col;
        private int Ren;


    	//Declaraciones de variables para mover la pantalla
        private int posx;
        private int posy;

        // Declaracion de variables para el uso de la Entidad
        private Entidad enti; 
        private string _nombre;
        private char[] aux = new char[20];
        private List<Entidad> entidad; // Lista de Entidades
        private long Cab, dir;
        private List<int> letras; // Lista de nombre de entidad


        //Variables para la clase Atributo
        private Atributo atri;
        private List<Atributo> atributo;
        private string nombreAtri;
        private int longi;

        private bool band, band2;
        private bool nuevo, abierto;

        // Variables para uso del archivo
        BinaryWriter bw;
        BinaryReader br;

        // Variable que dice el tamaño del archivo
        BinaryWriter aBw;
        BinaryReader aBr;


        DataGridView grid;
        List<DataGridView> tablas = new List<DataGridView>();
        List<BinaryWriter> registro;
        List<BinaryWriter> indice;

        private int renglon = 0;
        private int contador;
        //Constructor de la clase principal

        // variable para detectar los tipos de indices
        private int tipoIndice;
        List<string> reg = new List<string>();
        List<string> registros = new List<string>(); 
        List<indices> listaIndices = new List<indices>();
        // Lista de claves para enviar a los indices
        List<indicep> claves;
        string nombre_indice;
        public struct indices
        {
            public int val;
            public int tipo;
        }

        // VARIABLES PARA ENLAZAR ATRIBUTOS CON SU ENTIDAD
        bool nuevoAtributo;
        bool botonAtributo;
        string sEntidad;

        // Estructura de tipo indice primario
        public struct indicep
        {
            public int clave;
            public long dir;
        }
        private indicep temp;
        public Principal()
        {
            InitializeComponent();
            posx = 0;
            posy = 0;
           // enti = new Entidad();
            entidad = new List<Entidad>();
            letras = new List<int>();
            Cab = -1;
            atri = new Atributo();
            atributo = new List<Atributo>();
            band = false;
            band2 = false;
            // bw.Write(Cab);
            nuevo = false;
            abierto = false;
            registro = new List<BinaryWriter>();
            longi = 0;
            contador = 0;
            nuevoAtributo = false;
            botonAtributo = false;
            indice = new List<BinaryWriter>();
            tipoIndice = 0;


        } 

        #region Configuracion Formulario
        //Metodo que permite mover la ventana con el mouse dando clic
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
        	if(e.Button != MouseButtons.Left){
        		posx = e.X;
        		posy = e.Y;
        	}
        	else{
        		Left = Left + (e.X - posx);
        		Top  = Top  + (e.Y - posy);
        	}
        }
        //Opcion cerrar
        private void Cierrate(object sender, MouseEventArgs e)
        {
            this.Close();
        }
        //Opcion minimizar
        private void minimizate(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        private void creaEntidad(object sender, EventArgs e)
        {
            int[] nombres = new int[30];
            enti = new Entidad();
            List<string> nombresOrdenados = new List<string>();

            int cont;
            _nombre = textBox1.Text;
            comboBox1.Items.Add(_nombre); //Agrega las entidades al comboBox de atributo
            
            cont = _nombre.Length;
            for(;cont < 29; cont++)
            {
                _nombre += " ";
            }
            cEntidadRegistro.Items.Add(_nombre);
            comboBox6.Items.Add(_nombre);
            if (entidad.Count == 0)
            {
                enti.nombrate(_nombre);
                enti.direccionate(bw.BaseStream.Length);
                enti.ponteDireccionAtributo(-1);
                enti.ponteDireccionRegistro(-1);
                enti.ponteDireccionSig(-1);

                bw.Seek(0, SeekOrigin.Begin);
                bw.Write(enti.dameDE());
                bw.Write(enti.dameNombre());
                bw.Write(enti.dameDE());
                bw.Write(enti.dameDA());
                bw.Write(enti.dameDD());
                bw.Write(enti.dameDSIG());


            }
            else
            {
                enti.nombrate(_nombre);
                nombresOrdenados.Add(enti.dameNombre());
                
                enti.direccionate(bw.BaseStream.Length);
                enti.ponteDireccionAtributo(-1);
                enti.ponteDireccionRegistro(-1);
                enti.ponteDireccionSig(-2);
                

                
                entidad[entidad.Count - 1].ponteDireccionSig(enti.dameDE());
               // entidad  = ordenaEntidad(entidad, nombresOrdenados);

                bw.Seek((int)bw.BaseStream.Length-8, SeekOrigin.Begin);

                bw.Write(enti.dameDE());    
                bw.Write(enti.dameNombre());
                bw.Write(enti.dameDE());
                bw.Write(enti.dameDA());
                bw.Write(enti.dameDD());
                bw.Write(enti.dameDSIG());
            }
            entidad.Add(enti);
            
            imprimeLista(entidad);
            
        }

        // Algoritmo de ordenación de Entidad
        private List<Entidad> ordenaEntidad(List<Entidad> enti,List<string> nombres)
        {
            nombres.Sort();
            int i = 0;
            foreach(Entidad e in enti)
            {
                if (e.dameNombre() == nombres[i])
                {
                    foreach(Entidad e2 in enti)
                    {
                        if(e2.dameNombre() == nombres[i + 1] && i <= nombres.Count)
                        {
                            e.ponteDireccionSig(e2.dameDE());
                        }
                    }
                }

            }
            return enti;
        }

        
        // Metodo que muestra la lista en el datagrid
        private void imprimeLista(List<Entidad> enti)
        {
            // Limpia el datagrid para una nueva inserción
            dataGridView1.Rows.Clear(); 
            //Ciclo que inserta las entidades en el datagrid
            foreach(Entidad i in enti){
                if(i.dameNombre() == "ELIMINADO")
                {
                    continue;
                }
                else if(i.dameNombre() == "ELIMINADO                    ")
                {
                    continue;
                }
                else
                {
                    dataGridView1.Rows.Add(i.dameNombre(), i.dameDE(), i.dameDA(), i.dameDD(), i.dameDSIG());
                }
                
            }
        }
        

        
        // Metodos para los atributos ******************************************************************
        private void nombraAtributo(object sender, EventArgs e)
        {
            nombreAtri = textBox2.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        /*********************************************************************************
         *      Boton Crear Atributo
         *      Cuando se añade un atributo al archivo, se agrega de igual forma
         *      a un datagrid que permitirá agregar los registros.
         *      
         *********************************************************************************/
         private void creaAtributo(object sender, EventArgs e)
         {
            
            atri = new Atributo();              // Objeto de la clase atributo
            int cont;                           // Variable contador      
            _nombre = textBox2.Text;            // Se asigna a la variable nombre, el nombre del atributo dada por el textBox2
            
            /**********************************************
             * ciclo for que sucede mientras el nombre
             * del atributo tenga una longitud menor a 30
             **********************************************/
             cont = _nombre.Length;
             for(; cont < 29; cont++)
             {
                _nombre += " ";
            }
           // comboBox5.Items.Add(_nombre);
            /**********************************************************************************
             * Si no hay ningun atributo, entonces se agrega con valores principales en -1
             * SI NO entonces, se añade al final y se acomodan los apuntadores
             **********************************************************************************/
             if(atributo.Count == 0 )
             {
                atri.nombrate(_nombre);
                atri.ponteTipo(Convert.ToChar(comboTipo.SelectedItem));
                atri.ponteLongitud(Convert.ToInt32(textBox3.Text));
                atri.direccionate(bw.BaseStream.Length);
                atri.ponteTipoIndice(tipoIndice);
                atri.ponteDirIndice(-1);
                atri.ponteDirSig(-3);
              //  bw.Seek((int)bw.BaseStream.Length, SeekOrigin.Begin);
                bw.Write(atri.dameNombre());
                bw.Write(atri.dameTipo());
                bw.Write(atri.dameLongitud());
                bw.Write(atri.dameDireccion());
                bw.Write(atri.dameTI());
                bw.Write(atri.dameDirIndice());
                bw.Write(atri.dameDirSig());
                
            }
            else
            {
                atri.nombrate(_nombre);
                atri.ponteTipo(Convert.ToChar(comboTipo.SelectedItem));
                atri.ponteLongitud(Convert.ToInt32(textBox3.Text));
                atri.direccionate(bw.BaseStream.Length);
                atri.ponteTipoIndice(tipoIndice);
                atri.ponteDirIndice(-1);
                atri.ponteDirSig(-3);

                atributo[atributo.Count - 1].ponteDirSig(atri.dameDireccion());
                bw.Seek((int)bw.BaseStream.Length-8, SeekOrigin.Begin);
                bw.Write(atributo[atributo.Count-1].dameDirSig());
                bw.Write(atri.dameNombre());
                bw.Write(atri.dameTipo());
                bw.Write(atri.dameLongitud());
                bw.Write(atri.dameDireccion());
                bw.Write(atri.dameTI());
                bw.Write(atri.dameDirIndice());
                bw.Write(atri.dameDirSig());

            }
            atributo.Add(atri);     //Se añanade el atributo a la lista de atributos
            imprimeAtributo(atributo);// Se accede al metodo que muestra en el datagrid la lista de atributos

            /**************************************************************************************
             * Métodos que agregan en el datagrid para la creación de registros, usar los atributos
             **************************************************************************************/
             
          //   dataGridView3.Columns.Add(enti.dameNombre(), atri.dameNombre());
            // dataGridView4.Columns.Add(enti.dameNombre(), atri.dameNombre());

      //       if(atri.dameTI() == 2)
        //     {
          //      claves = new List<indicep>();
            //    nombre_indice = atri.dameNombre();
           // }
           // if(atri.dameTI() == 3)
           // {
             //   nombre_indice = atri.dameNombre();
           // }
            //tabControl1.TabPages[tablas.Count+1].Controls.Add(tablas[tablas.Count-1]);
            //registro[registro.Count - 1].Write(dataGridView3.Rows.ToString());

            /*
            imprimeLista(entidad);
            atri = new Atributo();
            long diraux;
            if (!band2){
                atri.direccionate(enti.dameDE() + 62);
                diraux = atri.dameDireccion();
                band2 = true;
            }
            atri.nombrate(nombreAtri);
            if(band){
                
                atri.ponteDirSig(-1);
                band = false;
            }
            else
            {
                atri.direccionate(atributo[atributo.Count - 1].dameDireccion() + 63);
                atributo[atributo.Count - 1].ponteDirSig(atri.dameDireccion());
            }
            if(atributo.Count > 1)
                atri.direccionate(atributo[atributo.Count - 1].dameDireccion() + 63);
            atri.ponteTipo(char.Parse(comboBox2.Text));
            atri.ponteLongitud(Convert.ToInt32(textBox3.Text));
            atri.ponteTipoIndice(Convert.ToInt32(comboBox3.Text));
            atri.ponteDirIndice(-1);
            atributo.Add(atri);
            imprimeAtributo(atributo);
            */
            botonAtributo = true;
            // Condicional que comprara si se creo un atributo y se  selecciono una entidad nueva
            if(nuevoAtributo == true)
            {
                ponteAtributo(atri);
                nuevoAtributo = false;
            }
        }

        /*******************************************************************
         * Metodo encargado de agregar la direccion del atributo a la entidad 
         *******************************************************************/
         private  void ponteAtributo(Atributo atri)
         {
            if(nuevo)
                bw.Close(); // Se cierra el archivo de modificación para poder usarlo de lectura 
            br = new BinaryReader(File.Open("Entidad.bin", FileMode.Open));  //Objeto de lectura del archivo
            //MessageBox.Show("Cabecera ->" + br.ReadInt64()); // Se lee la cabecera del diccionario de datos
            br.ReadInt64();
            long apuntador = 8; // se inicia desde la posición de la cabecera 
            string n;
            int tam;
            tam = comboBox1.Text.Length;
            string tempNombre;
            tempNombre = comboBox1.Text;
            // Debido a que en el archivo, los nombres de las entidades
            // se guardan con um tamaño de 30, el nombre que se tomo en en el combobox
            // es menor a 30 entonces se completa para que funcione la comparacion 
            for(tam = comboBox1.Text.Length; tam < 29; tam++ ){
                tempNombre += " ";
            }
            int i;
            // Ciclo que recorre las listas de entidades hasta que encuentra el nombre de la entidad seleccionada para su modificación
            for (i = 0; i < entidad.Count; i++){
                if (tempNombre == entidad[i].dameNombre()){                   
                    entidad[i].ponteDireccionAtributo(atri.dameDireccion());
                }
            }
            // Ya se logro dentro de la lista de entidades
            // Para hacerlo desde el archivo se hace lo siguiente
            // se recorre el archivo hasta su valor final en bytes
            while(apuntador < br.BaseStream.Length)
            {
                n = br.ReadString(); // nombre entidad
                br.ReadInt64();//direccion entidad
                br.ReadInt64(); // direccion atributo  
                br.ReadInt64(); // direccion del registro
                br.ReadInt64(); // direccion siguiente

                // Se iguala el apuntador a la posición actual en el archivo
                if (n == tempNombre)
                {
                   // MessageBox.Show("Apuntador del archivo\n" + apuntador);
                    break;
                }
                apuntador = br.BaseStream.Position;   
            }
            // Se obtuvo la posición de la entidad en el archivo para su modificación
            br.Close(); // se cierra el archivo de lectura
            bw = new BinaryWriter(File.Open("Entidad.bin", FileMode.Open));
            bw.Seek((int)apuntador + 38, SeekOrigin.Current);
            bw.Write(atri.dameDireccion());
            //bw.Close();
            imprimeLista(entidad);
        }

        
        /*************************************************************************************************
         *              Seleccion de entidad para el atributo
         *************************************************************************************************/
         

         private void seleccionaEntidad(object sender, EventArgs e)
         {
            
            string nEntidad, nIndice;
            nEntidad = comboBox1.Text;
            nIndice = comboBox1.Text;
            nEntidad += ".dat";
            nIndice += ".idx";
            contador = 0;
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
            dataGridView4.Columns.Clear();
            renglon = 0;
            dataGridView4.Columns.Add("Dirección del Registro", "Dirección del Registro");
            nuevoAtributo = true;

            // Ciclo while que limpia el datagrid de los atributos
            while (dataGridView2.RowCount > 0)
            {
                dataGridView2.Rows.Remove(dataGridView2.CurrentRow);
            }
         //   while (dataGridView3.RowCount > 0)
           // {
            //    dataGridView3.Rows.Remove(dataGridView3.CurrentRow);
            
           // }
        //    while (dataGridView4.RowCount > 0)
        ///    {
           //     dataGridView4.Rows.Remove(dataGridView4.CurrentRow);

           // }
            /******************************************************************
             * Se añade a la lista de registros, el nombre del regristro y se
             * crea el archivo del registro para su utilización.
             ******************************************************************/
             registro.Add(new BinaryWriter(File.Open(nEntidad,FileMode.Create)));
             indice.Add(new BinaryWriter(File.Open(nIndice, FileMode.Create)));
             
            /*
            tabControl1.TabPages.Add(comboBox1.Text);
            tabControl1.TabIndex = entidad.Count;
            grid = new DataGridView();
            grid.Width = 8000;
            tablas.Add(grid);
            */





            /*
            bw.Close();
            long tam;
            br = new BinaryReader(File.Open("Entidad.bin", FileMode.Open));
           

   
            tam = 0;
            cabecera.Text = br.ReadInt64().ToString();
            while (tam < br.BaseStream.Length && br.ReadString() != "ALTO")
            {
                enti = new Entidad();

                enti.nombrate(br.ReadString());
                comboBox1.Items.Add(enti.dameNombre());
                enti.direccionate(br.ReadInt64());
                enti.ponteDireccionAtributo(br.ReadInt64());
                enti.ponteDireccionRegistro(br.ReadInt64());
                enti.ponteDireccionSig(br.ReadInt64());
                entidad.Add(enti);
                imprimeLista(entidad);
                tam = br.BaseStream.Position;
            }

            br.Close();
            bw = new BinaryWriter(File.Open("Entidad.bin", FileMode.Open));
            */
        }
        /***********************************************************************************************
         * Método encargado de mostrar en el datagrid los atributos
         ***********************************************************************************************/
         private void imprimeAtributo(List<Atributo> atri)
         {
            dataGridView2.Rows.Clear();
            foreach (Atributo a in atri){
                dataGridView2.Rows.Add(a.dameNombre(), a.dameTipo(), a.dameLongitud(), a.dameDireccion(), a.dameTI(), a.dameDirIndice(), a.dameDirSig());
            }
        }
        /*****************************************************************************
         * 
         * Metodo que permite abrir el archivo y lo envia a uno nuevo
         * 
         *****************************************************************************/
         private void abreArchivo(object sender, EventArgs e)
         {
            button8.Hide();
            long tam;
            long sig;
            if(nuevo)
            bw.Close();
            br = new BinaryReader(File.Open("Entidad.bin", FileMode.Open));
            button7.Hide();
            string swap;
            
            tam = 0;
            cabecera.Text = br.ReadInt64().ToString();
            while (tam < br.BaseStream.Length)
            {
                MessageBox.Show("apuntador : " + tam);
                enti = new Entidad();
                enti.nombrate(br.ReadString());
                comboBox1.Items.Add(enti.dameNombre());
                enti.direccionate(br.ReadInt64());
                enti.ponteDireccionAtributo(br.ReadInt64());
                enti.ponteDireccionRegistro(br.ReadInt64());
                sig = br.ReadInt64();
                if (sig == -2)
                {
                    
                    enti.ponteDireccionSig(-2);
                    entidad.Add(enti);
                    break;
                }
                else
                {
                    enti.ponteDireccionSig(sig);
                    entidad.Add(enti);
                }
                
                
                tam = br.BaseStream.Position;
               // break;
                // if (sig == -1 && tam < br.BaseStream.Length)
                // {
                //   enti.ponteDireccionSig(sig);
                //    entidad.Add(enti);
                //     imprimeLista(entidad);
                //     abreAtributos(br, tam);
                //    break;

                // }
                //  else
                //  {

                //  }



            }
            imprimeLista(entidad);

            br.Close();
            bw = new BinaryWriter(File.Open("Entidad.bin", FileMode.Open));
            nuevo = true;


        }
        private void abreAtributos(BinaryReader br, long tam)
        {
            //tam = br.BaseStream.Position;
            while (tam < br.BaseStream.Length)
            {
                atri = new Atributo();
                atri.nombrate(br.ReadString());
                atri.ponteTipo(br.ReadChar());
                atri.ponteLongitud(br.ReadInt32());
                atri.direccionate(br.ReadInt64());
                atri.ponteTipoIndice(br.ReadInt32());
                atri.ponteDirIndice(br.ReadInt64());
                atri.ponteDirSig(br.ReadInt64());
                atributo.Add(atri);
                
                tam = br.BaseStream.Position;
            }
            imprimeAtributo(atributo);
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {
            
        }
        /***********************************************************************************
         * 
         *  Metodo que adquiere el renglon del datagrid y lo manda a uno nuevo ademas de 
         *  guardarlo en un archivo
         *  
         *  
         *  contador      ---> variable que cuenta las columnas de una entidad para  los registros
         *  datagridView3 ---> Tabla de inserción de registros
         *  datagridView4 ---> Tabla de índice primario
         * 
         * *********************************************************************************/
         private void insertarRegistro_Click(object sender, EventArgs e)
         {
            string sVal;
            long ap;
            int auxCol;
            reg = new List<string>();
            //Se asigna el apuntador del archivo 
            dataGridView4.Rows.Add();
            if (Ren == 0){
                foreach (Atributo i in atributo){
                    dataGridView4.Columns.Add(i.dameNombre(), i.dameNombre());
                }
                dataGridView4.Columns.Add("Direccion siguiente", "Direccion siguiente");
            }
            for (auxCol = 0; auxCol <= dataGridView3.Columns.Count; auxCol++){
               if(auxCol == 0){
                  dataGridView4.Rows[Ren].Cells[auxCol].Value = "direccion ";
              }
              else{
                reg.Add(dataGridView3.Rows[0].Cells[auxCol-1].Value.ToString());
                dataGridView4.Rows[Ren].Cells[auxCol].Value =  reg[auxCol-1];
              }
            }
            dataGridView4.Rows[Ren].Cells[auxCol].Value = -1;
            Ren++;
        

            //dataGridView4.Rows.Add(reg[0],reg[1]);
            //Se obtiene el primer campo del tagrid de registros

            /*
            // DECLARACIÓN DE VARIABLES PARA MANIPULAR EL DATAGRID 
            string _nombre = "";
            int nombre2 = 0;
            int posicion  = 1;
            long cabRegistro = 0;
            
            int i;
            List<string> nColumna = new List<string>();
            
            dataGridView4.Rows.Add();
            if(renglon == 0)
            {
                dataGridView4.Columns.Add("Dirección Siguiente","Dirección Siguiente");
            }
            for(contador = 0; contador <= dataGridView3.Columns.Count; contador++)
            {
                // Si el contador es cero, se agrega nombre a las columnas
                if(contador == 0)
                {
                    //Se obtiene el tamaño del archivo de registro
                    // Se usa [registro.Count-1] para saber con que atributo estamos trabajando
                    cabRegistro = registro[registro.Count - 1].BaseStream.Length;
                    registro[registro.Count - 1].Write(cabRegistro);
                    dataGridView4.Rows[renglon].Cells[contador].Value = cabRegistro;
                }
                else if( contador >= 1)
                {
                    int tam;
                    if(atributo[contador-1].dameTipo() == 'C')
                    {
                        _nombre = dataGridView3.Rows[0].Cells[contador - 1].Value.ToString();
                        tam = _nombre.Length;
                        while (tam < atributo[contador - 1].dameLongitud() - 1)
                        {
                            _nombre += " ";
                            tam++;
                        }
                        dataGridView4.Rows[renglon].Cells[contador].Value = _nombre;
                        registro[registro.Count - 1].Write(_nombre);

                    }
                    else if(atributo[contador - 1].dameTipo() == 'E')
                    {
                        nombre2 = Convert.ToInt32(dataGridView3.Rows[0].Cells[contador - 1].Value);
                        dataGridView4.Rows[renglon].Cells[contador].Value = nombre2;
                        registro[registro.Count - 1].Write(nombre2);
                    }
                    buscaRegistros();
                    //agregaIndices();
                    MessageBox.Show("Insertando registro");
                }
            }
            registro[registro.Count - 1].Write((long)-1);
            registro[registro.Count - 1].BaseStream.Seek(registro[registro.Count - 1].BaseStream.Length - 8, SeekOrigin.Begin);
            cabRegistro = registro[registro.Count - 1].BaseStream.Length;
            registro[registro.Count - 1].Write(cabRegistro);
            dataGridView4.Rows[renglon].Cells[contador].Value = cabRegistro;
            renglon++;
            */
        }
        public void buscaRegistros()
        {
            List<string> nomb = new List<string>();
            int i = 0;
            
            while(i < registros.Count)
            {
                foreach (Atributo a in atributo)
                {
                    if (registros[i] == a.dameNombre())
                    {
                        indices ax;
                        ax.val = i;
                        ax.tipo = a.dameTI();
                        listaIndices.Add(ax);
                        break;
                    }
                }
                i++;
            }
        }
        public void agregaIndices()
        {
            int tam;
            


            
        }
        public void ordenaIndice(List<indicep> claves)
        {


           // claves = claves.OrderBy(indicep => indicep.clave);
            foreach(indicep n in claves)
            {
                
                indicePrimario.Rows.Add(n.clave, n.dir);
            }
            
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            registro[registro.Count - 1].Close();
        }

        private void Cerrar_Click(object sender, EventArgs e)
        {
            foreach(BinaryWriter arch in indice)
            {
                arch.Close();
            }
        }
        

        /************************************************************************************
         * Selección del tipo de índice
         * Aquí sucede todos los movimientos necesarios para los indices
         * TIPO DE INDICE
         * 0 -- sin tipo
         * 1 -- clave de búsqueda 
         * 2 -- indice primario
         * 3 -- índice secundario
         * 4 -- árbol b+
         * 5 -- multilistas
         ************************************************************************************/
         private void comboIndice_SelectedIndexChanged(object sender, EventArgs e)
         {
            
            switch(comboIndice.SelectedItem.ToString())
            {
                case "Sin tipo":
                tipoIndice = 0;
                
                break;
                case "Clave de búsqueda":
                tipoIndice = 1;   
                break;
                case "Índice primario":
                tipoIndice = 2;
                break;
                case "Índice secundario":
                tipoIndice = 3;
                
                break;
                case "Árbol B+":
                tipoIndice = 4;    
                break;
                case "Multilistas":
                tipoIndice = 5;
                
                break; 
            }
        }
        
        /*******************************************************************************
         * Selecciona el atributo para mostrar el indice
         *******************************************************************************/
         private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
         {
            string nombreAtributo;
            nombreAtributo = comboBox5.Text;
            foreach(Atributo tipo in atributo)
            {
                if(nombreAtributo == tipo.dameNombre())
                {
                    textoIndice.Text = tipo.dameTI().ToString();                
                }
            }
            switch(textoIndice.Text)
            {
                case "0":
                textoIndice.Text = "Sin tipo";
                break;
                case "1":
                textoIndice.Text = "Clave de búsqueda";
                break;
                case "2":
                textoIndice.Text = "Índice Primario";
                break;
                case "3":
                textoIndice.Text = "Índice Secundario";
                break;
                case "4":
                textoIndice.Text = "Árbol B+";
                break;
                case "5":
                textoIndice.Text = "Multilistas";
                break;
            }
        }

        private void cEntidadRegistro_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nombre;
            string n;
            long apuntador;
            long final = 0;
            nombre = cEntidadRegistro.Text;
            
            bw.Close();
            // Se manda llamar a la apertura del archivo
            br = new BinaryReader(File.Open("Entidad.bin", FileMode.Open));
            apuntador = br.ReadInt64();
            while(apuntador < br.BaseStream.Length)
            {
                n = br.ReadString();
                br.ReadInt64();//direccion entidad
                br.ReadInt64(); // direccion atributo  
                br.ReadInt64(); // direccion del registro
                br.ReadInt64(); // direccion siguiente
                if(nombre == n)
                {
                    MessageBox.Show("Nombre Entidad ->" + n);
                    apuntador = br.BaseStream.Position;
                    MessageBox.Show("Apuntador a atributo ->" + apuntador); 
                    while(final != -3)
                    {
                        comboBox5.Items.Add(br.ReadString()); // nombre atributo
                        MessageBox.Show("Nombre -> " +comboBox5.Text);
                        MessageBox.Show("tipo ->" +br.ReadChar());//tipo
                        MessageBox.Show("longitud ->" +br.ReadInt32());// longitud
                        MessageBox.Show("direccion atributo ->" + br.ReadInt64()); // direccion atributo
                        MessageBox.Show("tipo indice ->" + br.ReadInt32()); // tipo de indice
                        MessageBox.Show("direccion indice ->" + br.ReadInt64()); // direccion indice
                        final = br.ReadInt64(); // direccion siguiente
                        MessageBox.Show("final ->" + final);
                       // if (final == -3)
                      //  {
                        //    break;
                       // }
                    }

                    break;
                }
                apuntador = br.BaseStream.Position;
            }
            br.Close();
            bw = new BinaryWriter(File.Open("Entidad.bin", FileMode.Open));
            foreach(Entidad i in entidad)
            {
                if(i.dameDA() == apuntador)
                {

                }
            }

        }
        
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            string nombre;
            string n;
            long apuntador;
            long final = 0;
            nombre = comboBox6.Text;
            string xx;
            Col = 0;
            Ren = 0;
            bw.Close();
            // Se manda llamar a la apertura del archivo
            br = new BinaryReader(File.Open("Entidad.bin", FileMode.Open));
            apuntador = br.ReadInt64();
            while (apuntador < br.BaseStream.Length)
            {
                n = br.ReadString();
                br.ReadInt64();//direccion entidad
                br.ReadInt64(); // direccion atributo  
                br.ReadInt64(); // direccion del registro
                br.ReadInt64(); // direccion siguiente
                if (nombre == n)
                {
                   // MessageBox.Show("Nombre Entidad ->" + n);
                    apuntador = br.BaseStream.Position;
                    //MessageBox.Show("Apuntador a atributo ->" + apuntador);
                    while (final != -3)
                    {
                        // br.ReadString(); // nombre atributo
                        xx = br.ReadString();
                        dataGridView3.Columns.Add(enti.dameNombre(), xx);
                        registros.Add(xx);
                        br.ReadChar();//tipo
                        br.ReadInt32();// longitud
                        br.ReadInt64(); // direccion atributo
                        br.ReadInt32(); // tipo de indice
                        br.ReadInt64(); // direccion indice
                        final = br.ReadInt64(); // direccion siguiente

                    }

                    break;
                }
                apuntador = br.BaseStream.Position;
            }
            br.Close();
            bw = new BinaryWriter(File.Open("Entidad.bin", FileMode.Open));
            foreach (Entidad i in entidad)
            {
                if (i.dameDA() == apuntador)
                {

                }
            }
        }

        
        



        /**************************************************************************************************************************
         *  Se crea un nuevo diccionario de datos 
         **************************************************************************************************************************/
         private void nuevoProyecto(object sender, EventArgs e)
         {
            bw = new BinaryWriter(File.Open("Entidad.bin", FileMode.Create));
            bw.Write(Cab);
            nuevo = true;
            button7.Hide();
            button8.Hide();
        }

        /**************************************************************************************************************************
         * Metodo encargado de eliminar la entidad
         * sus apuntadores los hace null
         * el nombre cambia a eliminado
         * la entidad anterior apunta al siguiente de la entidad eliminada
         *************************************************************************************************************************/
         private void eliminaEntidad(object sender, EventArgs e)
         {
            // Para la eliminación se requiere:
            // 1 variable auxiliar para el apuntador
            // 1 variable auxiliar para el apuntador siguiente
            // 1 variable para obtener el nombre del registro
            
            string _nombre;
            int tam;
            long ap;
            ap = 0;
            int renglon = 0;
            _nombre = textBox1.Text;
            tam = _nombre.Length;

            string n= "";
            long de, da, dd, ds;
            de = da = dd = ds = 0;
            
            while(tam < 29)
            {
                _nombre += " ";
                tam++;
            }

            int i;
            for(i = 0; i < entidad.Count; i++)
            {
                if(_nombre == entidad[i].dameNombre())
                {
                    if( i == 0)
                    {
                        entidad[i].ponteDireccionSig(-1);
                        entidad[i].nombrate("ELIMINADO");
                    }
                    else
                    {
                        entidad[i].ponteDireccionSig(-1);
                        entidad[i].nombrate("ELIMINADO");
                        entidad[i - 1].ponteDireccionSig(entidad[i + 1].dameDE());
                    }
                }
            }
            imprimeLista(entidad);


            
            // Se manda llamar al objeto encargado de la apertura del archivo
            if (nuevo)
            bw.Close();
            br = new BinaryReader(File.Open("Entidad.bin", FileMode.Open));
            cabecera.Text = br.ReadInt64().ToString();
            while (ap < br.BaseStream.Length)
            {
                n  = br.ReadString();   //Nombre
               // MessageBox.Show("Nombre: " + n);
                de = br.ReadInt64();    //Dirección
                da = br.ReadInt64();    //DireccionAtributo
                dd = br.ReadInt64();    //Direccion de Datos
                ds = br.ReadInt64();    //Direccion Siguiente
                renglon++;
                if (_nombre == n)
                {
                    break;
                }

                
                ap = br.BaseStream.Position;
            //    MessageBox.Show("El apuntador del archivo está en: " + ap);
            }
        //    MessageBox.Show("Entidad eliminanda\n" + n + " " +   de + " " + da + " " + dd + " " +  ds);
            MessageBox.Show("El apuntador del archivo está en: " + ap);
            //Cierra el archivo para lectura y abre para la modificación 
            br.Close();
            bw = new BinaryWriter(File.Open("Entidad.bin", FileMode.Open));
            bw.Seek((int)ap-8, SeekOrigin.Begin);
            bw.Write(ds);
            string eliminado;
            eliminado = "ELIMINADO";
            int cont;
            for(cont = eliminado.Length; cont < 29; cont++)
            {
                eliminado += " ";

            }
            bw.Write(eliminado);
            bw.Write(-1);
            bw.Write(-1);
            bw.Write(-1);
            bw.Write(-1);
            
            //dataGridView1.Rows.Remove();
        //    MessageBox.Show("Direccion de la siguiente entidad " + ds);

            bw.Close();
            

            /*
            _nombre = textBox1.Text;
            int z = _nombre.Length;

            for(;z< 29; z++)
            {
                _nombre += " ";
            }

            string nombre2;

            int i;
            for (i = 0; i < entidad.Count; i++ )
            {
                nombre2 = entidad[i].dameNombre();
                if(i == 0 && nombre2 == _nombre)
                {
                    entidad[i].ponteDireccionSig(-1);
                    entidad[i].direccionate(-1);
                    entidad[i].nombrate("Eliminado");
                    dataGridView1.Rows.RemoveAt(i);
                    break;
                }
                if (nombre2 == _nombre)
                {
                   // MessageBox.Show("Encontrado");
                    entidad[i - 1].ponteDireccionSig(entidad[i].dameDSIG());
                    entidad[i].nombrate("Eliminado");
                    entidad[i].direccionate(-1);
                    entidad[i].ponteDireccionSig(-1);
                    dataGridView1.Rows.RemoveAt(i-1);
                    break;

                    
                }
                
            }
            
            imprimeLista(entidad);
            */
        }
        /******************************************************************************************************************************************************************
         *                              M E T O D O    E L I M I N A      A T R I B U T O
         * 
         ******************************************************************************************************************************************************************/

         private void button4_Click(object sender, EventArgs e)
         {
            // Para la eliminación se requiere:
            // 1 variable auxiliar para el apuntador
            // 1 variable auxiliar para el apuntador siguiente
            // 1 variable para obtener el nombre del registro

            string _nombre;
            int tam;
            long ap;
            ap = 0;
            _nombre = textBox2.Text;
            string n = "";
            int tipo;
            int longitud;
            int ti;
            long da, di, ds;
            tam = _nombre.Length;
            while(tam < 29)
            {
                _nombre += " ";
                tam++;
            }
            int i;
            for(i = 0; i < atributo.Count; i++)
            {
                if(_nombre == atributo[i].dameNombre())
                {
                    if(i == 0)
                    {
                        atributo[i].ponteDirSig(-1);
                        atributo[i].nombrate("ELIMINADO");
                    }
                    else
                    {
                        atributo[i].ponteDirSig(-1);
                        atributo[i].nombrate("ELIMINADO");
                        atributo[i - 1].ponteDirSig(atributo[i + 1].dameDireccion());
                    }
                }
            }
            imprimeAtributo(atributo);
            

            /*
            string nn = textBox2.Text;
            int i;
            for (i = 0; i < atributo.Count; i++)
            {
                if (atributo[i].dameNombre() == nn)
                {
                    atributo[i - 1].ponteDirSig(atributo[i].dameDirSig());
                    atributo[i].direccionate(-1);
                    atributo[i].ponteDirSig(-1);
                    break;

                }
            }
            dataGridView2.Rows.RemoveAt(i);
            imprimeAtributo(atributo);
            */
        }   
    }
}
