using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace mapei
{
    public partial class FrmIzvestaj : Form
    {
        OleDbConnection conn = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = c:\\mapei\\mapei.mdb");
        DataSet ds;        
        
        public FrmIzvestaj( DataSet _ds)
        {
            InitializeComponent();
            ds = _ds;
            dgvIzvestaj.DataSource = ds.Tables["Izvestaj"];
            
        }

        private void btnKreirajIzvestaj_Click(object sender, EventArgs e)
        {
            try
            {
                double plastika=0;
                double papir=0;
                double celik=0;
                double aluminijum=0;



                OleDbCommand cmd1 = conn.CreateCommand();
                OleDbCommand cmd2 = conn.CreateCommand();
                cmd1.CommandText =
                    ("SELECT Naziv_ambalaze, SUM(Kolicina_otpadnog_materijala) AS Kolicina FROM (SELECT Proizvodi.id_proizvoda, Proizvodi.Naziv AS Naziv, Proizvodi.jm, Proizvodi.tare, Proizvodi.id_ambalaza, Ambalaza.naziv AS Naziv_ambalaze, Import.kolicina, Proizvodi.tare * Import.kolicina AS Kolicina_otpadnog_materijala FROM(Ambalaza INNER JOIN Proizvodi ON Ambalaza.[id_ambalaza] = Proizvodi.[id_ambalaza]) INNER JOIN Import ON Proizvodi.[id_proizvoda] = Import.[id_proizvoda]) GROUP BY Naziv_ambalaze"
                    );
                conn.Open();
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd1);

                if (ds.Tables["Otpad"] == null)
                {
                    adapter.Fill(ds, "Otpad");
                }
                else
                {
                    ds.Tables["Otpad"].Clear();
                    adapter.Fill(ds, "Otpad");
                }

                conn.Close();
                var defaultCultureForDb = new CultureInfo("en-US");

                // Naziv_ambalaze
                // Kolicina
                for (var index = 0; index < ds.Tables["Otpad"].Rows.Count; index++) {
                    switch (ds.Tables["Otpad"].Rows[index]["Naziv_ambalaze"])
                    {
                        case "aluminijum":
                            aluminijum = Convert.ToDouble(ds.Tables["Otpad"].Rows[index]["Kolicina"], defaultCultureForDb.NumberFormat);

                            break;
                        case "papir":
                            papir = Convert.ToDouble(ds.Tables["Otpad"].Rows[index]["Kolicina"], defaultCultureForDb.NumberFormat);
                            break;
                        case
                            "plastika":
                            plastika = Convert.ToDouble(ds.Tables["Otpad"].Rows[index]["Kolicina"], defaultCultureForDb.NumberFormat);
                            break;
                        case "celik":
                            celik = Convert.ToDouble(ds.Tables["Otpad"].Rows[index]["Kolicina"], defaultCultureForDb.NumberFormat);
                            break;
                    }
                }
                /*
                aluminijum = Convert.ToDouble(ds.Tables["Otpad"].Rows[0]["Kolicina"],defaultCultureForDb.NumberFormat);
                MessageBox.Show(Convert.ToString(aluminijum));
                papir = Convert.ToDouble(ds.Tables["Otpad"].Rows[1]["Kolicina"],defaultCultureForDb.NumberFormat);
                MessageBox.Show(Convert.ToString(papir));

                MessageBox.Show(Convert.ToString(ds.Tables["Otpad"].Rows[2]["Kolicina"]));
                

                celik = Convert.ToDouble(ds.Tables["Otpad"].Rows[2]["Kolicina"],defaultCultureForDb.NumberFormat);
                
                plastika = Convert.ToDouble(ds.Tables["Otpad"].Rows[3]["Kolicina"],defaultCultureForDb.NumberFormat);
                MessageBox.Show(Convert.ToString(plastika));
                */

                cmd2.CommandText =
                    ("INSERT INTO Izvestaj (naziv_import_tabele, datum_kreiranja_izvestaja, datum_od, datum_do, plastika, papir, celik, aluminijum) VALUES ('" +
                     txtNaziv.Text + "','" + DateTime.Now.ToString(defaultCultureForDb.DateTimeFormat) + "','" + dtpOd.Value.ToString(defaultCultureForDb.DateTimeFormat) + "','" +
                     dtpDo.Value.ToString(defaultCultureForDb.DateTimeFormat) + "','" + plastika.ToString(defaultCultureForDb) + "','" + celik.ToString(defaultCultureForDb) + "','" + papir.ToString(defaultCultureForDb) + "','" +
                     aluminijum.ToString(defaultCultureForDb) + "');");
                conn.Open();
                int insertedRows = cmd2.ExecuteNonQuery();
                MessageBox.Show("Uspesno dodati podaci u bazu");
                conn.Close();
                dgvIzvestaj.DataSource = null;
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM Izvestaj", conn);

                conn.Open();
                OleDbDataAdapter adapter_izvestaj = new OleDbDataAdapter(cmd);

                if (ds.Tables["Izvestaj"] == null)
                {
                    adapter_izvestaj.Fill(ds, "Izvestaj");
                }
                else
                {
                    ds.Tables["Izvestaj"].Clear();
                    adapter_izvestaj.Fill(ds, "Izvestaj");
                }

                conn.Close();

                dgvIzvestaj.DataSource = ds.Tables["Izvestaj"];
            }
            catch (InvalidOperationException invalidOpException)
            {
                MessageBox.Show(invalidOpException.Message);
                throw;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                throw;
            }
            finally
            {
                conn.Close();
            }
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
