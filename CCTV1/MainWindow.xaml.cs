using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using Vlc.DotNet.Wpf;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Interops;
using System.Reflection;

namespace CCTV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    /*
Перед загрузкой надо обновить ссылки на изображения в папке проекта с названием "Resources", т.к. я не смог реализовать решение данной проблемы.
Ссылки находятся в начале кода.

1. Реализована панель переключения между вкладками. созданo с помощью xaml.
2. В центральной части установлены картинки заглушки. созданo с помощью xaml.
3. Событие зажатия клавиш shift+F1 добавляется картинка заглушка и cоздается кнопка в нижней панели. C# код.
4. Нижняя панель создана с помощью xaml.
5. Вкладка Список - не реализована.
6. Вкладка настройки:
	6.1. НАстройка главного окна. Работает отображение 1 "потока", 2х и 4х. 3 потока реализовать не получилось.
	6.2. Добавление новых каналов не реализовано.

UPDATE

7. Добавлена функциональность вкладки список. (Для наглядности отображение списка создается кнопками в коде)
7.1. при нажатии на кнопки в нижней части отображается связаная картинка-заглушка.*/
    public partial class MainWindow : Window
    {
        //Используется VLC плеер. Поэтому он должен быть у вас установлен
        private readonly DirectoryInfo vlcLibDirectory;
        private VlcControl control;

        //NullReferenceException путь лежить к папке CCTV1\bin\Debug\netcoreapp3.1\
        //string source4 = Directory.GetCurrentDirectory() + @"\Resources\4.jpg";

        string source1 = @""+@"\Resources\1.jpg";
        string source2 = @""+@"\Resources\2.jpg";
        string source3 = @""+@"\Resources\3.jpg";
        string source4 = @""+@"\Resources\4.jpg";
        
        //Тут должен быть адрес rtsp потока
        string source5 = "";

        public MainWindow()
        {

            InitializeComponent();
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            //Кнопки для вкладки Список создаются в коде.
            Button[] buttons = new Button[3];
            CreateButtonsForList(buttons);
            for (int i = 0; i < buttons.Length; i++)
            {
                
                Grid.SetRow(buttons[i], i + 1);
                Grid.SetColumnSpan(buttons[i], 5);
                GridListRow2.Children.Add(buttons[i]);
            }
            buttons[0].Click += ButtonInList_Click_1;
            buttons[1].Click += ButtonInList_Click_2;
            buttons[2].Click += ButtonInList_Click_3;

            //Отображение rtsp потока в грид с использованием NuGet расширения Vlc.DotNet.Wpf
            try
            {
                this.control?.Dispose();
                this.control = new VlcControl();
                this.ControlContainer.Content = this.control;
                this.control.SourceProvider.CreatePlayer(this.vlcLibDirectory);

                // This can also be called before EndInit
                this.control.SourceProvider.MediaPlayer.Log += (_, args) =>
                {
                    string message = $"libVlc : {args.Level} {args.Message} @ {args.Module}";
                    System.Diagnostics.Debug.WriteLine(message);
                };
                control.SourceProvider.MediaPlayer.Play(new Uri(source5));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public class Car
        {
            private System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            public string Number { get; set; }
            public System.Windows.Controls.Image Img { get { return img; } set { img = value; } }

            public string Date { get; set; }
            public int Channel { get; set; }
            public enum Statuses : byte { Out, In };
            public Statuses Status { get; set; }
            public Car()
            {

                this.Channel = 0;
                this.Date = null;
                this.Number = null;
                this.Status = 0;
                this.Img = null;
            }
            public Car(int channel, DateTime date, string number, int status)
            {
                this.Number = number;
                this.Date = date.ToString("HH:mm dd.MM ");
                this.Channel = channel;
                this.Status = (Statuses)status;
            }
            public Car(int channel, DateTime date, string number, int status, string imgSource)
            {
                this.Number = number;
                this.Date = date.ToString("HH:mm dd.MM");
                this.Channel = channel;
                this.Status = (Statuses)status;
                img.Source = new BitmapImage(new Uri(imgSource));
            }
            public string ButtonString()
            {
                return $"{Channel}\n{Date}\n{Number}\n{Status}\n";
            }

        }
        //Событие комбинации клавиш
        private void PressKey(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift && e.Key == Key.F1)
            {
                Car car = new Car(12, DateTime.Now, "AA5432AA", 1);
                //Создание кнопки для события
                Button Button4 = new Button();
                Button4.HorizontalAlignment = HorizontalAlignment.Center;
                Button4.VerticalAlignment = VerticalAlignment.Center;
                Button4.Width = 80;
                Button4.Height = 70;

                TextBlock text = new TextBlock();
                text.TextAlignment = TextAlignment.Center;
                text.Text = car.ButtonString();

                Button4.Content = text;

                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Name = "Img4";
                img.Source = new BitmapImage(new Uri(source4));
                Button4.ToolTip = img;
                Button4.Background = System.Windows.Media.Brushes.DarkGray;
                car.Img.Source = new BitmapImage(new Uri(source4));
                Grid.SetColumn(Button4, 3);
                Grid.SetRow(Button4, 0);
                TileGridBottom.Children.Add(Button4);

                Grid.SetColumn(car.Img, 1);
                Grid.SetRow(car.Img, 1);
                TileGridCenter.Children.Add(car.Img);

                Button buttonToList = new Button();
                Grid buttonGrid = new Grid();
                ColumnDefinition[] colDef = new ColumnDefinition[12];
                for (int j = 0; j < colDef.Length; j++)
                {
                    colDef[j] = new ColumnDefinition();
                    colDef[j].Width = new GridLength(100, GridUnitType.Pixel);
                    buttonGrid.ColumnDefinitions.Add(colDef[j]);
                }
                TextBlock[] textBlocks = new TextBlock[5];
                for (int i = 0; i < textBlocks.Length; i++)
                {
                    textBlocks[i] = new TextBlock();
                    textBlocks[i].TextAlignment = TextAlignment.Left;
                }
                textBlocks[0].Text = " " + car.Channel.ToString();
                textBlocks[1].Text = " " + car.Date;
                textBlocks[2].Text = " " + car.Number;
                textBlocks[3].Text = "";
                textBlocks[4].Text = " " + car.Status.ToString();

                for (int j = 0; j < textBlocks.Length; j++)
                {
                    Grid.SetColumn(textBlocks[j], j);
                    buttonGrid.Children.Add(textBlocks[j]);
                }
                buttonToList.Content = buttonGrid;
                Grid.SetRow(buttonToList, 4);
                Grid.SetColumnSpan(buttonToList, 5);
                GridListRow2.Children.Add(buttonToList);

                buttonToList.Click += ButtonInList_Click_4;
            }
        }
        //Отображение 1й камеры
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateNetworkForGrid(1, 0.5, 1, 8, TileGridCenter, GridUnitType.Star);

            
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CreateNetworkForGrid(2, 2, 1, 1, TileGridCenter, GridUnitType.Star);
            Image image1 = new Image();
            image1.Source = new BitmapImage(new Uri(source1));

            Grid.SetRow(image1, 0);
            Grid.SetColumn(image1, 1);
            TileGridCenter.Children.Add(image1);

            Image image2 = new Image();
            image2.Source = new BitmapImage(new Uri(source2));

            Grid.SetRow(image2, 0);
            Grid.SetColumn(image2, 0);
            Grid.SetColumnSpan(image2, 1);
            TileGridCenter.Children.Add(image2);
            //TileGridCenter.Width = Convert.ToDouble(new GridLength(3, GridUnitType.Star));
            //TileGridCenter.Width = Convert.ToDouble(new GridLength(3, GridUnitType.Star));
            //mainGridRow1.Height = new GridLength(8, GridUnitType.Star);
            //mainGridRow2.Height = new GridLength(0, GridUnitType.Star);


        }
        //Не смог реализовать отображение на 3х камер
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
        //Отображение 4х камер
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //mainGridColumn1.Width = new GridLength(3, GridUnitType.Star);
            //mainGridColumn2.Width = new GridLength(3, GridUnitType.Star);
            //mainGridRow1.Height = new GridLength(4, GridUnitType.Star);
            //mainGridRow2.Height = new GridLength(4, GridUnitType.Star);
        }
        private void ButtonInList_Click_1(object sender, RoutedEventArgs e)
        {

            CreateNetworkForGrid(3, 110, 1, 110, GridListRow3, GridUnitType.Pixel);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = new BitmapImage(new Uri(source1));
            image.Name = "Image1";
            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 1);
            Grid.SetColumnSpan(image, 2);
            GridListRow3.Children.Add(image);

        }
        private void ButtonInList_Click_2(object sender, RoutedEventArgs e)
        {
            CreateNetworkForGrid(3, 110, 1, 110, GridListRow3, GridUnitType.Pixel);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = new BitmapImage(new Uri(source2));
            image.Name = "Image1";
            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 1);
            Grid.SetColumnSpan(image, 2);
            GridListRow3.Children.Add(image);
        }
        private void ButtonInList_Click_3(object sender, RoutedEventArgs e)
        {
            CreateNetworkForGrid(3, 110, 1, 110, GridListRow3, GridUnitType.Pixel);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = new BitmapImage(new Uri(source3));
            image.Name = "Image1";
            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 1);
            Grid.SetColumnSpan(image, 2);
            GridListRow3.Children.Add(image);
        }
        private void ButtonInList_Click_4(object sender, RoutedEventArgs e)
        {
            CreateNetworkForGrid(3, 110, 1, 110, GridListRow3, GridUnitType.Pixel);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = new BitmapImage(new Uri(source4));
            image.Name = "Image4";
            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 1);
            Grid.SetColumnSpan(image, 2);
            GridListRow3.Children.Add(image);
        }
        
        public static void CreateNetworkForGrid(int columnCount, double Width, int rowCount, double Height, Grid gridName, GridUnitType gridUnitType)
        {
            gridName.Children.Clear();
            //gridName = new Grid();
            var columns = new ColumnDefinition[columnCount];
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = new ColumnDefinition();
                // columns[i].Name = gridName + "Column" + i;
                columns[i].Width = new GridLength(Width, gridUnitType);
                gridName.ColumnDefinitions.Add(columns[i]);

            }
            var rows = new RowDefinition[rowCount];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = new RowDefinition();
                rows[i].Height = new GridLength(Height, gridUnitType);
                gridName.RowDefinitions.Add(rows[i]);
            }
        }
        public static Button[] CreateButtonsForList(Button[] buttons)
        {
            
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new Button();
            }
            for (int i = 0; i < buttons.Length; i++)
            {
                Grid buttonGrid = new Grid();
                ColumnDefinition[] colDef = new ColumnDefinition[12];
                for (int j = 0; j < colDef.Length; j++)
                {
                    colDef[j] = new ColumnDefinition();
                    colDef[j].Width = new GridLength(100, GridUnitType.Pixel);
                    buttonGrid.ColumnDefinitions.Add(colDef[j]);
                }


                TextBlock[] textBlocks = new TextBlock[5];
                for (int j = 0; j < textBlocks.Length; j++)
                {
                    textBlocks[j] = new TextBlock();
                    textBlocks[j].TextAlignment = TextAlignment.Left;
                }
                textBlocks[0].Text = " 12";
                textBlocks[1].Text = " 13:00 23.02";
                textBlocks[2].Text = " AA5555AA";
                textBlocks[3].Text = "";
                textBlocks[4].Text = " In";

                for (int j = 0; j < textBlocks.Length; j++)
                {
                    Grid.SetColumn(textBlocks[j], j);
                    buttonGrid.Children.Add(textBlocks[j]);
                }
                buttons[i].Content = buttonGrid;
            }
            return buttons;
        }
    }
}
