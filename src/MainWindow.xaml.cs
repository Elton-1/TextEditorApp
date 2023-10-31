using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Microsoft.Win32;


namespace textEditorApp
{

    /*<SUMMARY> MOST OF THE LOGIC IS HERE!<SUMMARY>*/

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string[] paths = { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty };
        private Button? currentTab = null;

        private void pushPathToFilesAndDisplayTab(String path)
        {
            if (paths.Length < 5) return;

            bool found = false;
            int index = 0;
            foreach (string s in paths)
            {
                if (s == String.Empty)
                {
                    paths[index] = path;
                    found = true;
                    displayTab(index);
                    break;
                }

                index++;
            }

            if (!found)
            {
                MessageBox.Show("Cannot open more than 6 files at a time!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void displayTab(int index)
        {
            if (index > 7 || index < 0) throw new IndexOutOfRangeException();

            switch (index)
            {
                case 0: tabOne.Visibility = Visibility.Visible; break;
                case 1: tabTwo.Visibility = Visibility.Visible; break;
                case 2: tabThree.Visibility = Visibility.Visible; break;
                case 3: tabFour.Visibility = Visibility.Visible; break;
                case 4: tabFive.Visibility = Visibility.Visible; break;
                case 5: tabSix.Visibility = Visibility.Visible; break;
                case 6: tabSeven.Visibility = Visibility.Visible; break;
                default: throw new IndexOutOfRangeException("Index must be between 0 and 6");
            }
        }

        private async Task readFileAndDisplayAsync(Button clickedBtn)
        {
            Button clickedButton = clickedBtn;

            int index = 0;

            switch (clickedButton.Name)
            {
                case "tabTwo" when paths[1] != String.Empty: index = 1; break;
                case "tabThree" when paths[2] != String.Empty: index = 2; break;
                case "tabFour" when paths[3] != String.Empty: index = 3; break;
                case "tabFive" when paths[4] != String.Empty: index = 4; break;
                case "tabSix" when paths[5] != String.Empty: index = 5; break;
                case "tabSeven" when paths[6] != String.Empty: index = 6; break;
            }

            try { 
            using (StreamReader sr = new StreamReader(paths[index]))
            {
                String content = await sr.ReadToEndAsync();
                TextBox.Text = content;
            }
            }
            catch
            {
                MessageBox.Show("Cannot open That file make sure the name is valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            currentTab = clickedButton;
        }

        private async void tabOnClickAsync(object sender, RoutedEventArgs e)
        {
            await readFileAndDisplayAsync((Button)sender);
        }

        private void CloseCurrentTab()
        {
            if (currentTab is not null)
            {
                switch (currentTab.Name)
                {
                    case "tabOne": paths[0] = String.Empty; break;
                    case "tabTwo": paths[1] = String.Empty; break;
                    case "tabThree": paths[2] = String.Empty; break;
                    case "tabFour": paths[3] = String.Empty; break;
                    case "tabFive": paths[4] = String.Empty; break;
                    case "tabSix": paths[5] = String.Empty; break;
                    case "tabSeven": paths[6] = String.Empty; break;

                }

                TextBox.Text = String.Empty;
                currentTab.Visibility = Visibility.Collapsed;

                currentTab = null;
            }
        }

        private void displayTabName()
        {
            int index = 0;
            foreach (var path in paths)
            {
                if (path is not null)
                {
                    switch (index)
                    {
                        case 0: tabOne.Content = System.IO.Path.GetFileName(path); break;
                        case 1: tabTwo.Content = System.IO.Path.GetFileName(path); break;
                        case 2: tabThree.Content = System.IO.Path.GetFileName(path); break;
                        case 3: tabFour.Content = System.IO.Path.GetFileName(path); break;
                        case 4: tabFive.Content = System.IO.Path.GetFileName(path); break;
                        case 5: tabSix.Content = System.IO.Path.GetFileName(path); break;
                        case 6: tabSeven.Content = System.IO.Path.GetFileName(path); break;
                    }
                }
                index++;
            }
        }

        private async void openFileBtnOnClickAsync(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                pushPathToFilesAndDisplayTab(openFileDialog.FileName);
                if(currentTab is null) currentTab = tabOne;
                await readFileAndDisplayAsync(currentTab);
                displayTabName();

            }
        }

        private void closeFileBtnOnClick(object sender, RoutedEventArgs e)
        {
            CloseCurrentTab();
        }

        private async void saveFileBtnOnClickAsync(object sender, RoutedEventArgs e)
        {
            String path = "";

            if (currentTab is not null)
            {
                switch (currentTab.Name)
                {
                    case "tabOne": path = paths[0]; break;
                    case "tabTwo": path = paths[1]; break;
                    case "tabThree": path = paths[2]; break;
                    case "tabFour": path = paths[3]; break;
                    case "tabFive": path = paths[4]; break;
                    case "tabSix": path = paths[5]; break;
                    case "tabSeven": path = paths[6]; break;
                }   
            }

            if (File.Exists(path) || path != "")
            {
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    await sw.WriteLineAsync(TextBox.Text);
                }
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text Files|*.txt|All Files|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    Task write = File.WriteAllTextAsync(filePath, TextBox.Text);
                    pushPathToFilesAndDisplayTab(filePath);
                    if (currentTab is null) currentTab = tabOne;
                    await Task.WhenAll(write, readFileAndDisplayAsync(currentTab));
                    displayTabName();
                }
            }
        }

       
    }

}
