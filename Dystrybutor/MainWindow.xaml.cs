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
using System.IO;
using static Dystrybutor.Config;

namespace Dystrybutor
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }






        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Calculations();
            dg.ItemsSource = refuelingList;
            amountSum.Text = refuelingList.Sum(item => item.fuelAmount).ToString();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {

        }


        public void Calculations()
        {
            FileInfo fi = new FileInfo(@"0101.txt");
            using (var reader = new StreamReader(@"0101.txt"))
            {
                
                int lineNumber = 0;
                bool isRefuelingInProgress = false;
                int beginValue = 0;
                DateTime startTime = new DateTime(1999, 9, 9);
                double fuelAmount = 0.0;

                Config.Frame currentFrame = new Config.Frame();
                Config.Frame lastFrame = new Config.Frame();
                Config.Frame lastLastFrame = new Config.Frame();

                while (!reader.EndOfStream)
                {
                    if (lineNumber > 1)
                    {

                        var line = reader.ReadLine();
                        var currentLineValues = line.Split(';');

                        currentFrame.dateTime = Convert.ToDateTime(currentLineValues[0]);
                        currentFrame.l7 = Convert.ToInt32(currentLineValues[5]);
                        currentFrame.dallas1 = Convert.ToInt32(currentLineValues[11]);
                        currentFrame.dallas2 = Convert.ToInt32(currentLineValues[12]);

                        if (lineNumber == 1)
                        {
                            lastLastFrame.dateTime = currentFrame.dateTime;
                            lastLastFrame.l7 = currentFrame.l7;
                            lastLastFrame.dallas1 = currentFrame.dallas1;
                            lastLastFrame.dallas2 = currentFrame.dallas2;
                        }
                        if (lineNumber == 2)
                        {
                            lastFrame.dateTime = currentFrame.dateTime;
                            lastFrame.l7 = currentFrame.l7;
                            lastFrame.dallas1 = currentFrame.dallas1;
                            lastFrame.dallas2 = currentFrame.dallas2;
                        }



                        // **************** NA PODSTAWIE IMPULSÓW ***********************//

                        if (lineNumber >= 3 && false)
                        {
                            if (currentFrame.l7 == lastFrame.l7)
                            {
                                // Nic się nie dzieje
                                if (currentFrame.l7 == lastLastFrame.l7)
                                {

                                }

                                // Zakończenie tankowania
                                else if (currentFrame.l7 > lastLastFrame.l7)
                                {
                                    fuelAmount = (currentFrame.l7 - beginValue) * converter;

                                    refuelingList.Add(new Refueling
                                    {
                                        startTime = Convert.ToDateTime(startTime),
                                        fuelAmount = Convert.ToDouble(fuelAmount)
                                    });

                                    Console.WriteLine(startTime + " : " + Math.Round(fuelAmount, 2) + "L");

                                }
                            }

                            else if (currentFrame.l7 > lastFrame.l7)
                            {
                                // Start tankowania
                                if (lastFrame.l7 == lastLastFrame.l7)
                                {
                                    beginValue = lastFrame.l7;
                                    startTime = currentFrame.dateTime;
                                }

                                // Kontynuacja tankowania
                                else if (lastFrame.l7 > lastLastFrame.l7)
                                {

                                }
                            }


                            lastLastFrame.l7 = lastFrame.l7;
                            lastFrame.l7 = currentFrame.l7;
                        }
                        /////////////////////////////////////////////////////////





                        // ************** NA PODSTAWIE LOGOWANIA DALLAS *****************//

                        if (lineNumber >= 3 && false)
                        {
                            if (currentFrame.dallas1 != lastFrame.dallas1 && currentFrame.dallas1 * lastFrame.dallas1 == 0)
                            {

                                // Zakończenie tankowania
                                if (currentFrame.dallas1 == 0)
                                {
                                    fuelAmount = (currentFrame.l7 - beginValue) * converter;

                                    refuelingList.Add(new Refueling
                                    {
                                        startTime = Convert.ToDateTime(startTime),
                                        fuelAmount = Convert.ToDouble(fuelAmount),
                                        dallas1 = lastLastFrame.dallas1.ToString()
                                    }); ;

                                    Console.WriteLine(startTime + " : " + Math.Round(fuelAmount, 2) + "L");

                                }

                                // Start tankowania
                                else if (currentFrame.dallas1 != 0)
                                {
                                    beginValue = lastFrame.l7;
                                    startTime = currentFrame.dateTime;
                                }
                            }

                            lastLastFrame.l7 = lastFrame.l7;
                            lastLastFrame.dallas1 = lastFrame.dallas1;
                            lastLastFrame.dallas2 = lastLastFrame.dallas2;
                            lastFrame.l7 = currentFrame.l7;
                            lastFrame.dallas1 = currentFrame.dallas1;
                            lastFrame.dallas2 = currentFrame.dallas2;

                        }



                        /////////////////////////////////////////////////////////



                        // ************** BEZ LOGOWANIA DALLAS *****************//
                        if (lineNumber >= 3)
                        {
                            // Start tankowania
                            if (isRefuelingInProgress == false && currentFrame.l7 > lastFrame.l7 && currentFrame.dallas1 == 0)
                            {
                                isRefuelingInProgress = true;
                                beginValue = lastFrame.l7;
                                startTime = lastFrame.dateTime;
                            }

                            // Zakończenie tankowania
                            if (isRefuelingInProgress && currentFrame.l7 == lastLastFrame.l7)
                            {
                                fuelAmount = (currentFrame.l7 - beginValue) * converter;

                                refuelingList.Add(new Refueling
                                {
                                    startTime = Convert.ToDateTime(startTime),
                                    fuelAmount = Convert.ToDouble(fuelAmount),
                                    dallas1 = lastLastFrame.dallas1.ToString()
                                }); ;

                                Console.WriteLine(startTime + " : " + Math.Round(fuelAmount, 2) + "L");
                                isRefuelingInProgress = false;
                            }


                            lastLastFrame.l7 = lastFrame.l7;
                            lastLastFrame.dallas1 = lastFrame.dallas1;
                            lastLastFrame.dallas2 = lastLastFrame.dallas2;
                            lastLastFrame.dateTime = lastLastFrame.dateTime;
                            lastFrame.l7 = currentFrame.l7;
                            lastFrame.dallas1 = currentFrame.dallas1;
                            lastFrame.dallas2 = currentFrame.dallas2;
                            lastFrame.dateTime = currentFrame.dateTime;

                        }
                    }

                    //if (lineNumber == 20000) break;
                    if (lineNumber% 1000 == 0)
                    {
                        int x = (int)((lineNumber * 120) / (fi.Length/100));
                        Console.WriteLine(x +  "%    Done");
                    }

                    lineNumber++;
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
