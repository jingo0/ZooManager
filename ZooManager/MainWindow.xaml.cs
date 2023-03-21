using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Specialized;

namespace ZooManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection conn;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["ZooManager.Properties.Settings.PractiseDBConnectionString"].ConnectionString;

            conn = new SqlConnection(connectionString);

            ShowZoos();
            ShowAllAnimal();

        }

        private void ShowZoos()
        {
            try
            {
                string query = $"SELECT * FROM Zoo";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, conn);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);
                    listZoo.DisplayMemberPath = "Location";
                    listZoo.SelectedValuePath = "Id";
                    listZoo.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying ShowZoos: " + ex.ToString());
            }
        }

        private void listZoo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(listZoo.SelectedValue.ToString());
            if(listZoo.SelectedValue != null)
            {
                ShowSelectedZooInTextBox();
                ShowAssociatedAnimal();
            }        
        }

        private void listAnimal_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(AllAnimals.SelectedValue == null) return;
            ShowSelectedAnimalInTextBox();
        }

        private void ShowAssociatedAnimal()
        {
            try
            {
                string query = $"select * from Animal as a inner join ZooAnimal za on a.Id=za.Animalid where za.Zooid = {listZoo.SelectedValue}";

                //SqlCommand sqlCommand = new SqlCommand(query, conn);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, conn);

                using (sqlDataAdapter)
                {
                    //sqlCommand.Parameters.AddWithValue("@ZooId", listZoo.SelectedValue);
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);
                    AssociatedAnimals.DisplayMemberPath = "Name";
                    AssociatedAnimals.SelectedValuePath = "Id";
                    AssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying ShowAssociatedAnimal: " + ex.ToString());
            }
        }

        private void ShowAllAnimal()
        {
            try
            {
                string query = $"select * from Animal";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, conn);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);
                    AllAnimals.DisplayMemberPath = "Name";
                    AllAnimals.SelectedValuePath = "Id";
                    AllAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying ShowAllAnimal: " + ex.ToString());
            }
        }

        private void DeleteZoo_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = $"delete from Zoo where id = {listZoo.SelectedValue}";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying DeleteZoo_click: " + ex.ToString());
            }
            finally
            {
                conn.Close();
                ShowZoos();
            }

        }

        private void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = $"insert into Zoo values ('{MyTextBox.Text}')";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying AddZoo_Click: " + ex.ToString());
            }
            finally
            {
                conn.Close();
                ShowZoos();
            }
        }

        private void addAnimalToZoo(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = $"insert into ZooAnimal values ('{listZoo.SelectedValue}','{AllAnimals.SelectedValue}')";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying addAnimalToZoo: " + ex.ToString());
            }
            finally
            {
                conn.Close();
                ShowAssociatedAnimal();
            }
        }

        private void addAnimal_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = $"insert into Animal values ('{MyTextBox.Text}')";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying addAnimalToZoo: " + ex.ToString());
            }
            finally
            {
                conn.Close();
                ShowZoos();
                ShowAllAnimal();
            }
        }

        private void deleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            int idx = (int)AllAnimals.SelectedValue;
            try
            {
                string query = $"delete from Animal where id =('{AllAnimals.SelectedValue}')";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying deleteAnimal_Click: " + ex.ToString());
            }
            finally
            {
                conn.Close();
                ShowZoos();
                ShowAllAnimal();
                listZoo.SelectedIndex = idx;
                if(listZoo.SelectedValue!=null) ShowAssociatedAnimal();
            }
        }

        private void deleteAssociatedAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = $"delete from ZooAnimal where Animalid =('{AssociatedAnimals.SelectedValue}')";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying deleteAnimal_Click: " + ex.ToString());
            }
            finally
            {
                conn.Close();
                ShowAssociatedAnimal();
            }
        }

        private void ShowSelectedZooInTextBox()
        {
            try
            {
                string query = $"select Location from Zoo where Id={listZoo.SelectedValue}";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, conn);

                using (sqlDataAdapter)
                {
                    DataTable dataValue = new DataTable();
                    sqlDataAdapter.Fill(dataValue);
                    MyTextBox.Text = dataValue.Rows[0]["Location"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying ShowSelectedZooInTextBox: " + ex.ToString());
            }
        }

        private void ShowSelectedAnimalInTextBox()
        {
            try
            {
                string query = $"select Name from Animal where Id={AllAnimals.SelectedValue}";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, conn);

                using (sqlDataAdapter)
                {
                    DataTable dataValue = new DataTable();
                    sqlDataAdapter.Fill(dataValue);
                    MyTextBox.Text = dataValue.Rows[0]["Name"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying ShowSelectedAnimalInTextBox: " + ex.ToString());
            }
        }

        private void updateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = $"Update Zoo Set Location = ('{MyTextBox.Text}') where Id ={listZoo.SelectedValue}";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying updateZoo_Click: " + ex.ToString());
            }
            finally
            {
                conn.Close();
                ShowZoos();
            }
        }

        private void updateAnimal_Click(object sender, RoutedEventArgs e)
        {
            //var temp = AllAnimals.SelectedValue;
            try
            {
                string query = $"Update Animal Set Name = ('{MyTextBox.Text}') where Id ={AllAnimals.SelectedValue}";
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteScalar();
                //AllAnimals.SelectedItem = temp.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in querying updateAnimal_Click: " + ex.ToString());
            }
            finally
            {
                conn.Close();
                ShowAllAnimal();
                //MessageBox.Show("finally: " + temp.ToString());
                //AllAnimals.SelectedIndex = (int)temp;
            }
        }

    

    }
}
