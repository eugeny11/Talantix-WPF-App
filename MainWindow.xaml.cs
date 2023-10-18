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
using System.Globalization;
using System.Configuration;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace test_full_stack_app
{
    public partial class MainWindow : Window
    {

          public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private Dictionary<string, List<string>> cityToDepartments = new Dictionary<string, List<string>>()
        {
            {"Москва", new List<string> {"Цех1", "Цех2"}},
            {"Санкт-Петербург", new List<string> {"Цех3", "Цех4"}},
            {"Екатеринбург", new List<string> {"Цех5", "Цех6"}}
        };

        private Dictionary<string, List<string>> departmentToEmployees = new Dictionary<string, List<string>>()
        {
            {"Цех1", new List<string> {"Иван", "Мария"}},
            {"Цех2", new List<string> {"Андрей", "Ольга"}},
            {"Цех3", new List<string> {"Сергей", "Наталья"}},
            {"Цех4", new List<string> {"Павел", "Елена"}},
            {"Цех5", new List<string> {"Алексей", "Татьяна"}},
            {"Цех6", new List<string> {"Дмитрий"}}
        };

private void LoadData()
{
    CityComboBox.ItemsSource = cityToDepartments.Keys;
    ShiftComboBox.ItemsSource = new List<string> {"Смена1", "Смена2", "Смена3"};

    var currentHour = DateTime.Now.Hour;
    if (currentHour >= 8 && currentHour < 20)
        Shift1RadioButton.IsChecked = true;
    else
        Shift2RadioButton.IsChecked = true;

    CityComboBox.SelectedIndex = 0;

    CityComboBox_SelectionChanged(CityComboBox, null);
}



      

        

private void CityComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
{
    if (CityComboBox.SelectedItem != null)
    {
        string selectedCity = CityComboBox.SelectedItem.ToString();

        if (cityToDepartments[selectedCity].Count > 0)
        {
            DepartmentComboBox.ItemsSource = cityToDepartments[selectedCity];
            DepartmentComboBox.SelectedIndex = 0;
            DepartmentComboBox.IsEnabled = true;
        }
        else
        {
            DepartmentComboBox.ItemsSource = null;
            DepartmentComboBox.IsEnabled = false;
        }

        UpdateEmployeeComboBox();
    }
}

private void UpdateEmployeeComboBox()
{
    if (DepartmentComboBox.SelectedItem != null)
    {
        string selectedDepartment = DepartmentComboBox.SelectedItem.ToString();

        if (departmentToEmployees.ContainsKey(selectedDepartment) && departmentToEmployees[selectedDepartment].Count > 0)
        {
            EmployeeComboBox.ItemsSource = departmentToEmployees[selectedDepartment];
            EmployeeComboBox.SelectedIndex = 0;
            EmployeeComboBox.IsEnabled = true;
        }
        else
        {
            EmployeeComboBox.ItemsSource = null;
            EmployeeComboBox.IsEnabled = false;
        }
    }
}




private void DepartmentComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
{
    if (DepartmentComboBox.SelectedItem != null)
    {
        string selectedDepartment = DepartmentComboBox.SelectedItem.ToString();

        if (departmentToEmployees.ContainsKey(selectedDepartment) && departmentToEmployees[selectedDepartment].Count > 0)
        {
            EmployeeComboBox.ItemsSource = departmentToEmployees[selectedDepartment];
            EmployeeComboBox.SelectedIndex = 0;
            EmployeeComboBox.IsEnabled = true;
        }
        else
        {
            EmployeeComboBox.ItemsSource = null; 
            EmployeeComboBox.IsEnabled = false;
        }
    }
}






private void SaveButton_Click(object sender, RoutedEventArgs e)
{
    if (CityComboBox.SelectedItem == null ||
        DepartmentComboBox.SelectedItem == null ||
        EmployeeComboBox.SelectedItem == null ||
        (Shift1RadioButton.IsChecked == false && Shift2RadioButton.IsChecked == false))
    {
        MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
        return; 
    }

    try
    {
        var selectedData = new
        {
            City = CityComboBox.SelectedItem.ToString(),
            Department = DepartmentComboBox.SelectedItem.ToString(),
            Employee = EmployeeComboBox.SelectedItem.ToString(),
            Shift = Shift1RadioButton.IsChecked == true ? "Первая (8:00 - 20:00)" : "Вторая (20:00 - 8:00)"
        };

        var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(selectedData);
        System.IO.File.WriteAllText("selectedData.json", jsonData);

        MessageBox.Show("Данные успешно сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private void SaveToDbButton_Click(object sender, RoutedEventArgs e)
{
    if (CityComboBox.SelectedItem == null ||
        DepartmentComboBox.SelectedItem == null ||
        EmployeeComboBox.SelectedItem == null ||
        (Shift1RadioButton.IsChecked == false && Shift2RadioButton.IsChecked == false))
    {
        MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }

    string city = CityComboBox.SelectedItem.ToString();
    string department = DepartmentComboBox.SelectedItem.ToString();
    string employee = EmployeeComboBox.SelectedItem.ToString();
    string shift = Shift1RadioButton.IsChecked == true ? "Первая (8:00 - 20:00)" : "Вторая (20:00 - 8:00)";

    string connectionString = "";

    try
    {
        connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ShiftSchedulerConnection"].ConnectionString;
        MessageBox.Show($"Connection String: {connectionString}");
        SaveToDatabase(city, department, employee, shift);
        MessageBox.Show("Данные успешно сохранены в базу данных!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Произошла ошибка при сохранении данных в базу: {ex.Message}\n{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

    }
}

private void SaveToDatabase(string city, string department, string employee, string shift)
{
    string connectionString = "";
    try
    {
        connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ShiftSchedulerConnection"].ConnectionString;
    }
    catch (Exception ex)
    {
        MessageBox.Show("Ошибка при получении строки подключения: " + ex.Message);
        return;
    }

    try
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO shifts (City, Department, Employee, Shift) VALUES (@City, @Department, @Employee, @Shift)";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@City", city);
            cmd.Parameters.AddWithValue("@Department", department);
            cmd.Parameters.AddWithValue("@Employee", employee);
            cmd.Parameters.AddWithValue("@Shift", shift);

            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message);
    }
}



    }

    public class NullToBorderColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Brushes.Red : Brushes.Gray; 
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
}
