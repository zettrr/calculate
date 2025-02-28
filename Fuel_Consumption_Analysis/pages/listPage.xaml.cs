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
    /// Логика взаимодействия для listPage.xaml
    /// </summary>
    public partial class listPage : Page
    {
        public listPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string[] SortingList { get; set; } =
        {
            "Без сортировки",
            "Сначала новые",
            "Сначала старые",
            "Больше расстояние",
            "Меньше расстояние",
            "Больше расход",
            "Меньше расход"
        };

        public string[] FilterList { get; set; } =
        {
            "Все результаты",
            "0-14.99",
            "15-29.99",
            "30 и более"
        };

        private void cmbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void txtSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            var result = Fuel_Consumption_AnalysisEntities.GetContext().Cars.ToList();

            result = result.Where(c => c.Mark.ToLower().Contains(txtSearch.Text.ToLower())
                             || c.Model.ToLower().Contains(txtSearch.Text.ToLower())).ToList();

            switch (cmbSorting.SelectedIndex)
            {
                case 1:
                    result = result.OrderByDescending(p => p.Date).ToList();
                    break;
                case 2:
                    result = result.OrderBy(p => p.Date).ToList();
                    break;
                case 3:
                    result = result.OrderByDescending(p => p.Distance).ToList();
                    break;
                case 4:
                    result = result.OrderBy(p => p.Distance).ToList();
                    break;
                case 5:
                    result = result.OrderByDescending(p => p.Consumption).ToList();
                    break;
                case 6:
                    result = result.OrderBy(p => p.Consumption).ToList();
                    break;

            }

            switch (cmbFilter.SelectedIndex)
            {
                case 1:
                    result = result.Where(s => s.Result < 15).ToList();
                    break;
                case 2:
                    result = result.Where(s => s.Result >= 15 && s.Result < 30).ToList();
                    break;
                case 3:
                    result = result.Where(s => s.Result >= 30).ToList();
                    break;
            }


            LViewCars.ItemsSource = result;

        }

        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            AddRedoRecord addRedoRecord = new AddRedoRecord(null);
            NavigationService.Navigate(addRedoRecord);
        }

        private void RedoRecord_Click(object sender, RoutedEventArgs e)
        {
            AddRedoRecord addRedoRecord = new AddRedoRecord(LViewCars.SelectedItem as Cars);
            NavigationService.Navigate(addRedoRecord);
        }

        private void DeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            if (LViewCars.SelectedItems.Count > 0)
            {
                if (MessageBox.Show($"Вы действительно хотите удалить запись?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    return;
                }

                var selected = LViewCars.SelectedItems.Cast<Cars>().ToArray();
                int carsCount = 0;
                var db = Fuel_Consumption_AnalysisEntities.GetContext();
                foreach (var item in selected)
                {
                    
                    db.Cars.Remove(item);
                    db.SaveChanges();
                    carsCount++;
                    
                }
                if (carsCount != 0)
                {
                    MessageBox.Show($"Удалено записей: {carsCount}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                UpdateData();

            }
            else
            {
                MessageBox.Show("Выберите агента для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LViewRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
