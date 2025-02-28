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
using Fuel_Consumption_Analysis.Model;

namespace Fuel_Consumption_Analysis.pages
{
    /// <summary>
    /// Логика взаимодействия для AddRedoRecord.xaml
    /// </summary>
    public partial class AddRedoRecord : Page
    {
        Cars _cars;
        float result,distance,consumption;
        public AddRedoRecord(Cars cars)
        {
            InitializeComponent();
            this._cars = cars;
            cmbModel.IsEnabled = false;
            var db = Fuel_Consumption_AnalysisEntities.GetContext();

            cmbMark.ItemsSource = db.CarMarks.Select(m => m.Title).ToList();
            
            if(cars == null)
            {
                return;
            }
            cmbModel.IsEnabled = false;

            cmbMark.SelectedItem = cars.Mark;
            cmbModel.SelectedItem = cars.Model;
            txtConsumption.Text = cars.Consumption.ToString();
            txtDistance.Text = cars.Distance.ToString();

        }

        private void SaveNewRecord()
        {
            var db = Fuel_Consumption_AnalysisEntities.GetContext();
            Cars cars =  new Cars();
            Guid uuid = Guid.NewGuid();
            cars.CarID = "ID_" + uuid.ToString().Substring(0, 5); 
            cars.Distance = distance;
            cars.Consumption = consumption;
            cars.Result = result;
            cars.MarkID = db.CarMarks.Where(m => m.Title == cmbMark.SelectedItem.ToString()).First().MarkID;
            cars.ModelID = db.CarModels.Where(m => m.Title == cmbModel.SelectedItem.ToString()).First().ModelID;
            cars.Date = DateTime.Now;

            db.Cars.Add(cars);
            db.SaveChanges();
        }

        private void UpdateRecord()
        {
            var db = Fuel_Consumption_AnalysisEntities.GetContext();
            _cars.Distance = distance;
            _cars.Consumption = consumption;
            _cars.Result = result;
            _cars.MarkID = db.CarMarks.Where(m => m.Title == cmbMark.SelectedItem.ToString()).First().MarkID;
            _cars.ModelID = db.CarModels.Where(m => m.Title == cmbModel.SelectedItem.ToString()).First().ModelID;
            _cars.Date = DateTime.Now;

            
            db.SaveChanges();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (!checkFields())
            {
                return;
            }

            this.result = (float)Math.Round(distance * consumption / 100); ;
            txtResult.Text = result.ToString();

            if(_cars == null)
            {
                SaveNewRecord();
                return;
            }
            
            UpdateRecord();
        }

        private bool checkFields()
        {
            if(string.IsNullOrWhiteSpace(txtConsumption.Text.Trim()))
            {
                MessageBox.Show("Неверно введен расход топлива: " + txtConsumption.Text);
                return false;
            }

            if( string.IsNullOrWhiteSpace(txtDistance.Text.Trim()))
            {
                MessageBox.Show("Неверно введена дистанция: " + txtDistance.Text);
                return false;
            }

            if(!float.TryParse(txtConsumption.Text.Trim(), out consumption))
            {
                MessageBox.Show("Неверно введен расход топлива: " + txtConsumption.Text);
                return false;
            }

            if (!float.TryParse(txtDistance.Text.Trim(), out distance))
            {
                MessageBox.Show("Неверно введена дистанция: " + txtDistance.Text);
                return false;
            }

            if(cmbMark.SelectedValue == null)
            {
                MessageBox.Show("Выберите марку машины.");
                return false;
            }

            if (cmbModel.SelectedValue == null)
            {
                MessageBox.Show("Выберите модель машины.");
                return false;
            }

            return true;
        }

        private void cmbMark_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbModel.IsEnabled = true;
            var db = Fuel_Consumption_AnalysisEntities.GetContext();
            string markID = db.CarMarks.Where(m => m.Title == cmbMark.SelectedItem.ToString()).FirstOrDefault().MarkID;

            cmbModel.ItemsSource = db.CarModels.Where(m => m.MarkID == markID).Select(m => m.Title).ToList();
        }
    }
}
