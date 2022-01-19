using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace Correos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            //mostrar la lista
            //teniendo el total de iteracciones podemos meter las dos listas en un ciclo for para ir evaluando si el correo existe o no
            //para ello se tiene que evaluar si el EMAIL existe en la base de datos CORREO tabla CONTACTO
            //Si existe no hacer nada y si no existe proceder a guardar el NOMBRE y EMAIL en la tabla contacto
            //para ello se necesita saber el tamaño de la lista para saber el numero de iteraciones que debera realizar
            for (int c = 0; c < ListaCorreos.Count; c++)
            {
                using (SqlConnection cnn = new SqlConnection(sql))
                {
                    cnn.Open();
                    String consulta = "SELECT EMAIL FROM CONTACTOS WHERE EMAIL='"+ListaCorreos[c]+"'";
                    SqlCommand cmd = new SqlCommand(consulta, cnn);

                    SqlDataReader readers = cmd.ExecuteReader();
                    if (readers.Read())
                    {
                        //Si ya esta registrado el correo no hacer nada 
                    }
                    else
                    {
                        //insertar el registro en la base de datos 
                        using (SqlConnection cnnI = new SqlConnection(sql))
                        {
                            cnnI.Open();
                            String insertar = "INSERT INTO CONTACTOS (NOMBRE, EMAIL, FECHA, fkORIGENDOMINIO) VALUES (@NOMBRE, @EMAIL, @FECHA, @DOMINIO)";
                            SqlCommand cmdI = new SqlCommand(insertar, cnnI);
                            cmdI.Parameters.Add(new SqlParameter("@NOMBRE", SqlDbType.VarChar)).Value = ListaNombres[c];
                            cmdI.Parameters.Add(new SqlParameter("@EMAIL", SqlDbType.VarChar)).Value = ListaCorreos[c];
                            cmdI.Parameters.Add(new SqlParameter("@FECHA", SqlDbType.Date)).Value = System.DateTime.Now.ToString();
                            cmdI.Parameters.Add(new SqlParameter("@DOMINIO", SqlDbType.Int)).Value = DominoOrigen;
                            if (cmdI.ExecuteNonQuery() == 1)
                            {

                                richTextBox2.Text = richTextBox2.Text + ListaNombres[c] + " , " + ListaCorreos[c] + "\n";
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }
        }

        private void btnAbrirArchivo_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            richTextBox1.Text = fileContent;
            RutaDeArchivo = filePath.ToString();

            //Separar Nombre y Email en listas diferentes
            using (TextFieldParser parser = new TextFieldParser(@"" + RutaDeArchivo))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                while (!parser.EndOfData)
                {
                    //Console.WriteLine("Linea:");
                    var fields = parser.ReadFields();
                    foreach (var field in fields)
                    {
                        if (campo == true)
                        {
                            campo = false;
                            ListaNombres.Add(field);
                        }
                        else
                        {
                            campo = true;
                            ListaCorreos.Add(field);
                        }
                    }
                }
            }
        }
        private string sql = @"server=192.168.2.20; database=CORREO ; integrated security = true";
        private bool campo = true;
        private int DominoOrigen = 0;
        List<string> ListaNombres = new List<string>();
        List<string> ListaCorreos = new List<string>();
        private String RutaDeArchivo = "";

        private void toolStripVelocidadWeb_Click(object sender, EventArgs e)
        {
            
        }

        private void btnVerContactos_Click(object sender, EventArgs e)
        {

        }

        private void cbbDominioOrigen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbDominioOrigen.Text=="equipodeproteccion.com")
            {
                DominoOrigen = 1;
            }else
            if (cbbDominioOrigen.Text == "gruposte.com")
            {
                DominoOrigen = 2;
            }else
            if (cbbDominioOrigen.Text == "cursodeprimerosauxilios.com.mx")
            {
                DominoOrigen = 4;
            }
        }
    }
}
