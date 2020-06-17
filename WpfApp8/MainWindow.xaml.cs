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

using System.Collections;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

using iTextSharp.text.pdf;

using iTextSharp.text.pdf.parser;

namespace WpfApp8
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        char[,] alfB = new char[6, 6];
        bool b = false;
        char[] alf = new char[] { 'A', 'D', 'F', 'G', 'V', 'X' };
        Random rand = new Random();
        public MainWindow()
        {
            InitializeComponent();
            char ch = 'A';
            for (int i = 0; i < 36; i++)
            {
                int ii = rand.Next(0, 6);
                int jj = rand.Next(0, 6);
                while (alfB[ii, jj] != '\0')
                {
                    ii = rand.Next(0, 6);
                    jj = rand.Next(0, 6);
                }
                alfB[ii, jj] = ch++;
                if (ch > 'Z') ch = '0';

            }
            tbl1.Text += "     A D F G V X\n";
            tbl1.Text += " ____________ \n";
            string[] tstr = new string[] { "A", "D", "F", "G", "V", "X" };
            for (int i = 0; i < 6; i++)
            {
                tbl1.Text += tstr[i] + "| ";
                for (int j = 0; j < 6; j++)
                {
                    tbl1.Text += " " + alfB[i, j];
                }
                tbl1.Text += "\n";
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (b == false) { MessageBox.Show("Загрузите книгу"); return; }
            if (tb2.Text == "") { MessageBox.Show("Введите текст"); return; }
            string s = tb2.Text;
            string s1 = "";
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                if (!Char.IsLetterOrDigit(ch)) continue;
                if (((ch >= 'а' && ch <= 'я') || (ch >= 'А' && ch <= 'Я')) && ch != ' ') continue;

                int iid = 0, jid = 0;
                for (int ii = 0; ii < 6; ii++)
                {
                    for (int jj = 0; jj < 6; jj++)
                    {
                        if (char.ToUpper(ch) == alfB[ii, jj]) { jid = jj; iid = ii; }
                    }
                }
                if (ch == ' ') s1 += ch;
                else
                {
                    s1 += alf[iid].ToString() + alf[jid].ToString() + "";
                }


            }

            MessageBox.Show(s1);
            string s2 = "";
            for (int i = 0; i < s1.Length; i++)
            {
                char ch = s1[i];
                int idstr = 0, idstroki = 0, idsimv = 0;

                for (int ii = 0; ii < pdfReader.NumberOfPages; ii++)
                {
                    string ss = PdfTextExtractor.GetTextFromPage(pdfReader, ii + 1);
                    string[] ss1 = ss.Split('\n');
                    //pdfReader = new PdfReader(s);
                    for (int iii = 0; iii < ss1.Length; iii++)
                    {
                        if (ss1[iii].IndexOf(ch) != -1) { idsimv = ss1[iii].IndexOf(ch) + 1; idstroki = iii + 1; break; }
                    }
                    if (idstroki != 0) { idstr = ii + 1; break; }
                }
                s2 += idstr + ";" + idstroki + ";" + idsimv + "|";

            }
            s2 = s2.Remove(s2.Length - 1);
            tb3.Text = s2;
        }
        PdfReader pdfReader;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string filename = "";
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "";
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "Text documents|*.pdf";

            Nullable<bool> result = dlg.ShowDialog();
            filename = dlg.FileName;


            if (result == true)
            {
                string str = filename;
                pdfReader = new PdfReader(str);
                for (int i = 0; i < pdfReader.NumberOfPages; i++)
                {
                    tb1.Text += "\n Page -" + (i + 1) + " \n" + PdfTextExtractor.GetTextFromPage(pdfReader, i + 1);
                }

                if (str.Length > 48) str = str.Insert(48, "|");
                if (str.Length > 96) str = str.Insert(96, "|");
                str = str.Replace("|", "\n");
                Label1.Content = str;
                string[] ss = tb1.Text.Split('\n');
                b = true;
            }



        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (b == false) { MessageBox.Show("Загрузите книгу"); return; }
            string s = tb2_Copy.Text;
            string[] ss = s.Split('|');
            if (ss.Length == 0 || ss.Length % 2 != 0) { MessageBox.Show("Ввод некоректный "); return; }

            for (int i = 0; i < ss.Length; i++)
            {
                string[] sst = ss[i].Split(';');
                if (sst.Length != 3) { MessageBox.Show("Ввод некоректный "); return; }
            }

            string s1 = "";
            for (int i = 0; i < ss.Length; i++)
            {
                string[] sst = ss[i].Split(';');
                int straniza = Convert.ToInt32(sst[0]);
                int stroka = Convert.ToInt32(sst[1]) - 1;
                int simv = Convert.ToInt32(sst[2]) - 1;

                string sss = PdfTextExtractor.GetTextFromPage(pdfReader, straniza);
                string[] sss1 = sss.Split('\n');
                string ts = sss1[stroka];
                char ts1 = ts[simv];

                s1 += ts1.ToString();
            }


            string s2 = "";
            for (int i = 0; i < s1.Length; i += 2)
            {
                int peid = 0, poid = 0; ;
                for (int j = 0; j < 6; j++)
                {
                    if (s1[i] == alf[j]) { peid = j; }
                    if (s1[i + 1] == alf[j]) { poid = j; }
                }
                s2 += alfB[peid, poid];
            }


            tb3_Copy.Text += s2;
        }
    }
}
